#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdaptiveStateMachine<TContext> : IAdaptiveStateMachine<TContext>
{
    private readonly Dictionary<Type, List<StateTransition<TContext>>> _transitionsByState = new();
    private readonly List<StateTransition<TContext>> _any = new();
    private static readonly List<StateTransition<TContext>> EmptyTransitions = new();

    private List<StateTransition<TContext>> _currentTransitions = EmptyTransitions;
    private IState<TContext>? _currentState;
    public IState<TContext>? CurrentState => _currentState;
    private Dictionary<Type, IState<TContext>> _stateCache = new();

    public bool IsBlocking => _currentState?.IsBlocking ?? false;

    public AdaptiveStateMachine<TContext> WithStates(IEnumerable<IState<TContext>> states) => WithStates(states.ToArray());
    public AdaptiveStateMachine<TContext> WithStates(params IState<TContext>[] states)
    {
        _stateCache = states.ToDictionary(s => s.GetType(), s => s);
        return this;
    }

    public AdaptiveStateMachine<TContext> WithTransitions(IEnumerable<StateTransition<TContext>> transitions) => WithTransitions(transitions.ToArray());
    public AdaptiveStateMachine<TContext> WithTransitions(params StateTransition<TContext>[] transitions)
    {
        foreach (var transition in transitions) { RegisterTransition(transition); }
        return this;
    }

    public AdaptiveStateMachine<TContext> WithInitialState(TContext context, Type stateType)
    {
        SetInitialState(context, stateType);
        return this;
    }


    public void RegisterTransition(StateTransition<TContext> transition)
    {
        if (transition.From == null) _any.Add(transition);
        else
        {
            if (!_transitionsByState.TryGetValue(transition.From, out var list))
                _transitionsByState[transition.From] = list = new();
            list.Add(transition);
        }
    }

    public void SetInitialState(TContext context, Type stateType) => SwitchState(context, stateType, true);

    public void Update(TContext context)
    {
        if (IsBlocking) return;

        foreach (var t in _any)
        {
            if (t.Condition(context))
            {
                SwitchState(context, t.ResolveTo(_currentState?.GetType(), context));
                return;
            }
        }

        foreach (var t in _currentTransitions)
        {
            if (t.Condition(context))
            {
                SwitchState(context, t.ResolveTo(_currentState?.GetType(), context));
                return;
            }
        }

        var next = _currentState?.EvaluateExit(context);
        if (next != null) SwitchState(context, next);
        else _currentState?.Update(context);
    }

    public void FixedUpdate(TContext context) => _currentState?.FixedUpdate(context);
    public void LateUpdate(TContext context) => _currentState?.LateUpdate(context);

    private void SwitchState(TContext context, Type? toType, bool force = false)
    {
        if (toType == null || !_stateCache.TryGetValue(toType, out var next)) return;
        if (_currentState == next && !force) return;

        Logwin.Log("[FSM]", $"Switching from {_currentState?.GetType().Name ?? "None"} to {next.GetType().Name}");

        _currentState?.Exit(context);
        _currentState = next;
        _currentTransitions = _transitionsByState.TryGetValue(_currentState.GetType(), out var list) ? list : EmptyTransitions;
        _currentState.Enter(context);
    }

    public void ForceExit(TContext context)
    {
        _currentState?.Exit(context);
        _currentState = null;
    }
}
