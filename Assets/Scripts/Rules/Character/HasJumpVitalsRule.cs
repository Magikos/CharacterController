using Rules.Core;

public class HasJumpVitalsRule : Rule<CharacterContext>
{
    public static readonly HasJumpVitalsRule Instance = new HasJumpVitalsRule();
    private HasJumpVitalsRule() { }
    public override bool Evaluate(CharacterContext ctx) => ctx.Vitals.CanJump;
}
