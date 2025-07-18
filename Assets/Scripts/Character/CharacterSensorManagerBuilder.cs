using System;
using System.Collections.Generic;

public static class CharacterSensorManagerBuilder
{
    public static IEnumerable<SensorTransition<CharacterContext>> BuildDefaultTransitions()
    {
        return new SensorTransition<CharacterContext>[]
        {
            // Collider overlapping sensor - always active
            new SensorTransition<CharacterContext>
            {
                SensorType = typeof(ColliderOverlappingSensor),
                Mode = SensorUpdateMode.EveryFrame,
                Condition = ctx => IsMoving(ctx)
            },

            // Collider overlapping sensor - always active
            new SensorTransition<CharacterContext>
            {
                SensorType = typeof(ColliderOverlappingSensor),
                Mode = SensorUpdateMode.Reduced,
                Condition = ctx => IsIdle(ctx)
            },

            // Idle state - minimal sensor updates
            new SensorTransition<CharacterContext>
            {
                SensorType = typeof(GroundSensor),
                Mode = SensorUpdateMode.Minimal,
                Condition = ctx => IsIdle(ctx)
            },
            
            // Moving state - reduced sensor updates
            new SensorTransition<CharacterContext>
            {
                SensorType = typeof(GroundSensor),
                Mode = SensorUpdateMode.Reduced,
                Condition = ctx => IsMoving(ctx)
            },
            
            // High activity - every frame updates
            new SensorTransition<CharacterContext>
            {
                SensorType = typeof(GroundSensor),
                Mode = SensorUpdateMode.EveryFrame,
                Condition = ctx => IsHighActivity(ctx)
            }
        };
    }

    private static bool IsIdle(CharacterContext ctx)
    {
        return ctx.Motor.CurrentVelocity.magnitude <= 0.1f && ctx.Sensor.IsGrounded;
    }

    private static bool IsMoving(CharacterContext ctx)
    {
        return ctx.Motor.CurrentVelocity.magnitude > 0.1f && ctx.Motor.CurrentVelocity.magnitude <= 3f && ctx.Sensor.IsGrounded;
    }

    private static bool IsHighActivity(CharacterContext ctx)
    {
        return !ctx.Sensor.IsGrounded ||
               ctx.Motor.CurrentVelocity.magnitude > 3f ||
               ctx.Sensor.IsOnSlope ||
               ctx.Sensor.HasStepAhead;
    }
}
