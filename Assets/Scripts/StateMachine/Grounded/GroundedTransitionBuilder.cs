using System;
using System.Collections.Generic;
using Rules.Core;
using UnityEngine;

public static class GroundedTransitionBuilder
{
    public static IEnumerable<StateTransition<CharacterContext>> BuildDefaultTransitions()
    {
        // Sliding transitions (highest priority - environmental reactions)

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(WalkState),
            Condition = ShouldSlideRule.Instance,
            ResolveTo = (_, _) => typeof(SlidingState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(RunState),
            Condition = ShouldSlideRule.Instance,
            ResolveTo = (_, _) => typeof(SlidingState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SprintState),
            Condition = ShouldSlideRule.Instance,
            ResolveTo = (_, _) => typeof(SlidingState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SlidingState),
            Condition = new AndRule<CharacterContext>(
                new NotRule<CharacterContext>(ShouldSlideRule.Instance),
                new MoveInputGreaterThanRule(LocomotionSettings.WalkInputThreshold)
            ),
            ResolveTo = (_, _) => typeof(WalkState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SlidingState),
            Condition = new AndRule<CharacterContext>(
                new NotRule<CharacterContext>(ShouldSlideRule.Instance),
                new NotRule<CharacterContext>(new MoveInputGreaterThanRule(LocomotionSettings.WalkInputThreshold))
            ),
            ResolveTo = (_, _) => typeof(IdleState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(IdleState),
            Condition = new MoveInputGreaterThanRule(LocomotionSettings.WalkInputThreshold),
            ResolveTo = (_, _) => typeof(WalkState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(WalkState),
            Condition = new NotRule<CharacterContext>(new MoveInputGreaterThanRule(LocomotionSettings.WalkInputThreshold)),
            ResolveTo = (_, _) => typeof(IdleState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(WalkState),
            Condition = new MoveInputGreaterThanRule(LocomotionSettings.RunInputThreshold),
            ResolveTo = (_, _) => typeof(RunState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(RunState),
            Condition = new NotRule<CharacterContext>(new MoveInputGreaterThanRule(LocomotionSettings.RunInputThreshold)),
            ResolveTo = (_, _) => typeof(WalkState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(RunState),
            Condition = new AndRule<CharacterContext>(
                IsSprintPressedRule.Instance,
                HasJumpVitalsRule.Instance // Assuming CanSprint is similar to CanJump
            ),
            ResolveTo = (_, _) => typeof(SprintState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SprintState),
            Condition = new OrRule<CharacterContext>(
                new NotRule<CharacterContext>(IsSprintPressedRule.Instance),
                new NotRule<CharacterContext>(HasJumpVitalsRule.Instance)
            ),
            ResolveTo = (_, _) => typeof(RunState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = null, // Any grounded state
            Condition = new CanJumpRule(),
            ResolveTo = (_, _) => typeof(JumpState)
        };

        yield return new StateTransition<CharacterContext>
        {
            From = typeof(SoftLandingState),
            Condition = TrueRule<CharacterContext>.Instance, // Always transition
            ResolveTo = (from, ctx) => PickMovementState(ctx)
        };
    }

    public static Type PickMovementState(CharacterContext ctx)
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
