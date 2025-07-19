using Rules.Core;

public class IsJumpPressedRule : IRule<CharacterContext>
{
    public static readonly IsJumpPressedRule Instance = new IsJumpPressedRule();
    private IsJumpPressedRule() { }
    public bool Evaluate(CharacterContext ctx) => ctx.Input.IsJumpPressed;
    public static implicit operator System.Func<CharacterContext, bool>(IsJumpPressedRule rule) => rule.Evaluate;
}
