using Rules.Core;

public class HasStaminaRule : ThresholdRule<CharacterContext, float>
{
    public HasStaminaRule(float requiredStamina)
        : base(ctx => ctx.Vitals.Stamina, requiredStamina, (current, required) => current >= required)
    {
    }
}
