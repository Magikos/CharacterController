#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class AdaptiveStateMachine<TContext> : IAdaptiveStateMachine<TContext>, IContainsStates<TContext>
{
    public bool IsBlocking => _currentState?.IsBlocking ?? false;
    public float BlockTimeout { get; set; } = 5.0f; // e.g., 5 seconds

    private readonly Dictionary<Type, List<StateTransition<TContext>>> _transitionsByState = new();
    private readonly List<StateTransition<TContext>> _any = new();
    private static readonly List<StateTransition<TContext>> EmptyTransitions = new();

    private List<StateTransition<TContext>> _currentTransitions = EmptyTransitions;
    private IState<TContext>? _currentState;
    public IState<TContext>? CurrentState => _currentState;
    private Dictionary<Type, IState<TContext>> _stateCache = new();
    private Type? _evaluateExitType = null;
    private float _blockTimer = 0f;

    private bool _defaultOnNullState = false;
    private Type? _defaultIfNullType = null;

    public AdaptiveStateMachine<TContext> WithNullDefault(IState<TContext> defaultState)
    {
        var type = defaultState.GetType();
        _defaultOnNullState = true;
        _defaultIfNullType = type;
        _stateCache[type] = defaultState;
        return this;
    }

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
        if (_currentState == null && _defaultOnNullState && _defaultIfNullType != null)
        {
            SwitchState(context, _defaultIfNullType, true);
            return;
        }

        if (IsStateBlocking(context)) return;

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

        if (IsExiting(context)) return;

        _currentState?.Update(context);
    }

    public void FixedUpdate(TContext context) => _currentState?.FixedUpdate(context);
    public void LateUpdate(TContext context) => _currentState?.LateUpdate(context);


    private bool IsStateBlocking(TContext context)
    {
        if (IsBlocking)
        {
            if (_blockTimer > BlockTimeout)
            {
                Logwin.LogWarning("[FSM]", $"Blocking timeout; forcing unblock of state: {_currentState?.GetType().Name ?? "Error"}");
                return false; // Force unblock after timeout
            }
            _blockTimer += Time.deltaTime;
        }

        return IsBlocking;
    }


    public Type? EvaluateExit() => _evaluateExitType;
    private bool IsExiting(TContext context)
    {
        _evaluateExitType = _currentState?.EvaluateExit(context);
        if (_evaluateExitType == null) return false;

        if (_stateCache.ContainsKey(_evaluateExitType))
        {
            SwitchState(context, _evaluateExitType, true);
            _evaluateExitType = null; // Reset exit type after switching
            return true;
        }

        foreach (var state in _stateCache.Values)
        {
            if (state is IContainsStates<TContext> containsStates && containsStates.ContainsState(_evaluateExitType))
            {
                SwitchState(context, state.GetType(), true);  //set the parent first, then the child.
                containsStates.SwitchState(context, _evaluateExitType, true);
                _evaluateExitType = null;
                return true;
            }
        }

        ForceExit(context);
        return true; // Force exit if no valid state found
    }

    public void SwitchState(TContext context, Type? toType, bool force = false)
    {
        if (toType == null) return;
        if (!_stateCache.TryGetValue(toType, out var next))
        {
            Debug.LogError($"[FSM] : Invalid next state type: {toType.Name}");
            return;
        }

        if (_currentState == next && !force)
        {
            _currentState.ReEnter(context);
            return;
        }

        Logwin.Log("[FSM]", $"Switching from {_currentState?.GetType().Name ?? "None"} to {next.GetType().Name}");

        _currentState?.Exit(context);
        _currentState = next;
        _currentTransitions = _transitionsByState.TryGetValue(_currentState.GetType(), out var list) ? list : EmptyTransitions;
        _currentState.Enter(context);
        _blockTimer = 0f; // Reset block timer on state switch
    }

    public bool ContainsState(Type? toType)
    {
        if (toType == null) return false;

        var contains = _stateCache.ContainsKey(toType);
        if (!contains)
        {
            foreach (var state in _stateCache.Values)
            {
                if (state is IContainsStates<TContext> containsStates && containsStates.ContainsState(toType))
                {
                    contains = true;
                    break;
                }
            }
        }

        return contains;
    }

    public void ForceExit(TContext context)
    {
        _currentState?.Exit(context);
        _currentState = null;
    }

    public void Dispose()
    {
        // Clean up resources if necessary
        foreach (var state in _stateCache.Values) { state.Dispose(); }
        _stateCache.Clear();
    }
}
