using System;
using System.Collections.Generic;

public static class CharacterSensorManagerBuilder
{
    public static IEnumerable<SensorTransition<CharacterContext>> BuildDefaultTransitions()
    {
        // Integrated Character Sensor Transitions (replaces separate GroundSensor and StepDetectionSensor)
        yield return new SensorTransition<CharacterContext>
        {
            SensorType = typeof(IntegratedCharacterSensor),
            Mode = SensorUpdateMode.Minimal,
            Condition = ctx => IsIdle(ctx)
        };

        yield return new SensorTransition<CharacterContext>
        {
            SensorType = typeof(IntegratedCharacterSensor),
            Mode = SensorUpdateMode.Reduced,
            Condition = ctx => IsMoving(ctx)
        };

        yield return new SensorTransition<CharacterContext>
        {
            SensorType = typeof(IntegratedCharacterSensor),
            Mode = SensorUpdateMode.EveryFrame,
            Condition = ctx => !ctx.Sensor.IsGrounded || (IsMoving(ctx) && ctx.Motor.CurrentVelocity.magnitude > 3f)
        };
    }

    private static bool IsIdle(CharacterContext ctx)
    {
        return ctx.Motor.CurrentVelocity.magnitude <= 0.1f && ctx.Sensor.IsGrounded;
    }

    private static bool IsMoving(CharacterContext ctx)
    {
        return ctx.Motor.CurrentVelocity.magnitude > 0.1f && ctx.Sensor.IsGrounded;
    }

}
