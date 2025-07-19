using Rules.Core;

public class CanJumpRule : AndRule<CharacterContext>
{
    public CanJumpRule() : base(
        HasJumpVitalsRule.Instance,
        IsGroundedRule.Instance,
        IsJumpPressedRule.Instance
    )
    { }
}
