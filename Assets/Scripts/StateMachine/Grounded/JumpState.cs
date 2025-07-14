using UnityEngine;

public class JumpState : GroundedBaseState
{
    private static readonly int JumpAnim = Animator.StringToHash("Jump");

    // Jump configuration - tuned for snappy, responsive feel
    private const float MinJumpHeight = 0.6f;      // Minimum jump height (tap) - reduced for snappier feel
    private const float MaxJumpHeight = 1.6f;      // Maximum jump height (hold) - reduced for less air time
    private const float JumpInputWindow = 0.15f;   // How long player can hold to reach max height - shorter for quicker jumps

    // Jump state tracking
    private float _jumpStartTime;
    private float _initialJumpVelocity;
    private bool _jumpInputReleased;
    private bool _jumpCompleted;

    public override bool IsBlocking => !_jumpCompleted; // Block transitions until jump is complete

    public override void Enter(CharacterContext context)
    {
        context.Vitals.DrainStamina(10f);
        PlayAnimation(context.References.Animator, JumpAnim);

        _jumpStartTime = Time.time;
        _jumpInputReleased = false;
        _jumpCompleted = false;

        // Calculate initial jump velocity for minimum jump height
        _initialJumpVelocity = CalculateJumpVelocity(MinJumpHeight, context.Motor.Gravity);

        Logwin.Log("[JumpState]", $"Jump started with initial velocity: {_initialJumpVelocity}");
    }

    public override void FixedUpdate(CharacterContext context)
    {
        float jumpTime = Time.time - _jumpStartTime;

        CheckJumpInputRelease(context, jumpTime);

        Vector3 horizontalMove = CalculateHorizontalMovement(context);
        Vector3 velocity = CalculateJumpVelocity(context, jumpTime);

        SetPhysicsIntent(context, jumpTime, horizontalMove, velocity);
        ApplyRotationIfMoving(context);

        LogJumpState(velocity, jumpTime, context);
    }

    private void CheckJumpInputRelease(CharacterContext context, float jumpTime)
    {
        if (!context.Input.IsJumpPressed && !_jumpInputReleased)
        {
            _jumpInputReleased = true;
            Logwin.Log("[JumpState]", $"Jump input released at time: {jumpTime}");
        }
    }

    private Vector3 CalculateHorizontalMovement(CharacterContext context)
    {
        // Preserve ALL existing horizontal momentum when jumping - never slow down
        Vector3 currentHorizontal = new Vector3(context.Motor.CurrentVelocity.x, 0, context.Motor.CurrentVelocity.z);

        // Air control only adds steering ability, doesn't replace momentum
        Vector3 airControlInput = context.Input.MoveDirection * context.Stats.MoveSpeed * LocomotionSettings.JumpAirControl;

        // Always preserve full momentum, add air control on top
        return currentHorizontal + airControlInput;
    }

    private Vector3 CalculateJumpVelocity(CharacterContext context, float jumpTime)
    {
        Vector3 velocity = context.Motor.CurrentVelocity;

        if (IsInActiveJumpPhase(jumpTime))
        {
            // Variable jump height based on how long they're holding
            float holdProgress = jumpTime / JumpInputWindow;
            float targetHeight = Mathf.Lerp(MinJumpHeight, MaxJumpHeight, holdProgress);
            float targetVelocity = CalculateJumpVelocity(targetHeight, context.Motor.Gravity);

            // Smoothly increase velocity towards target
            velocity.y = Mathf.Max(_initialJumpVelocity, Mathf.Lerp(_initialJumpVelocity, targetVelocity, holdProgress));
        }
        else
        {
            // Jump phase complete, allow transition to airborne
            _jumpCompleted = true;

            // If jump was released early and we're still going up, apply extra downward force for snappier feel
            if (_jumpInputReleased && velocity.y > 0)
            {
                velocity.y *= 0.6f; // Reduce upward momentum for quicker peak and fall
            }
        }

        return velocity;
    }

    private bool IsInActiveJumpPhase(float jumpTime)
    {
        return !_jumpInputReleased && jumpTime < JumpInputWindow;
    }

    private void SetPhysicsIntent(CharacterContext context, float jumpTime, Vector3 horizontalMove, Vector3 velocity)
    {
        if (IsInActiveJumpPhase(jumpTime))
        {
            // Still in active jump phase - override physics
            context.Intent.OverrideGravity = true;
            context.Intent.OverrideYVelocity = true;
            context.Intent.DesiredVelocity = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);
        }
        else
        {
            // Jump input released or window expired - let motor handle gravity
            context.Intent.OverrideGravity = false;
            context.Intent.OverrideYVelocity = false;
            context.Intent.DesiredVelocity = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);
        }
    }

    private void ApplyRotationIfMoving(CharacterContext context)
    {
        if (context.Input.MoveDirection != Vector3.zero)
        {
            ApplyRotation(context, context.Input.MoveDirection);
        }
    }

    private void LogJumpState(Vector3 velocity, float jumpTime, CharacterContext context)
    {
        Logwin.Log("[JumpState]", $"Jump velocity: {velocity.y:F2}, Time: {jumpTime:F2}, Released: {_jumpInputReleased}, Completed: {_jumpCompleted}, Overrides: G:{context.Intent.OverrideGravity} Y:{context.Intent.OverrideYVelocity}");
    }

    private static float CalculateJumpVelocity(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(-2f * jumpHeight * gravity);
    }

    public override void Exit(CharacterContext context)
    {
        Logwin.Log("[JumpState]", $"Jump ended, final velocity: {context.Motor.CurrentVelocity.y:F2}");
    }
}
