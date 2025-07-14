using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCharacterController : BaseCharacterController<CharacterContext>
{
    protected override void InitializeStateMachine(CharacterContext context)
    {
        _stateMachine = new AdaptiveStateMachine<CharacterContext>()
        .WithStates(
            new Grounded().WithStateMachine(
                new AdaptiveStateMachine<CharacterContext>()
                    .WithStates(
                        new IdleState(),
                        new WalkState(),
                        new RunState(),
                        new SprintState(),
                        new JumpState(),
                        new SlidingState(),
                        new SoftLandingState(),
                        new HardLandingState()
                    )
                    .WithTransitions(GroundedTransitionBuilder.BuildDefaultTransitions())
                    .WithInitialState(_context, typeof(IdleState))
            ),
            new AirborneState().WithStateMachine(
                new AdaptiveStateMachine<CharacterContext>()
                .WithStates(new FallingState())
                .WithTransitions(
                    new StateTransition<CharacterContext>
                    {
                        From = null, // Any state (including initial entry)
                        Condition = ctx => ctx.Motor.IsFalling,
                        ResolveTo = (_, _) => typeof(FallingState)
                    }
                )
                .WithInitialState(_context, typeof(FallingState))
        ))
        .WithTransitions(
            new StateTransition<CharacterContext>
            {
                From = typeof(Grounded),
                Condition = ctx => !ctx.Sensor.IsGrounded && ctx.Sensor.TimeSinceLastGrounded > LocomotionSettings.GroundedGracePeriod,
                ResolveTo = (_, _) => typeof(AirborneState)
            },
            new StateTransition<CharacterContext>
            {
                From = typeof(AirborneState),
                Condition = ctx => ctx.Sensor.IsGrounded,
                ResolveTo = (_, _) => typeof(Grounded)
            }
        )
        .WithInitialState(_context, typeof(Grounded));
    }

    protected override void InitializeSensors(CharacterContext context)
    {
        // Single integrated sensor - much cleaner!
        _sensorManager = new SensorManager<CharacterContext>(transform)
            .WithSensor(new GroundSensor())
            .WithTransition(CharacterSensorManagerBuilder.BuildDefaultTransitions());

        _sensorManager.Initialize(context);
    }


    protected override void Awake()
    {
        base.Awake();
        IsPlayerControlled = true;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!Application.isPlaying || _context == null) return;

        DrawStateSpecificGizmos();
    }

    private void DrawStateSpecificGizmos()
    {
        Vector3 position = transform.position;

        // Draw current state indicator
        DrawCurrentStateIndicator(position);

        // Draw velocity information
        DrawVelocityInfo(position);

        // Draw sensor information
        DrawSensorInfo(position);
    }

    private void DrawCurrentStateIndicator(Vector3 position)
    {
        Vector3 indicatorPos = position + Vector3.up * 3f;

        if (_context.Motor.CurrentVelocity.y > 0 && (_context.Intent.OverrideGravity || _context.Intent.OverrideYVelocity))
        {
            Gizmos.color = Color.yellow; // Jumping/Flying
            Gizmos.DrawWireCube(indicatorPos, Vector3.one * 0.2f);
        }
        else if (_context.Motor.IsFalling)
        {
            Gizmos.color = Color.red; // Falling
            Gizmos.DrawWireCube(indicatorPos, Vector3.one * 0.2f);
        }
        else if (_context.Sensor.IsGrounded)
        {
            if (_context.Sensor.ShouldSlide)
            {
                Gizmos.color = Color.magenta; // Sliding
            }
            else if (_context.Sensor.IsOnSlope)
            {
                Gizmos.color = Color.orange; // On slope
            }
            else
            {
                Gizmos.color = Color.green; // Normal grounded
            }
            Gizmos.DrawWireCube(indicatorPos, Vector3.one * 0.2f);
        }
    }

    private void DrawVelocityInfo(Vector3 position)
    {
        Vector3 velocity = _context.Motor.CurrentVelocity;

        // Draw velocity magnitude as colored line
        if (velocity.magnitude > 0.1f)
        {
            Color speedColor = Color.Lerp(Color.green, Color.red, velocity.magnitude / 10f);
            Gizmos.color = speedColor;
            Gizmos.DrawLine(position + Vector3.up * 2f, position + Vector3.up * 2f + velocity.normalized);
        }
    }

    private void DrawSensorInfo(Vector3 position)
    {
        // Draw step ahead indicator
        if (_context.Sensor.HasStepAhead)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(position, _context.Sensor.StepPosition);
            Gizmos.DrawWireSphere(_context.Sensor.StepPosition, 0.1f);
        }

        // Draw slope direction
        if (_context.Sensor.IsOnSlope)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(position, _context.Sensor.SlopeDirection * 2f);
        }
    }
#endif
}
