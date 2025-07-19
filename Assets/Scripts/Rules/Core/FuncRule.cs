using System;

namespace Rules.Core
{
    public class FuncRule<TContext> : Rule<TContext>
    {
        private readonly Func<TContext, bool> _func;
        public FuncRule(Func<TContext, bool> func) => _func = func;
        public override bool Evaluate(TContext context) => _func(context);
    }
}
