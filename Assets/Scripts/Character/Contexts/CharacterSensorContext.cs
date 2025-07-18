using UnityEngine;

/// <summary>
/// Concrete implementation of sensor context.
/// </summary>
public class CharacterSensorContext
{
    // Core Sensor Data
    public float BaseSensorRange { get; set; } = 0.1f; // Default sensor range for ground detection
    public float BaseCastLength { get; set; } = 0.1f; // Default cast length for ground detection
    public int ExcludeLayers { get; set; } = LayerMask.GetMask("UI", "Ignore Raycast"); // Default layers to ignore in sensor checks

    // Ground Detection
    public bool IsGrounded { get; set; }
    public float TimeSinceLastGrounded { get; set; }
    public float LastGroundedTime { get; set; }
    public float LastGroundedHeight { get; set; }

    // Surface Information
    public Vector3 FeetPosition { get; set; }
    public float GroundDistance { get; set; }
    public Vector3 GroundNormal { get; set; } = Vector3.up;
    public Vector3 GroundContactPoint { get; set; }
    public Vector3 GroundPosition { get; set; }

    // Slope Information (calculated properties)
    public float SlopeAngle => Vector3.Angle(Vector3.up, GroundNormal);
    public bool IsOnSlope => SlopeAngle > 1f; // More than 1 degree is considered a slope

    // Wall Detection (future)
    public bool IsTouchingWall { get; set; }
    public Vector3 WallNormal { get; set; }

    // Ceiling Detection (future)
    public bool HasCeilingAbove { get; set; }
    public float CeilingDistance { get; set; }

    // Ledge Detection (future)
    public bool HasLedgeAhead { get; set; }
    public Vector3 LedgePosition { get; set; }

    // Step Detection
    public bool HasStepAhead { get; set; }
    public float StepHeight { get; set; }
    public Vector3 StepPosition { get; set; }
    public bool HasObstacleAhead { get; set; }

    // Slope Movement Analysis (calculated properties)
    public bool ShouldSlide => SlopeAngle >= 60f; // Steep slopes trigger sliding
    public bool CanWalkOnSlope => SlopeAngle <= 45f; // Walkable slope threshold
    public bool CanRunOnSlope => SlopeAngle <= 30f; // Running slope threshold
    public Vector3 SlopeDirection => Vector3.ProjectOnPlane(Vector3.down, GroundNormal).normalized;

    public Vector3 GroundAdjustmentVelocity { get; set; } = Vector3.zero; // Used for adjusting position on ground
    public Vector3 CastOrigin { get; set; }

    public bool IsColliderOverlapping { get; set; }

    public void Initialize(GameObject owner)
    {
        // Initialize any references or state here
        // This could include setting up colliders, raycasts, etc.
    }

    public void Dispose()
    {
        // Clean up any resources if necessary
        // This could include removing event listeners, nullifying references, etc.
    }

    public void ResetFrameContext()
    {
        // Reset frame-specific data
        // Note: Don't reset persistent data like IsGrounded, GroundNormal, etc.
    }
}
