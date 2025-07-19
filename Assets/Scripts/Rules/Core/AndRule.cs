using System;
using System.Linq;

namespace Rules.Core
{
    public class AndRule<TContext> : Rule<TContext>
    {
        private readonly IRule<TContext>[] _rules;
        public AndRule(params IRule<TContext>[] rules) => _rules = rules;
        public override bool Evaluate(TContext context) => _rules.All(r => r.Evaluate(context));
    }
}
