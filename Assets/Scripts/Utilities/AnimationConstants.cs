using UnityEngine;

namespace GameDevTV.RTS.Utilities
{

public static class AnimationConstants
{
    // static readonly. these can't be const, because they're populated at runtime.
    public static readonly int SPEED = Animator.StringToHash("Speed");
    public static readonly int IS_GATHERING = Animator.StringToHash("IsGathering");
}
}