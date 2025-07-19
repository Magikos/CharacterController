using System;
using System.Linq;

namespace Rules.Core
{
    public class OrRule<TContext> : Rule<TContext>
    {
        private readonly IRule<TContext>[] _rules;
        public OrRule(params IRule<TContext>[] rules) => _rules = rules;
        public override bool Evaluate(TContext context) => _rules.Any(r => r.Evaluate(context));
    }
}
