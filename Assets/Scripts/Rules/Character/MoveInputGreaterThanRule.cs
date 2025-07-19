using Rules.Core;

public class MoveInputGreaterThanRule : IRule<CharacterContext>
{
    private readonly float _threshold;
    public MoveInputGreaterThanRule(float threshold) => _threshold = threshold;
    public bool Evaluate(CharacterContext ctx) => ctx.Input.MoveInputMagnitude > _threshold;
    public static implicit operator System.Func<CharacterContext, bool>(MoveInputGreaterThanRule rule) => rule.Evaluate;
}
