using Godot;

namespace BlackMana.AutoLoads;

internal sealed partial class SfxManager : Node
{
    internal const string ScenePath = $"{ScenePaths.Root}/{nameof(SfxManager)}";

    [Export] public AudioStream SelectSound { get; set; }
    [Export] public AudioStream DeselectSound { get; set; }
    [Export] public AudioStream ConfirmMoveSound { get; set; }

    private AudioStreamPlayer _player;

    public override void _Ready()
    {
        _player = new AudioStreamPlayer();
        AddChild(_player);
    }

    public void PlaySelect()
        => Play(SelectSound);

    public void PlayDeselect()
        => Play(DeselectSound);

    public void PlayConfirmMove()
        => Play(ConfirmMoveSound);

    private void Play(AudioStream stream)
    {
        if (stream is null)
            return;

        _player.Stream = stream;
        _player.Play();
    }
}
