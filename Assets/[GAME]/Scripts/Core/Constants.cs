using UnityEngine;

public static class Constants
{
    #region Animations
    public const string IDLE = "Idle";
    public const string SPEED = "Speed";
    public const string ATTACK = "Attack";
    public const string SWIM = "IsSwim";
    public const string AIM = "Aim";
    public const string JUMP = "IsJump";
    public const string GROUNDED = "IsGrounded";
    public const string CLIMB = "Climb";
    #endregion

    #region Font colors
    public const string GREEN_FONT_COLOR = "<color=green>";
    public const string RED_FONT_COLOR = "<color=red>";
    public const string WHITE_FONT_COLOR = "<color=white>";
    #endregion

    #region ColliderDirection
    public const int X_AXIS = 0; 
    public const int Y_AXIS = 1; 
    public const int Z_AXIS = 2;
    #endregion

    #region World layers
    public static float BACK_LAYER = 2;
    public static float MIDDLE_LAYER = 0;
    public static float FORWARD_LAYER = -2;
    #endregion
}