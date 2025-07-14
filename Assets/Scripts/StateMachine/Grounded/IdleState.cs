using UnityEngine;

public class IdleState : GroundedBaseState
{
    private static readonly int IdleAnimHash = Animator.StringToHash("Idle");

    public override void Enter(CharacterContext context) => PlayAnimation(context.References.Animator, IdleAnimHash, 0.1f);

    public override void FixedUpdate(CharacterContext context)
    {
        ApplyRotation(context, context.Input.MoveDirection);
        ApplyMovement(context, Vector3.zero, 0f); // No movement in idle
    }
}
