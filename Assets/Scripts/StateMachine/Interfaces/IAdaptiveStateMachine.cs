#nullable enable

using System;
using System.Collections.Generic;

public interface IAdaptiveStateMachine<TContext> : IDisposable
{
    IState<TContext>? CurrentState { get; }
    bool IsBlocking { get; }

    void FixedUpdate(TContext context);
    void ForceExit(TContext context);
    void LateUpdate(TContext context);
    void RegisterTransition(StateTransition<TContext> transition);
    void SetInitialState(TContext context, Type stateType);
    void Update(TContext context);
    AdaptiveStateMachine<TContext> WithInitialState(TContext context, Type stateType);
    AdaptiveStateMachine<TContext> WithStates(IEnumerable<IState<TContext>> states);
    AdaptiveStateMachine<TContext> WithStates(params IState<TContext>[] states);
    AdaptiveStateMachine<TContext> WithTransitions(IEnumerable<StateTransition<TContext>> transitions);
    AdaptiveStateMachine<TContext> WithTransitions(params StateTransition<TContext>[] transitions);
}
