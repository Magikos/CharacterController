using Rules.Core;

public class IsSprintPressedRule : Rule<CharacterContext>
{
    public static readonly IsSprintPressedRule Instance = new IsSprintPressedRule();
    private IsSprintPressedRule() { }
    public override bool Evaluate(CharacterContext ctx) => ctx.Input.IsSprintPressed;
}
