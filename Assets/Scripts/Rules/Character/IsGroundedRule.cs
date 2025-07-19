using Rules.Core;

public class IsGroundedRule : IRule<CharacterContext>
{
    public static readonly IsGroundedRule Instance = new IsGroundedRule();
    private IsGroundedRule() { }
    public bool Evaluate(CharacterContext ctx) => ctx.Sensor.IsGrounded;
    public static implicit operator System.Func<CharacterContext, bool>(IsGroundedRule rule) => rule.Evaluate;
}
