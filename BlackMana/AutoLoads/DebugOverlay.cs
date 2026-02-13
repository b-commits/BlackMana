using System;
using System.Collections.Generic;
using System.Linq;
using BlackMana.Common.Actions;
using Godot;

namespace BlackMana.AutoLoads;

internal sealed partial class DebugOverlay : CanvasLayer
{
    internal const string ScenePath = $"{ScenePaths.Root}/{nameof(DebugOverlay)}";

    private readonly Dictionary<string, Func<Dictionary<string, string>>> _providers = new();
    private readonly Dictionary<string, Action<string>> _commands = new();
    private RichTextLabel _label;
    private LineEdit _input;
    private PanelContainer _panel;
    private bool _visible;

    public override void _Ready()
    {
        Layer = 100;
        BuildUi();
        RegisterBuiltInCommands();
        _panel.Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsActionPressed(ActionProvider.ToggleDebug))
            return;

        _visible = !_visible;
        _panel.Visible = _visible;

        if (_visible)
            _input.GrabFocus();
    }

    public override void _Process(double delta)
    {
        if (!_visible)
            return;

        _label.Clear();
        _label.PushMono();

        var header = $"[color=gray]Debug Overlay — {Engine.GetFramesPerSecond()} FPS[/color]";
        _label.AppendText($"{header}\n");

        foreach (var (name, provider) in _providers.OrderBy(p => p.Key))
        {
            _label.AppendText($"\n[color=yellow]▸ {name}[/color]\n");

            foreach (var (key, value) in provider())
            {
                var coloredValue = Colorize(value);
                _label.AppendText($"  {key,-16} {coloredValue}\n");
            }
        }

        _label.Pop();
    }

    public void Register(string name, Func<Dictionary<string, string>> provider)
        => _providers[name] = provider;

    public void Unregister(string name)
        => _providers.Remove(name);

    public void RegisterCommand(string name, Action<string> handler)
        => _commands[name] = handler;

    private void RegisterBuiltInCommands()
    {
        RegisterCommand("fps", value => Engine.MaxFps = int.Parse(value));
        RegisterCommand("timescale", value => Engine.TimeScale = float.Parse(value));
    }

    private void OnCommandSubmitted(string text)
    {
        _input.Clear();

        var separatorIndex = text.IndexOf('=');
        if (separatorIndex < 1)
        {
            LogResponse("[color=red]Invalid format. Use: command=value[/color]");
            return;
        }

        var command = text[..separatorIndex].Trim().ToLower();
        var value = text[(separatorIndex + 1)..].Trim();

        if (!_commands.TryGetValue(command, out var handler))
        {
            LogResponse($"[color=red]Unknown command: {command}[/color]");
            return;
        }

        try
        {
            handler(value);
            LogResponse($"[color=green]{command} = {value}[/color]");
        }
        catch (Exception ex)
        {
            LogResponse($"[color=red]Error: {ex.Message}[/color]");
        }
    }

    private void LogResponse(string bbcodeText)
    {
        _label.AppendText($"\n{bbcodeText}\n");
    }

    private static string Colorize(string value)
    {
        return value switch
        {
            "True" => "[color=green]True[/color]",
            "False" => "[color=red]False[/color]",
            "none" or "null" or "" => "[color=gray]—[/color]",
            _ => $"[color=cyan]{value}[/color]"
        };
    }

    private void BuildUi()
    {
        _panel = new PanelContainer();
        _panel.AnchorRight = 1.0f;
        _panel.AnchorBottom = 0.0f;
        _panel.OffsetBottom = 0;
        _panel.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
        _panel.SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;

        var styleBox = new StyleBoxFlat
        {
            BgColor = new Color(0.05f, 0.05f, 0.1f, 0.85f),
            ContentMarginLeft = 12,
            ContentMarginRight = 12,
            ContentMarginTop = 8,
            ContentMarginBottom = 8,
            CornerRadiusBottomRight = 6
        };
        _panel.AddThemeStyleboxOverride("panel", styleBox);

        var vbox = new VBoxContainer();
        _panel.AddChild(vbox);

        _label = new RichTextLabel
        {
            BbcodeEnabled = true,
            FitContent = true,
            ScrollActive = false,
            AutowrapMode = TextServer.AutowrapMode.Off,
            CustomMinimumSize = new Vector2(320, 0)
        };
        _label.AddThemeFontSizeOverride("normal_font_size", 13);
        _label.AddThemeFontSizeOverride("mono_font_size", 13);
        vbox.AddChild(_label);

        _input = new LineEdit
        {
            PlaceholderText = "command=value",
            CustomMinimumSize = new Vector2(320, 0),
            CaretBlink = true
        };
        _input.AddThemeFontSizeOverride("font_size", 13);
        _input.TextSubmitted += OnCommandSubmitted;
        vbox.AddChild(_input);

        AddChild(_panel);
    }
}