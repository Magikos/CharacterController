namespace Rules.Core
{
    public interface IRule<TContext>
    {
        bool Evaluate(TContext context);
    }
}
