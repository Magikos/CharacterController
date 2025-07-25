using UnityEngine;

public class SoftLandingState : GroundedBaseState
{
    private static readonly int LandedAnimHash = Animator.StringToHash("SoftLanding");
    public override void Enter(CharacterContext context) => PlayAnimation(context.References.Animator, LandedAnimHash, 0.1f);

}
