using BlackMana.Common.Movement;

namespace BlackMana.Scenes.Player;

internal static class PlayerAnimations
{
    internal static readonly CharacterAnimationSet AnimationSet = new(
        IdleFrame: "IdleFrame",
        IdleSelectedFrame: "IdleSelectedFrame",
        WalkN: "WalkNSelected",
        WalkS: "WalkSSelected",
        WalkE: "WalkESelected",
        WalkW: "WalkWSelected",
        WalkNE: "WalkNESelected",
        WalkNW: "WalkNWSelected",
        WalkSE: "WalkSESelected",
        WalkSW: "WalkSWSelected");
}