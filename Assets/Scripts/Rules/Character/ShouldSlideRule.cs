using Rules.Core;

public class ShouldSlideRule : IRule<CharacterContext>
{
    public static readonly ShouldSlideRule Instance = new ShouldSlideRule();
    private ShouldSlideRule() { }
    public bool Evaluate(CharacterContext ctx) => ctx.Sensor.ShouldSlide;
    public static implicit operator System.Func<CharacterContext, bool>(ShouldSlideRule rule) => rule.Evaluate;
}
