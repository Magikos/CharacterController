using UnityEngine;

#if UNITY_EDITOR
using MiniTools.BetterGizmos;
#endif

public class FallingState : AirborneBaseState
{
    private static readonly int FallAnim = Animator.StringToHash("Fall");
    private const float TerminalVelocity = -15f; // Maximum falling speed

    public override void Enter(CharacterContext context)
    {
        PlayAnimation(context.Animator, FallAnim);
        AirControlFactor = LocomotionSettings.FallingAirControl; // Set air control for falling

        // Track fall start if we're just starting to fall
        if (context.Motor.CurrentVelocity.y >= 0)
        {
            context.Motor.FallStartHeight = context.Transform.position.y;
        }

        Logwin.Log("[FallingState]", $"Started falling from height: {context.Motor.FallStartHeight}");
    }

    public override void FixedUpdate(CharacterContext context)
    {
        Vector3 velocity = context.Motor.CurrentVelocity;

        // We don't override gravity - let the motor apply it naturally
        // We also don't override Y velocity - let gravity/motor handle it completely

        // Handle horizontal movement with limited air control
        Vector3 horizontalMove = context.Input.MoveDirection * context.Stats.MoveSpeed * AirControlFactor;

        // Set desired velocity for horizontal movement only
        // Y component doesn't matter since OverrideYVelocity = false, motor ignores DesiredVelocity.y
        context.Intent.DesiredVelocity = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);

        // Apply rotation if moving
        if (context.Input.MoveDirection != Vector3.zero)
        {
            ApplyRotation(context, context.Input.MoveDirection);
        }

        // Update fall distance calculation for logging
        float currentHeight = context.Transform.position.y;
        float fallDistance = context.Motor.FallStartHeight - currentHeight;

        Logwin.Log("[FallingState]", $"Falling - Velocity: {velocity.y:F2}, Fall Distance: {fallDistance:F1}", LogwinParam.Color(Color.red));
    }

    public override System.Type EvaluateExit(CharacterContext context)
    {
        // FallingState should only stay in falling or exit the airborne system entirely
        // The top-level Airborne -> Grounded transition will handle the switch
        // Grounded.Enter() will decide which specific landing state to use
        return null;
    }

    public override void Exit(CharacterContext context)
    {
        float fallDistance = context.Motor.FallStartHeight - context.Transform.position.y;
        Logwin.Log("[FallingState]", $"Finished falling - Distance: {fallDistance:F1}, Final velocity: {context.Motor.CurrentVelocity.y:F2}");
    }
}
