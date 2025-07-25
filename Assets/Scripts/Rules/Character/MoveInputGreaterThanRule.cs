using Rules.Core;

public class MoveInputGreaterThanRule : ThresholdRule<CharacterContext, float>
{
    public MoveInputGreaterThanRule(float threshold) : base(ctx => ctx.Input.MoveInputMagnitude, threshold, (a, b) => a.CompareTo(b) > 0)
    {
    }
}
