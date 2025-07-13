public class CharacterStats
{
    public float MoveSpeed { get; set; } = 5f;
    public float SprintMultiplier { get; set; } = 1.5f;
    public float JumpHeight { get; set; } = 2f;
    public float Acceleration { get; set; } = 20f;
    public float Deceleration { get; set; } = 25f;

    public float CrouchSpeedMultiplier { get; set; } = 0.5f;
    public float RotationSpeed { get; set; } = 10f;
    public float JumpControlPower { get; set; } = 20f;

    // New physics parameters for better movement feel
    public float GroundDrag { get; set; } = 5f;
    public float AirDrag { get; set; } = 0.5f;
    public float GravityScale { get; set; } = 2.0f; // Increased for very snappy jumps and faster falling
    public float AirAccelerationMultiplier { get; set; } = 0.3f;
}
