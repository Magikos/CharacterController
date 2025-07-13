using UnityEngine;

#if UNITY_EDITOR
using MiniTools.BetterGizmos;
#endif

/// <summary>
/// Responsible for applying motion, gravity, and velocity resolution to the character.
/// Contains embedded motor behaviors for clean, integrated physics processing.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CharacterMotor : MonoBehaviour, ICharacterMotor
{
    #region Components & Context
    private Rigidbody _rigidbody;
    private CharacterContext _context;
    #endregion

    #region Motor Settings
    [Header("Motion Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float terminalVelocity = -15f;

    [Header("Slope Behavior")]
    [SerializeField] private float maxWalkableAngle = 45f;
    [SerializeField] private float maxRunAngle = 30f;
    [SerializeField] private float slideAngle = 60f;
    [SerializeField] private AnimationCurve uphillSpeedCurve = AnimationCurve.Linear(0f, 1f, 45f, 0.5f);
    [SerializeField] private AnimationCurve downhillSpeedCurve = AnimationCurve.Linear(0f, 1f, 45f, 1.2f);
    [SerializeField] private float slopeFrictionMultiplier = 0.8f;
    [SerializeField] private float slideSpeed = 8f;
    [SerializeField] private float slideAcceleration = 2f;
    [SerializeField] private float slideControl = 0.3f;

    [Header("Step-Up Behavior")]
    [SerializeField] private float maxAutoStepHeight = 0.3f;
    [SerializeField] private float stepUpSpeed = 2f;
    [SerializeField] private float stepUpForwardBoost = 1.2f;
    [SerializeField] private AnimationCurve stepUpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Debug Visualization")]
    [SerializeField] private bool showVelocityGizmos = true;
    [SerializeField] private bool showPhysicsOverrides = true;
    #endregion

    #region Step-Up State
    private bool _isSteppingUp = false;
    private float _stepUpProgress = 0f;
    private Vector3 _stepStartPosition;
    private Vector3 _stepTargetPosition;
    #endregion

    #region Initialization
    public void Initialize(ICharacterContext context)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        _context = context as CharacterContext;
        _context.Motor.CurrentVelocity = Vector3.zero;
        _context.Motor.Gravity = gravity;
    }
    #endregion

    #region Main Physics Loop
    public void ApplyMotion(ICharacterContext context)
    {
        Vector3 velocity = context.Motor.CurrentVelocity;
        Vector3 desiredVelocity = context.Intent.DesiredVelocity;

        // Process step-up behavior first (highest priority)
        velocity = ProcessStepUpBehavior(velocity);

        // Process standard physics if not stepping up
        if (!_isSteppingUp)
        {
            velocity = ProcessGravity(velocity, context);
            velocity = ProcessYVelocity(velocity, desiredVelocity, context);
            velocity = ProcessHorizontalMovement(velocity, desiredVelocity, context);
        }

        // Apply ground positioning correction
        ProcessGroundPositioning(context);

        // Update context and apply to rigidbody
        context.Motor.CurrentVelocity = velocity;
        _rigidbody.MovePosition(transform.position + velocity * context.FixedDeltaTime);

        // Apply rotation
        ProcessRotation(context);
    }
    #endregion

    #region Physics Processing Methods
    private Vector3 ProcessGravity(Vector3 velocity, ICharacterContext context)
    {
        if (!context.Intent.OverrideGravity && !context.Sensor.IsGrounded)
        {
            velocity.y += context.Motor.Gravity * context.Stats.GravityScale * context.FixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, terminalVelocity);
        }
        else if (context.Sensor.IsGrounded && velocity.y < 0)
        {
            // Reset downward velocity when grounded to prevent sinking
            velocity.y = 0f;
        }
        return velocity;
    }

    private Vector3 ProcessYVelocity(Vector3 velocity, Vector3 desiredVelocity, ICharacterContext context)
    {
        if (context.Intent.OverrideYVelocity)
        {
            velocity.y = desiredVelocity.y;
        }
        return velocity;
    }

    private Vector3 ProcessHorizontalMovement(Vector3 velocity, Vector3 desiredVelocity, ICharacterContext context)
    {
        if (context.Intent.OverrideHorizontalPhysics)
        {
            velocity.x = desiredVelocity.x;
            velocity.z = desiredVelocity.z;
        }
        else
        {
            Vector3 horizontalDesired = new Vector3(desiredVelocity.x, 0, desiredVelocity.z);

            // Check for obstacles - prevent movement if blocked
            if (context.Sensor.HasObstacleAhead && horizontalDesired.magnitude > 0.1f)
            {
                // Stop all horizontal movement when obstacle detected
                horizontalDesired = Vector3.zero;
                Logwin.Log("CharacterMotor", "Obstacle ahead - stopping movement");
            }

            // Apply slope behavior modifications
            if (context.Sensor.IsGrounded && context.Sensor.IsOnSlope)
            {
                horizontalDesired = ProcessSlopeBehavior(horizontalDesired, velocity, context);
            }

            // Apply acceleration
            float acceleration = context.Sensor.IsGrounded ?
                context.Stats.Acceleration :
                context.Stats.Acceleration * context.Stats.AirAccelerationMultiplier;

            velocity.x = Mathf.MoveTowards(velocity.x, horizontalDesired.x, acceleration * context.FixedDeltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, horizontalDesired.z, acceleration * context.FixedDeltaTime);
        }
        return velocity;
    }

    private void ProcessGroundPositioning(ICharacterContext context)
    {
        // Disabled ground positioning correction to prevent oscillation
        // The Rigidbody physics should handle ground positioning naturally

        // if (context.Sensor.IsGrounded &&
        //     Vector3.Distance(transform.position, context.Sensor.DesiredGroundPosition) > 0.01f)
        // {
        //     float maxCorrection = 0.5f;
        //     Vector3 targetPosition = Vector3.MoveTowards(
        //         transform.position,
        //         context.Sensor.DesiredGroundPosition,
        //         maxCorrection * context.FixedDeltaTime
        //     );
        //     transform.position = targetPosition;
        // }
    }

    private void ProcessRotation(ICharacterContext context)
    {
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = context.Intent.DesiredRotation;

        // Only apply rotation if there's a meaningful difference
        if (Quaternion.Angle(currentRotation, targetRotation) > 1f)
        {
            float rotationSpeed = 10f; // Adjust this value for rotation speed
            Quaternion nextRotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * context.FixedDeltaTime);

            // Ensure the quaternion is normalized (unit length)
            nextRotation.Normalize();

            _rigidbody.MoveRotation(nextRotation);
        }
    }
    #endregion

    #region Slope Behavior
    private Vector3 ProcessSlopeBehavior(Vector3 horizontalDesired, Vector3 currentVelocity, ICharacterContext context)
    {
        if (context.Sensor.ShouldSlide)
        {
            // Calculate slide velocity
            Vector3 slideDirection = context.Sensor.SlopeDirection;
            Vector3 slideVelocity = slideDirection * slideSpeed;

            // Allow some player control while sliding
            Vector3 controlInfluence = horizontalDesired * slideControl;
            return Vector3.Lerp(currentVelocity, slideVelocity + controlInfluence, slideAcceleration * context.FixedDeltaTime);
        }
        else if (context.Sensor.CanWalkOnSlope)
        {
            // Align movement to slope and apply speed modifier
            Vector3 alignedMovement = Vector3.ProjectOnPlane(horizontalDesired, context.Sensor.GroundNormal).normalized * horizontalDesired.magnitude;
            float speedModifier = GetSlopeSpeedModifier(context.Sensor.SlopeAngle, horizontalDesired, context.Sensor.SlopeDirection);
            return alignedMovement * speedModifier;
        }
        else
        {
            // Slope too steep to walk on
            return Vector3.zero;
        }
    }

    private float GetSlopeSpeedModifier(float slopeAngle, Vector3 moveDirection, Vector3 slopeDirection)
    {
        if (slopeAngle <= 1f) return 1f;

        float dot = Vector3.Dot(moveDirection.normalized, slopeDirection.normalized);
        bool movingUphill = dot > 0.1f;
        bool movingDownhill = dot < -0.1f;

        if (movingUphill)
            return uphillSpeedCurve.Evaluate(slopeAngle);
        else if (movingDownhill)
            return downhillSpeedCurve.Evaluate(slopeAngle);

        return Mathf.Lerp(1f, slopeFrictionMultiplier, slopeAngle / 45f);
    }
    #endregion

    #region Step-Up Behavior
    private Vector3 ProcessStepUpBehavior(Vector3 velocity)
    {
        // Check if we should start step-up
        if (!_isSteppingUp && ShouldAutoStepUp())
        {
            StartStepUp();
        }

        // Update step-up motion if active
        if (_isSteppingUp)
        {
            return UpdateStepUpMotion(velocity);
        }

        return velocity;
    }

    private bool ShouldAutoStepUp()
    {
        if (!_context.Sensor.IsGrounded) return false;
        if (!_context.Sensor.HasStepAhead) return false;
        if (_context.Sensor.StepHeight > maxAutoStepHeight) return false;
        if (_context.Sensor.StepHeight < 0.05f) return false;

        Vector3 moveDirection = _context.Motor.CurrentVelocity.normalized;
        Vector3 stepDirection = (_context.Sensor.StepPosition - transform.position).normalized;
        return Vector3.Dot(moveDirection, stepDirection) > 0.5f;
    }

    private void StartStepUp()
    {
        _isSteppingUp = true;
        _stepUpProgress = 0f;
        _stepStartPosition = transform.position;
        _stepTargetPosition = _context.Sensor.StepPosition;
    }

    private Vector3 UpdateStepUpMotion(Vector3 currentVelocity)
    {
        _stepUpProgress += stepUpSpeed * _context.FixedDeltaTime;

        if (_stepUpProgress >= 1f)
        {
            // Step-up complete
            _isSteppingUp = false;
            transform.position = _stepTargetPosition;

            Vector3 forwardDirection = (_stepTargetPosition - _stepStartPosition).normalized;
            forwardDirection.y = 0;
            return currentVelocity + forwardDirection * stepUpForwardBoost;
        }

        // Calculate smooth step-up motion
        float easedProgress = stepUpCurve.Evaluate(_stepUpProgress);
        Vector3 targetPosition = Vector3.Lerp(_stepStartPosition, _stepTargetPosition, easedProgress);
        Vector3 deltaPosition = targetPosition - transform.position;
        return deltaPosition / _context.FixedDeltaTime;
    }
    #endregion

    #region Debug Visualization
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || _context == null) return;
        if (!showVelocityGizmos && !showPhysicsOverrides) return;

        Vector3 position = transform.position;

        if (showVelocityGizmos)
        {
            DrawVelocityGizmos(position);
        }

        if (showPhysicsOverrides)
        {
            DrawPhysicsOverrides(position);
        }

        if (_isSteppingUp)
        {
            DrawStepUpGizmos();
        }
    }

    private void DrawVelocityGizmos(Vector3 position)
    {
        Vector3 velocity = _context.Motor.CurrentVelocity;
        if (velocity.magnitude > 0.1f)
        {
            BetterGizmos.DrawArrow(Color.blue, position + Vector3.up * 0.1f,
                position + Vector3.up * 0.1f + velocity, Vector3.up, 0.3f);
        }
    }

    private void DrawPhysicsOverrides(Vector3 position)
    {
        Vector3 indicatorPos = position + Vector3.up * 2.5f;

        if (_context.Intent.OverrideGravity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(indicatorPos + Vector3.left * 0.3f, Vector3.one * 0.1f);
        }

        if (_context.Intent.OverrideYVelocity)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(indicatorPos, Vector3.one * 0.1f);
        }

        if (_context.Intent.OverrideHorizontalPhysics)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(indicatorPos + Vector3.right * 0.3f, Vector3.one * 0.1f);
        }
    }

    private void DrawStepUpGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_stepStartPosition, _stepTargetPosition);

        float easedProgress = stepUpCurve.Evaluate(_stepUpProgress);
        Vector3 currentStepPos = Vector3.Lerp(_stepStartPosition, _stepTargetPosition, easedProgress);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currentStepPos, 0.1f);
    }
#endif
    #endregion
}
