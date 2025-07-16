#nullable enable

using System;

public interface IContainsStates<in TContext>
{
    bool ContainsState(Type? toType);
    void SwitchState(TContext context, Type? toType, bool force = false);
}
