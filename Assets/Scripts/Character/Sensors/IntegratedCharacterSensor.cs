using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using MiniTools.BetterGizmos;
#endif

/// <summary>
/// Integrated sensor system that handles ground detection, step detection, and slope analysis.
/// Embedded directly in the character controller for clean, integrated environmental sensing.
/// </summary>
[RequireComponent(typeof(CapsuleCollider))]
public class IntegratedCharacterSensor : MonoBehaviour, ISensor<CharacterContext>
{
    #region Components & State
    private CapsuleCollider _collider;
    private RaycastHit[] _hits = new RaycastHit[8];
    private HashSet<Collider> _selfColliders;
    private bool _initialized = false;
    #endregion

    #region Settings
    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask excludeLayers = 0;

    [Header("Step Detection")]
    [SerializeField] private float stepCheckDistance = 0.5f;
    [SerializeField] private float maxDetectableStepHeight = 1.5f;
    [SerializeField] private float minStepHeight = 0.05f;

    [Header("Debug Visualization")]
    [SerializeField] private bool showGroundCast = true;
    [SerializeField] private bool showStepDetection = true;
    #endregion

    #region ISensor Implementation
    public SensorUpdateMode DefaultMode => SensorUpdateMode.Reduced;

    public void Initialize(CharacterContext context)
    {
        if (_initialized) return;

        _collider = GetComponent<CapsuleCollider>();
        InitializeSelfColliders();
        _initialized = true;
    }

    public void UpdateSensor(CharacterContext context)
    {
        UpdateGroundDetection(context);
        UpdateStepDetection(context);
        UpdateObstacleDetection(context);
    }
    #endregion

    #region Ground Detection
    private void UpdateGroundDetection(CharacterContext context)
    {

    }

    private void LogGroundState(bool wasGrounded, bool grounded, RaycastHit hit)
    {

    }
    #endregion

    #region Step Detection
    private void UpdateStepDetection(CharacterContext context)
    {
        Vector3 moveDirection = context.Motor.CurrentVelocity.normalized;
        if (moveDirection.magnitude < 0.1f)
        {
            ResetStepDetection(context);
            return;
        }

        DetectStepInDirection(context, moveDirection);
    }

    private void DetectStepInDirection(CharacterContext context, Vector3 direction)
    {
        Vector3 basePos = transform.position + _collider.center;
        int layerMask = ~excludeLayers.value;

        // Lower check - detect step face
        Vector3 lowerStart = basePos + Vector3.up * 0.1f;
        bool hasObstacle = Physics.Raycast(lowerStart, direction, out RaycastHit lowerHit,
            stepCheckDistance, layerMask);

        if (!hasObstacle)
        {
            ResetStepDetection(context);
            return;
        }

        // Upper check - should go over a step
        Vector3 upperStart = basePos + Vector3.up * maxDetectableStepHeight;
        bool upperBlocked = Physics.Raycast(upperStart, direction, stepCheckDistance, layerMask);

        if (upperBlocked)
        {
            // Too tall to be a step
            context.Sensor.HasStepAhead = false;
            context.Sensor.HasObstacleAhead = true;
            return;
        }

        // Downward cast to find step top
        Vector3 downwardStart = lowerHit.point + direction * 0.1f + Vector3.up * maxDetectableStepHeight;
        if (Physics.Raycast(downwardStart, Vector3.down, out RaycastHit stepTopHit,
            maxDetectableStepHeight * 2f, layerMask))
        {
            float stepHeight = stepTopHit.point.y - transform.position.y;

            if (stepHeight >= minStepHeight && stepHeight <= maxDetectableStepHeight)
            {
                context.Sensor.HasStepAhead = true;
                context.Sensor.StepHeight = stepHeight;
                context.Sensor.StepPosition = stepTopHit.point;
                context.Sensor.HasObstacleAhead = false;

                Logwin.Log("IntegratedSensor", $"Step detected: height {stepHeight:F2}m at {stepTopHit.point}");
            }
            else
            {
                ResetStepDetection(context);
            }
        }
        else
        {
            ResetStepDetection(context);
        }
    }

    private void ResetStepDetection(CharacterContext context)
    {
        context.Sensor.HasStepAhead = false;
        context.Sensor.StepHeight = 0f;
        context.Sensor.StepPosition = Vector3.zero;
        context.Sensor.HasObstacleAhead = false;
    }
    #endregion

    #region Obstacle Detection
    private void UpdateObstacleDetection(CharacterContext context)
    {
        // Reset obstacle detection
        context.Sensor.HasObstacleAhead = false;

        // Only check if we have movement input
        Vector3 moveDirection = context.Input.MoveDirection;
        if (moveDirection.magnitude < 0.1f) return;

        // Cast from character center in movement direction
        Vector3 castStart = transform.position + _collider.center;
        float castDistance = stepCheckDistance;
        int layerMask = ~excludeLayers.value;

        // Check for obstacles in movement direction
        if (Physics.Raycast(castStart, moveDirection, castDistance, layerMask))
        {
            context.Sensor.HasObstacleAhead = true;
            Logwin.Log("IntegratedSensor", $"Obstacle detected in movement direction: {moveDirection}");
        }
    }
    #endregion

    #region Initialization Helpers
    private void InitializeSelfColliders()
    {
        _selfColliders = new HashSet<Collider>();
        var colliders = GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
        {
            _selfColliders.Add(col);
        }

        Logwin.Log("IntegratedSensor", $"Initialized {_selfColliders.Count} self-colliders for exclusion");
    }
    #endregion

    #region Debug Visualization
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || _collider == null) return;

        Vector3 center = transform.position + _collider.center;
        float radius = _collider.radius * 0.95f;
        float halfHeight = _collider.height / 2f;

        if (showGroundCast)
        {
            DrawGroundDetectionGizmos(center, radius, halfHeight);
        }

        if (showStepDetection)
        {
            DrawStepDetectionGizmos(center);
            DrawObstacleDetectionGizmos(center);
        }
    }

    private void DrawGroundDetectionGizmos(Vector3 center, float radius, float halfHeight)
    {
        Vector3 castStart = center - Vector3.up * (halfHeight - radius);

        // Draw ground cast
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(castStart, radius);
        Gizmos.DrawLine(castStart, castStart + Vector3.down * groundCheckDistance);
        Gizmos.DrawWireSphere(castStart + Vector3.down * groundCheckDistance, radius);
    }

    private void DrawStepDetectionGizmos(Vector3 center)
    {
        // Draw step detection rays
        Gizmos.color = Color.blue;
        Vector3 forward = transform.forward;

        // Lower ray
        Vector3 lowerStart = center + Vector3.up * 0.1f;
        Gizmos.DrawRay(lowerStart, forward * stepCheckDistance);

        // Upper ray
        Vector3 upperStart = center + Vector3.up * maxDetectableStepHeight;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(upperStart, forward * stepCheckDistance);
    }

    private void OnDrawGizmosSelected()
    {
        if (_collider == null) return;

        Vector3 basePos = transform.position + _collider.center;

        // Draw detection zones
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(basePos + Vector3.forward * stepCheckDistance * 0.5f,
                           new Vector3(0.1f, maxDetectableStepHeight, stepCheckDistance));
    }

    private void DrawObstacleDetectionGizmos(Vector3 center)
    {
        // Draw obstacle detection ray in forward direction  
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(center, transform.forward * stepCheckDistance);

        // Draw a wire sphere to show detection range
        Gizmos.color = Color.cyan * 0.3f;
        Gizmos.DrawWireSphere(center + transform.forward * stepCheckDistance, 0.2f);
    }
#endif
    #endregion
}
