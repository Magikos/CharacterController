using UnityEngine;

public class WalkState : GroundedBaseState
{
    private static readonly int WalkAnimHash = Animator.StringToHash("Walk");

    public override void Enter(CharacterContext context) => PlayAnimation(context.Animator, WalkAnimHash, 0.1f);

    public override void FixedUpdate(CharacterContext context)
    {
        ApplyRotation(context, context.Input.MoveDirection);
        ApplyMovement(context, context.Input.MoveDirection, LocomotionSettings.WalkSpeed);
    }
}
