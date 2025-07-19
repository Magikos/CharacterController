namespace Rules.Core
{

    public class NotRule<TContext> : Rule<TContext>
    {
        private readonly IRule<TContext> _rule;
        public NotRule(IRule<TContext> rule) => _rule = rule;
        public override bool Evaluate(TContext context) => !_rule.Evaluate(context);
    }
}
