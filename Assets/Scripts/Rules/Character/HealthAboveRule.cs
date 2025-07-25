using Rules.Core;

public class HealthAboveRule : ThresholdRule<CharacterContext, float>
{
    public HealthAboveRule(float minHealth)
        : base(ctx => ctx.Vitals.Health, minHealth, (current, min) => current > min)
    {
    }
}
