// using UnityEngine;

// /// <summary>
// /// Player-specific character controller that wires up the player context.
// /// Inherits from the generic base controller with CharacterContext.
// /// </summary>
// public class PlayerCharacterController : BaseCharacterController<CharacterContext>
// {
//     protected override void InitializeStateMachine()
//     {
//         _stateMachine = new AdaptiveStateMachine<CharacterContext>()
//         .WithStates(
//             new Grounded().WithStateMachine(
//                 new AdaptiveStateMachine<CharacterContext>()
//                     .WithStates(new IdleState(), new WalkState(), new RunState(), new SprintState(), new JumpState(), new SoftLandingState(), new HardLandingState())
//                     .WithTransitions(GroundedTransitionBuilder.BuildDefaultTransitions())
//                     .WithInitialState(_context, typeof(IdleState))
//             ),
//             new AirborneState().WithStateMachine(
//                 new AdaptiveStateMachine<CharacterContext>()
//                 .WithStates(new FallingState())
//                 .WithTransitions(
//                     new StateTransition<CharacterContext>
//                     {
//                         From = null, // Any state (including initial entry)
//                         Condition = ctx => ctx.Motor.IsFalling,
//                         ResolveTo = (_, _) => typeof(FallingState)
//                     }
//                 )
//                 .WithInitialState(_context, typeof(FallingState))
//         ))
//         .WithTransitions(
//             new StateTransition<CharacterContext>
//             {
//                 From = typeof(Grounded),
//                 Condition = ctx => !ctx.Sensor.IsGrounded && ctx.Sensor.TimeSinceLastGrounded > LocomotionSettings.GroundedGracePeriod,
//                 ResolveTo = (_, _) => typeof(AirborneState)
//             },
//             new StateTransition<CharacterContext>
//             {
//                 From = typeof(AirborneState),
//                 Condition = ctx => ctx.Sensor.IsGrounded,
//                 ResolveTo = (_, _) => typeof(Grounded)
//             }
//         )
//         .WithInitialState(_context, typeof(Grounded));
//     }

//     protected override void InitializeSensors()
//     {
//         _sensorManager = new SensorManager<CharacterContext>(transform)
//             .WithMonoSensor<GroundSensor>()
//             .WithMonoSensor<StepDetectionSensor>()
//             .WithTransition(CharacterSensorManagerBuilder.BuildDefaultTransitions());
//     }

//     protected override void Awake()
//     {
//         base.Awake();
//         IsPlayerControlled = true;
//     }

// #if UNITY_EDITOR
//     void OnDrawGizmos()
//     {
//         if (!Application.isPlaying || _context == null) return;

//         // Draw state-specific gizmos based on current state
//         DrawStateSpecificGizmos();
//     }

//     private void DrawStateSpecificGizmos()
//     {
//         Vector3 position = transform.position;
//         Vector3 velocity = _context.Motor.CurrentVelocity;

//         // Jump arc prediction when jumping (has upward velocity and physics overrides)
//         if (velocity.y > 0 && (_context.Intent.OverrideGravity || _context.Intent.OverrideYVelocity))
//         {
//             DrawJumpArcGizmos(position, velocity);
//         }

//         // Fall visualization when falling
//         if (_context.Motor.IsFalling)
//         {
//             DrawFallingGizmos(position, velocity);
//         }

//         // State indicator showing current state type
//         DrawCurrentStateIndicator(position);
//     }

//     private void DrawJumpArcGizmos(Vector3 startPos, Vector3 velocity)
//     {
//         if (velocity.y <= 0) return;

//         // Predict jump arc using actual physics values
//         Vector3 currentPos = startPos;
//         Vector3 currentVel = velocity;
//         float timeStep = 0.1f;
//         float gravity = _context.Motor.Gravity;

//         // Draw trajectory points
//         for (int i = 0; i < 20; i++) // Draw 2 seconds of arc
//         {
//             Vector3 nextPos = currentPos + currentVel * timeStep;
//             currentVel.y += gravity * timeStep; // Apply gravity

//             // Draw arc segment with fading color
//             Color arcColor = Color.Lerp(Color.yellow, Color.red, i / 20f);
//             Gizmos.color = arcColor;
//             Gizmos.DrawWireSphere(currentPos, 0.05f);

//             // Draw velocity vector at this point
//             if (i % 3 == 0) // Every few points
//             {
//                 Gizmos.color = arcColor;
//                 Gizmos.DrawRay(currentPos, currentVel * 0.2f);
//             }

//             currentPos = nextPos;

//             if (currentVel.y < 0 && nextPos.y < startPos.y) break; // Stop when we'd hit ground level
//         }

//         // Draw peak of jump
//         float timeToPeak = -velocity.y / gravity;
//         Vector3 peakPos = startPos + new Vector3(
//             velocity.x * timeToPeak,
//             velocity.y * timeToPeak + 0.5f * gravity * timeToPeak * timeToPeak,
//             velocity.z * timeToPeak
//         );

//         Gizmos.color = Color.cyan;
//         Gizmos.DrawWireSphere(peakPos, 0.1f);
//     }

//     private void DrawFallingGizmos(Vector3 position, Vector3 velocity)
//     {
//         // Draw downward velocity indicator
//         if (velocity.y < 0)
//         {
//             Color fallColor = Color.red;
//             Gizmos.color = fallColor;
//             Gizmos.DrawRay(position, Vector3.down * Mathf.Abs(velocity.y) * 0.2f);
//         }

//         // Draw fall danger indicator based on velocity
//         float fallSpeed = Mathf.Abs(velocity.y);
//         if (fallSpeed > 5f) // Dangerous fall speed
//         {
//             Color dangerColor = Color.Lerp(Color.yellow, Color.red, (fallSpeed - 5f) / 10f);
//             Gizmos.color = dangerColor;
//             Gizmos.DrawWireSphere(position + Vector3.up * 0.5f, 0.2f);
//         }

//         // Draw fall distance indicator
//         float fallDistance = _context.Motor.FallStartHeight - position.y;
//         if (fallDistance > 0)
//         {
//             Vector3 fallStart = new Vector3(position.x, _context.Motor.FallStartHeight, position.z);
//             Color distanceColor = Color.Lerp(Color.green, Color.red, fallDistance / 10f); // Red after 10m fall

//             // Draw fall distance line
//             Gizmos.color = distanceColor;
//             Gizmos.DrawLine(fallStart, position);

//             // Draw fall start point
//             Gizmos.color = Color.cyan;
//             Gizmos.DrawWireSphere(fallStart, 0.1f);
//         }
//     }

//     private void DrawCurrentStateIndicator(Vector3 position)
//     {
//         // Draw state indicator above character
//         Vector3 indicatorPos = position + Vector3.up * 3f;

//         if (_context.Motor.CurrentVelocity.y > 0 && (_context.Intent.OverrideGravity || _context.Intent.OverrideYVelocity))
//         {
//             Gizmos.color = Color.yellow;
//             Gizmos.DrawWireCube(indicatorPos, Vector3.one * 0.2f);
//         }
//         else if (_context.Motor.IsFalling)
//         {
//             Gizmos.color = Color.red;
//             Gizmos.DrawWireCube(indicatorPos, Vector3.one * 0.2f);
//         }
//         else if (_context.Sensor.IsGrounded)
//         {
//             Gizmos.color = Color.green;
//             Gizmos.DrawWireCube(indicatorPos, Vector3.one * 0.2f);
//         }
//     }
// #endif
// }
