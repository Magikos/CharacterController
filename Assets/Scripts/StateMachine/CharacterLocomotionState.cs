#nullable enable

using UnityEngine;

public abstract class CharacterLocomotionState : BaseState<CharacterContext>
{
    // -- MOVEMENT --
    protected virtual void ApplyMovement(CharacterContext context, Vector3 direction, float speed)
    {
        Vector3 move = direction * speed;
        context.Intent.DesiredVelocity = move;
    }

    // -- ROTATION --
    protected virtual void ApplyRotation(CharacterContext context, Vector3 direction)
    {
        if (direction == Vector3.zero) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        context.Intent.DesiredRotation = targetRotation;
    }
}
