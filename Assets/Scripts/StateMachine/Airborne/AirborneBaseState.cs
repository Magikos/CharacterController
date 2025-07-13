public abstract class AirborneBaseState : CharacterLocomotionState
{
    protected float AirControlFactor = 0.5f;

    protected virtual void ApplyAirControl(CharacterContext context)
    {
        // Optional air control logic can go here
        ApplyMovement(context, context.Intent.DesiredVelocity, context.Stats.MoveSpeed * AirControlFactor);
    }
}
