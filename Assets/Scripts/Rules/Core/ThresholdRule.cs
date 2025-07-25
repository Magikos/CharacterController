using System;

namespace Rules.Core
{
    public class ThresholdRule<TContext, TValue> : Rule<TContext>
        where TValue : IComparable<TValue>
    {
        private readonly Func<TContext, TValue> _selector;
        private readonly TValue _threshold;
        private readonly Func<TValue, TValue, bool> _comparison;

        public ThresholdRule(Func<TContext, TValue> selector, TValue threshold, Func<TValue, TValue, bool> comparison)
        {
            _selector = selector;
            _threshold = threshold;
            _comparison = comparison;
        }

        public override bool Evaluate(TContext context) => _comparison(_selector(context), _threshold);
    }
}
