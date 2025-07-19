using System;

namespace Rules.Core
{
    public abstract class Rule<TContext> : IRule<TContext>
    {
        public abstract bool Evaluate(TContext context);
        public static implicit operator Func<TContext, bool>(Rule<TContext> rule) => rule.Evaluate;
    }
}
