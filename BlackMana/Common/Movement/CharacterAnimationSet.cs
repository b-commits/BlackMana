namespace BlackMana.Common.Movement;

internal sealed record CharacterAnimationSet(
    string IdleFrame,
    string IdleSelectedFrame,
    string WalkN,
    string WalkS,
    string WalkE,
    string WalkW,
    string WalkNE,
    string WalkNW,
    string WalkSE,
    string WalkSW);