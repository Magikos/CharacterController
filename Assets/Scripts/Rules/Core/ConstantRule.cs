using System;

namespace Rules.Core
{
    public abstract class ConstantRule<TContext> : Rule<TContext>
    {
        private readonly bool _value;
        public ConstantRule(bool value) => _value = value;
        public override bool Evaluate(TContext context) => _value;
    }

    public class TrueRule<TContext> : ConstantRule<TContext>
    {
        public TrueRule() : base(true) { }
    }

    public class FalseRule<TContext> : ConstantRule<TContext>
    {
        public FalseRule() : base(false) { }
    }
}
