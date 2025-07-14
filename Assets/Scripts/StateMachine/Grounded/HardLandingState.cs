using UnityEngine;

public class HardLandingState : GroundedBaseState
{
    private static readonly int HardLandingAnimHash = Animator.StringToHash("HardLanding");
    public override void Enter(CharacterContext context) => PlayAnimation(context.References.Animator, HardLandingAnimHash, 0.1f);
}