using System;
using System.Collections.Generic;
using UnityEngine;

public static class GroundedTransitionBuilder
{
    public static IEnumerable<StateTransition<CharacterContext>> BuildDefaultTransitions()
    {
        // Sliding transitions (highest priority - environmental reactions)
        yield return new StateTransition<CharacterContext>
        {
            From = typeof(WalkState),
            Condition = ctx => ctx.Sensor.ShouldSlide,
            ResolveTo = (_, _) => typeof(SlidingState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(RunState),
            Condition = ctx => ctx.Sensor.ShouldSlide,
            ResolveTo = (_, _) => typeof(SlidingState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SprintState),
            Condition = ctx => ctx.Sensor.ShouldSlide,
            ResolveTo = (_, _) => typeof(SlidingState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SlidingState),
            Condition = ctx => !ctx.Sensor.ShouldSlide && ctx.Input.MoveInputMagnitude > LocomotionSettings.WalkInputThreshold,
            ResolveTo = (_, _) => typeof(WalkState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SlidingState),
            Condition = ctx => !ctx.Sensor.ShouldSlide && ctx.Input.MoveInputMagnitude <= LocomotionSettings.WalkInputThreshold,
            ResolveTo = (_, _) => typeof(IdleState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(IdleState),
            Condition = ctx => ctx.Input.MoveInputMagnitude > LocomotionSettings.WalkInputThreshold,
            ResolveTo = (_, _) => typeof(WalkState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(WalkState),
            Condition = ctx => ctx.Input.MoveInputMagnitude < LocomotionSettings.WalkInputThreshold,
            ResolveTo = (_, _) => typeof(IdleState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(WalkState),
            Condition = ctx => ctx.Input.MoveInputMagnitude >= LocomotionSettings.RunInputThreshold,
            ResolveTo = (_, _) => typeof(RunState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(RunState),
            Condition = ctx => ctx.Input.MoveInputMagnitude < LocomotionSettings.RunInputThreshold,
            ResolveTo = (_, _) => typeof(WalkState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(RunState),
            Condition = ctx => ctx.Input.IsSprintPressed && ctx.Vitals.CanSprint,
            ResolveTo = (_, _) => typeof(SprintState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SprintState),
            Condition = ctx => !ctx.Input.IsSprintPressed || !ctx.Vitals.CanSprint,
            ResolveTo = (_, _) => typeof(RunState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = null, // Any grounded state
            Condition = ctx => ctx.Input.IsJumpPressed && ctx.Sensor.IsGrounded && ctx.Vitals.CanJump,
            ResolveTo = (_, _) => typeof(JumpState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SoftLandingState),
            Condition = ctx => true, // Always transition from LandedState
            ResolveTo = (from, ctx) => PickMovementState(ctx)
        };

    }

    public static Type PickMovementState(ICharacterContext ctx)
    {
        float magnitude = Mathf.Max(ctx.Input.MoveInputMagnitude, ctx.Motor.CurrentVelocity.magnitude);

        if (ctx.Motor.FallDistance >= 5f)
            return typeof(HardLandingState);
        if (ctx.Motor.FallDistance >= 2.5f)
            return typeof(SoftLandingState);

        if (ctx.Input.IsSprintPressed && magnitude >= LocomotionSettings.SprintInputThreshold)
            return typeof(SprintState);
        if (magnitude >= LocomotionSettings.RunInputThreshold)
            return typeof(RunState);
        if (magnitude >= LocomotionSettings.WalkInputThreshold)
            return typeof(WalkState);
        if (magnitude >= 0)
            return typeof(IdleState);

        return typeof(IdleState);

    }

}
