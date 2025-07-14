using UnityEngine;

public class RunState : GroundedBaseState
{
    private static readonly int RunAnimHash = Animator.StringToHash("Run");
    public override void Enter(CharacterContext context) => PlayAnimation(context.References.Animator, RunAnimHash, 0.1f);
    public override void FixedUpdate(CharacterContext context)
    {
        ApplyRotation(context, context.Input.MoveDirection);
        ApplyMovement(context, context.Input.MoveDirection, LocomotionSettings.RunSpeed);
    }
}
