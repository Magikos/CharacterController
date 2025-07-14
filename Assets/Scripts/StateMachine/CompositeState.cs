#nullable enable

using System;

public abstract class CompositeState<TContext> : BaseState<TContext>, IContainsStates<TContext>
{
    public override bool IsBlocking => StateMachine.IsBlocking;

    protected AdaptiveStateMachine<TContext> StateMachine { get; private set; } = null!;

    public override void Enter(TContext context) { }
    public override void Exit(TContext context) => StateMachine.ForceExit(context);
    public override void Update(TContext context) => StateMachine.Update(context);
    public override void FixedUpdate(TContext context) => StateMachine.FixedUpdate(context);
    public override void LateUpdate(TContext context) => StateMachine.LateUpdate(context);

    public override Type? EvaluateExit(TContext context)
    {
        var subExit = StateMachine.EvaluateExit();
        if (subExit != null) { Logwin.Log("[Composite]", $"Bubbling exit from sub: {subExit.Name}"); }

        return subExit ?? base.EvaluateExit(context);
    }

    public CompositeState<TContext> WithStateMachine(AdaptiveStateMachine<TContext> stateMachine)
    {
        StateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        return this;
    }

    public bool ContainsState(Type? toType) => StateMachine.ContainsState(toType);
    public void SwitchState(TContext context, Type? toType, bool force = false)
    {
        if (StateMachine == null) throw new InvalidOperationException("StateMachine is not initialized.");
        StateMachine.SwitchState(context, toType, force);
    }

    public override void Dispose()
    {
        base.Dispose();
        StateMachine.Dispose();
        StateMachine = null!;
    }

}
