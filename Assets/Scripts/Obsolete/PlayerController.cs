// using System.Linq;
// using UnityEngine;

// [RequireComponent(typeof(CharacterController), typeof(Animator))]
// public class PlayerController : MonoBehaviour
// {
//     private CharacterController _controller;
//     private Animator _animator;
//     private AdaptiveStateMachine<LocomotionContext> _rootStateMachine;
//     private LocomotionContext _context;
//     private PlayerControls _inputActions;

//     [SerializeField] private float _stamina = 100f;
//     [SerializeField] private Transform _cameraTransform;

//     private void Awake()
//     {
//         _controller = GetComponent<CharacterController>();
//         _animator = GetComponent<Animator>();
//         _cameraTransform = Camera.main.transform;

//         _inputActions = new PlayerControls();
//         _inputActions.Character.Enable();

//         _context = new LocomotionContext
//         {
//             Animator = _animator,
//             Controller = _controller,
//             Stats = new CharacterStats { Stamina = _stamina }
//         };

//         // Root state machine
//         _rootStateMachine = new AdaptiveStateMachine<LocomotionContext>()
//         .WithStates(
//             new Grounded().WithStateMachine(
//                 new AdaptiveStateMachine<LocomotionContext>()
//                     .WithStates(new IdleState(), new WalkState(), new RunState(), new SprintState(), new JumpState(), new SoftLandingState())
//                     .WithTransitions(GroundedTransitionBuilder.BuildDefaultTransitions())
//                     .WithInitialState(_context, typeof(IdleState))
//             ),
//             new AirborneState().WithStateMachine(
//                 new AdaptiveStateMachine<LocomotionContext>()
//                 .WithStates(new FallingState())
//                 .WithInitialState(_context, typeof(FallingState))
//         ))
//         .WithTransitions(
//             new StateTransition<LocomotionContext>
//             {
//                 From = typeof(Grounded),
//                 Condition = ctx => !ctx.IsGrounded,
//                 ResolveTo = (_, _) => typeof(AirborneState)
//             },
//             new StateTransition<LocomotionContext>
//             {
//                 From = typeof(AirborneState),
//                 Condition = ctx => ctx.IsGrounded,
//                 ResolveTo = (_, _) => typeof(Grounded)
//             }
//         )
//         .WithInitialState(_context, typeof(Grounded));
//     }

//     private void Update()
//     {
//         Vector2 input = _inputActions.Character.Move.ReadValue<Vector2>();
//         Vector3 camForward = _cameraTransform.forward;
//         Vector3 camRight = _cameraTransform.right;
//         camForward.y = 0;
//         camRight.y = 0;

//         Vector3 direction = (camForward.normalized * input.y + camRight.normalized * input.x).normalized;

//         _context.MoveInputMagnitude = input.magnitude;
//         _context.MoveDirection = direction;
//         _context.DeltaTime = Time.deltaTime;
//         _context.IsJumpPressed = _inputActions.Character.Jump.IsPressed();
//         _context.IsSprintPressed = _inputActions.Character.Sprint.IsPressed();

//         Debug.Log($"IsSprintPressed: {_context.IsSprintPressed}, MoveInput: {_context.MoveInputMagnitude}");

//         //coyote time logic
//         if (_controller.isGrounded)
//         {
//             _context.TimeSinceLastGrounded = 0f;
//             _context.IsGrounded = true;
//         }
//         else
//         {
//             _context.TimeSinceLastGrounded += Time.deltaTime;
//             _context.IsGrounded = _context.TimeSinceLastGrounded <= LocomotionSettings.GroundedGracePeriod;
//         }

//         // You can trigger stamina regen logic here or from a StatusEffectState
//         _context.Stats.RegenStamina(_context.DeltaTime, !_context.IsSprintPressed);

//         _rootStateMachine.Update(_context);
//     }

//     private void FixedUpdate()
//     {
//         _context.FixedDeltaTime = Time.fixedDeltaTime;
//         _rootStateMachine.FixedUpdate(_context);
//     }

//     private void LateUpdate()
//     {
//         _rootStateMachine.LateUpdate(_context);
//     }
// }
