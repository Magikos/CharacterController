using System;

public abstract class CompositeState<TContext> : BaseState<TContext>
{
    public override bool IsBlocking => StateMachine.IsBlocking;

    protected AdaptiveStateMachine<TContext> StateMachine { get; private set; } = null!;

    public override void Enter(TContext context) { }
    public override void Exit(TContext context) => StateMachine.ForceExit(context);
    public override void Update(TContext context) => StateMachine.Update(context);
    public override void FixedUpdate(TContext context) => StateMachine.FixedUpdate(context);
    public override void LateUpdate(TContext context) => StateMachine.LateUpdate(context);

    public CompositeState<TContext> WithStateMachine(AdaptiveStateMachine<TContext> stateMachine)
    {
        StateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        return this;
    }
}