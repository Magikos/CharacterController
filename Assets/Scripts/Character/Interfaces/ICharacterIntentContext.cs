
using UnityEngine;

/// <summary>
/// Interface for intent context.
/// </summary>
public interface ICharacterIntentContext
{
    Vector3 DesiredVelocity { get; set; }
    Quaternion DesiredRotation { get; set; }

    bool WantsToSprint { get; set; }
    bool WantsToCrouch { get; set; }

    // New: Granular physics control flags
    bool OverrideGravity { get; set; }      // State controls gravity application
    bool OverrideYVelocity { get; set; }    // State controls Y velocity directly
    bool OverrideHorizontalPhysics { get; set; } // State controls X/Z physics (for special movement)

    void Initialize(GameObject owner);
    void ResetFrameContext();
}
