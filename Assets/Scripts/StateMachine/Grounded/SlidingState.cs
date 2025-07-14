using UnityEngine;

/// <summary>
/// Character is sliding down a steep slope with limited or no control
/// </summary>
public class SlidingState : GroundedBaseState
{
    private static readonly int SlideAnimHash = Animator.StringToHash("Slide");

    [Header("Sliding Settings")]
    [SerializeField] private float slideControlFactor = 0.3f; // How much input control during slide

    public override void Enter(CharacterContext context)
    {
        PlayAnimation(context.References.Animator, SlideAnimHash, 0.1f);
    }

    public override void FixedUpdate(CharacterContext context)
    {
        // Allow limited control during sliding - like steering a sled
        Vector3 inputDirection = context.Input.MoveDirection;
        Vector3 slideDirection = context.Sensor.SlopeDirection;

        // Player can influence slide direction slightly
        Vector3 controlInfluence = Vector3.ProjectOnPlane(inputDirection, Vector3.up) * slideControlFactor;
        Vector3 desiredDirection = (slideDirection + controlInfluence).normalized;

        // Set intent - motor will handle the actual sliding physics
        float currentSpeed = context.Motor.CurrentVelocity.magnitude;
        float slideSpeed = Mathf.Max(currentSpeed, 1f); // Minimum slide speed

        ApplyMovement(context, desiredDirection, slideSpeed);
        ApplyRotation(context, desiredDirection);
    }

#if UNITY_EDITOR
    public void DrawGizmos(CharacterContext context)
    {
        Vector3 position = context.References.GameObject.transform.position;

        // Draw sliding direction
        Vector3 slideDir = context.Sensor.SlopeDirection * 2f;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(position, slideDir);

        // Draw control influence
        Vector3 inputDirection = context.Input.MoveDirection;
        if (inputDirection.magnitude > 0.1f)
        {
            Vector3 controlInfluence = Vector3.ProjectOnPlane(inputDirection, Vector3.up) * slideControlFactor;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(position + Vector3.up * 0.5f, controlInfluence * 2f);
        }

        // Draw slide state indicator
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(position + Vector3.up * 3f, Vector3.one * 0.3f);
    }
#endif
}
