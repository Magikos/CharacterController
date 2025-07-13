
using UnityEngine;

/// <summary>
/// Subcontext that holds interpreted intent flags.
/// </summary>
public class CharacterIntentContext : ICharacterIntentContext
{
    public Vector3 DesiredVelocity { get; set; }
    public Quaternion DesiredRotation { get; set; }
    public bool WantsToSprint { get; set; }
    public bool WantsToCrouch { get; set; }

    // New: Granular physics control flags
    public bool OverrideGravity { get; set; }
    public bool OverrideYVelocity { get; set; }
    public bool OverrideHorizontalPhysics { get; set; }

    public void Initialize(GameObject owner)
    {
        // Initialize any references or state here
    }

    /// <summary>
    /// Reset all frame-based intent data to neutral/default state.
    /// Called each frame before state updates to provide a clean slate.
    /// Only resets intent/commands, not persistent state data.
    /// </summary>
    public void ResetFrameContext()
    {
        // Commands/Actions (reset to neutral - no actions wanted)
        WantsToSprint = false;
        WantsToCrouch = false;

        // Physics overrides (reset to default motor behavior)
        OverrideGravity = false;
        OverrideYVelocity = false;
        OverrideHorizontalPhysics = false;

        // Movement intent (reset to neutral - no movement desired)
        DesiredVelocity = Vector3.zero;
        // Note: DesiredRotation is left as-is since it should maintain current rotation
        // unless a state explicitly wants to change it
    }

#if UNITY_EDITOR && DEBUG
    /// <summary>
    /// Debug validation to ensure states are properly setting intent each frame.
    /// Only active in debug builds to avoid performance impact.
    /// </summary>
    public void ValidateIntentWasSet(string stateName = "Unknown")
    {
        // Some states like Idle legitimately don't set intent, so we check for reasonable combinations
        bool hasMovementIntent = DesiredVelocity != Vector3.zero;
        bool hasPhysicsOverrides = OverrideGravity || OverrideYVelocity || OverrideHorizontalPhysics;
        bool hasActionIntent = WantsToSprint || WantsToCrouch;

        // Only warn if state seems completely inactive (might be intentional for states like Idle)
        if (!hasMovementIntent && !hasPhysicsOverrides && !hasActionIntent)
        {
            // Don't warn for states that are expected to be passive
            if (!stateName.Contains("Idle") && !stateName.Contains("Disabled") && !stateName.Contains("Waiting"))
            {
                Logwin.Log("IntentValidation", $"State '{stateName}' may not be setting intent properly - no movement, overrides, or actions detected");
            }
        }
    }
#endif
}
