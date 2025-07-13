using UnityEngine;

/// <summary>
/// Interface for sensor context - handles environmental detection data.
/// </summary>
public interface ICharacterSensorContext
{
    // Ground Detection
    bool IsGrounded { get; set; }
    float TimeSinceLastGrounded { get; set; }
    float LastGroundedTime { get; set; }
    float LastGroundedHeight { get; set; }

    // Surface Information
    Vector3 GroundNormal { get; set; }
    Vector3 GroundContactPoint { get; set; }
    Vector3 DesiredGroundPosition { get; set; }

    // Slope Information
    float SlopeAngle { get; }
    bool IsOnSlope { get; }

    // Wall Detection (future)
    bool IsTouchingWall { get; set; }
    Vector3 WallNormal { get; set; }

    // Ceiling Detection (future)
    bool HasCeilingAbove { get; set; }
    float CeilingDistance { get; set; }

    // Ledge Detection (future)
    bool HasLedgeAhead { get; set; }
    Vector3 LedgePosition { get; set; }

    // Step Detection
    bool HasStepAhead { get; set; }
    float StepHeight { get; set; }
    Vector3 StepPosition { get; set; }
    bool HasObstacleAhead { get; set; }

    // Slope Movement Analysis
    bool ShouldSlide { get; }
    bool CanWalkOnSlope { get; }
    bool CanRunOnSlope { get; }
    Vector3 SlopeDirection { get; }

    void Initialize(GameObject owner);
    void ResetFrameContext();
}
