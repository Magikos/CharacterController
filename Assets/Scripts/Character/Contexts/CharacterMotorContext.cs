using UnityEngine;
/// <summary>
/// Subcontext for real-time movement state - pure motion and physics.
/// </summary>
public class CharacterMotorContext : ICharacterMotorContext
{
    // Core Motion
    public Vector3 CurrentVelocity { get; set; }
    public Vector3 ExternalForces { get; set; }

    // Fall Tracking
    public float FallStartHeight { get; set; }

    // Physics Settings
    public float Gravity { get; set; } = -9.81f; // Default gravity value  

    // --- Derived Helpers ---
    public float FallDistance { get; private set; } // Calculated based on sensor data
    public bool IsFalling => CurrentVelocity.y <= 0 && FallDistance > 0.1f;
    public bool IsMoving => CurrentVelocity.magnitude > 0.1f;

    public void Initialize(GameObject owner)
    {
        // Initialize any references or state here
    }

    public void ResetFrameContext()
    {
        // Reset frame-specific state
    }

    // Helper method to update fall distance (called by sensor system)
    public void UpdateFallDistance(float currentHeight, float lastGroundedHeight)
    {
        FallDistance = currentHeight - lastGroundedHeight;
    }
}