using UnityEngine;
/// <summary>
/// Interface for motion context - pure motion and physics data.
/// </summary>
public interface ICharacterMotorContext
{
    // Core Motion
    Vector3 CurrentVelocity { get; set; }
    Vector3 ExternalForces { get; set; }

    // Fall Tracking
    float FallStartHeight { get; set; }

    // Physics Settings
    float Gravity { get; set; }

    // Derived Properties
    float FallDistance { get; }
    bool IsFalling { get; }
    bool IsMoving { get; }

    void Initialize(GameObject owner);
    void ResetFrameContext();
}
