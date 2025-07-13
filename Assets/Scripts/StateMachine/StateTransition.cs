#nullable enable

using System;

public class StateTransition<TContext>
{
    public Type? From; // null = Any
    public Func<TContext, bool> Condition = _ => false; // Transition never activates unless set
    public Func<Type?, TContext, Type?> ResolveTo = (_, _) => null!; // Explicit null transition (null forgiving)
}
