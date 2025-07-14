#nullable enable

using System;

public interface IState<TContext> : IDisposable
{
    void Enter(TContext context);
    void Exit(TContext context);
    void Update(TContext context);
    void FixedUpdate(TContext context);
    void LateUpdate(TContext context);
    Type? EvaluateExit(TContext context); // Formerly GetNextState
    bool IsBlocking { get; } // Optional exit prevention
}
