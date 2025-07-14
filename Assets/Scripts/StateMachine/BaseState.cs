#nullable enable

using System;
using UnityEngine;

public abstract class BaseState<TContext> : IState<TContext>
{
    public virtual void Enter(TContext context) { }
    public virtual void Exit(TContext context) { }
    public virtual void Update(TContext context) { }
    public virtual void FixedUpdate(TContext context) { }
    public virtual void LateUpdate(TContext context) { }
    public virtual Type? EvaluateExit(TContext context) => null;
    public virtual bool IsBlocking => false;

    protected virtual void PlayAnimation(Animator animator, int animHash, float transitionDuration = 0.1f)
    {
        if (animator == null) return;
        animator.CrossFade(animHash, transitionDuration);
    }

    public virtual void Dispose()
    {
        // Clean up resources if necessary
    }
}
