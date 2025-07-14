using UnityEngine;

public class SprintState : GroundedBaseState
{
    private static readonly int SprintAnimHash = Animator.StringToHash("Sprint");
    public override void Enter(CharacterContext context) => PlayAnimation(context.References.Animator, SprintAnimHash, 0.1f);
    public override void FixedUpdate(CharacterContext context)
    {
        ApplyRotation(context, context.Input.MoveDirection);
        ApplyMovement(context, context.Input.MoveDirection, LocomotionSettings.SprintSpeed);
        context.Vitals.DrainStamina(context.FixedDeltaTime * 10f);
    }
}
