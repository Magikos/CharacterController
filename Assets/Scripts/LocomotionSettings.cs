public static class LocomotionSettings
{
    public const float RotationSpeed = 10f; // Slerp rotation speed

    // Normalized input thresholds (0.0 - 1.0)
    public const float WalkInputThreshold = 0.1f;
    public const float RunInputThreshold = 0.5f;
    public const float SprintInputThreshold = 0.9f;

    // Actual movement speeds (meters per second)
    public const float WalkSpeed = 1.89f;
    public const float RunSpeed = 3.6f;
    public const float SprintSpeed = 4.9f;

    // Coyote time to allow jumping shortly after falling
    public const float GroundedGracePeriod = 0.2f;

    // Air control settings
    public const float JumpAirControl = 0.3f;      // Air control while jumping (additive steering, not replacement)
    public const float FallingAirControl = 0.2f;   // Air control while falling (less responsive)
    public const float GeneralAirControl = 0.25f;  // Default air control for other airborne states

    // Slope handling settings (ready for slope implementation)
    public const float MaxWalkableSlope = 45f;      // Maximum slope angle for walking (degrees)
    public const float MaxRunningSlope = 35f;       // Maximum slope angle for running (degrees)
    public const float SlopeSpeedMultiplier = 0.8f; // Speed reduction on slopes
    public const float SlopeForceStrength = 8f;     // How strongly slopes affect movement
}
