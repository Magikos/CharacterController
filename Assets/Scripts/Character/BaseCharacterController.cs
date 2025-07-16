using Codice.CM.Common;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer.Operations;
using UnityEngine;

/// <summary>
/// The base controller that owns all character motion systems.
/// Designed for both players and AI, modular, generic, and context-driven.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
public class BaseCharacterController : MonoBehaviour
{
    [Header("Collider Settings")]
    [Tooltip("Enable to use the character's collider for ground detection and movement.")]
    [CharacterConfigContext("StepHeightRatio")][SerializeField][Range(0.0f, 1.0f)] protected float _stepHeightRatio = 0.1f; // Ratio of collider height to step height
    [CharacterConfigContext("ColliderHeight")][SerializeField] protected float _colliderHeight = 2f;
    [CharacterConfigContext("ColliderThickness")][SerializeField] protected float _colliderThickness = 1f;
    [CharacterConfigContext("ColliderOffset")][SerializeField] protected Vector3 _colliderOffset = Vector3.zero;

    [Header("Gravity Settings")]
    [CharacterConfigContext("GravityScale")][SerializeField] protected float _gravityScale = 1f; // Scale for gravity effect
    [CharacterConfigContext("TerminalVelocity")][SerializeField] protected float _terminalVelocity = 15f; // Maximum falling speed

    [Header("Ground Settings")]
    [CharacterConfigContext("MaxGroundCheckDistance")][SerializeField] protected float _maxGroundCheckDistance = 5f; // Maximum distance for ground checks
    [CharacterConfigContext("GroundedTolerance")][SerializeField] protected float _groundedTolerance = 0.1f; // Tolerance for being considered grounded

    [Header("Movement Settings")]
    [CharacterConfigContext("MovementSpeed")][SerializeField] protected float _movementSpeed = 5f; // Base movement speed
    [CharacterConfigContext("JumpForce")][SerializeField] protected float _jumpForce = 10f; // Force applied when jumping

    [Header("Debug Settings")]
    [CharacterConfigContext("DebugMode")][SerializeField] protected bool _debugMode = false; // Enable debug mode for additional logging and visual aids

    protected int _currentLayer;
    protected CharacterContext _context;
    protected PlayerInputProvider _input;
    protected CharacterMotor _motor;
    protected IAdaptiveStateMachine<CharacterContext> _stateMachine;
    protected ISensorManager<CharacterContext> _sensorManager;

    public bool IsPlayerControlled = false;
    public bool IsLocal = true; // Can be used for multiplayer authority

    // --- Properties ---
    public CharacterContext GetContext() => _context;

    protected virtual void OnValidate()
    {
        if (!gameObject.activeInHierarchy) return;

        RecalculateColliderDimensions();
    }

    void RecalculateColliderDimensions()
    {
        if (_context == null)
        {
            _context = new CharacterContext();
            _context.Initialize(gameObject);
        }

        CapsuleCollider collider = _context.References.Collider; // define local variable for clarity
        collider.height = _colliderHeight * (1f - _stepHeightRatio);
        collider.radius = _colliderThickness * 0.5f;
        collider.center = _colliderOffset * _colliderHeight + new Vector3(0f, (_colliderHeight - collider.height) * 0.5f, 0f);

        if (collider.height * 0.5f < collider.radius)
        {
            collider.radius = collider.height * 0.5f; // Ensure radius is not larger than half height
            Logwin.LogWarning("[BaseCharacterController]", "Collider radius adjusted to prevent overlap with height.");
        }

        const float safetyDistanceFactor = 0.001f;  // Safety factor to avoid precision issues
        float length = _colliderHeight * (1f - _stepHeightRatio) * 0.5f + _colliderHeight * safetyDistanceFactor;
        _context.Sensor.BaseSensorRange = length * (1f + safetyDistanceFactor) * transform.lossyScale.x;
        _context.Sensor.BaseCastLength = length * transform.lossyScale.x;
        _context.Sensor.CastOrigin = collider.bounds.center;

        //update layer mask for sensors
        int objectLayer = gameObject.layer;
        int layerMask = Physics.AllLayers;
        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

        layerMask &= ~(1 << ignoreRaycastLayer); // Clear Ignore Raycast layer from mask
        for (int i = 0; i < 32; i++)
        {
            if (Physics.GetIgnoreLayerCollision(objectLayer, i))
            {
                layerMask &= ~(1 << i); // Clear the bit for this layer
            }
        }

        _currentLayer = objectLayer;
        _context.Sensor.ExcludeLayers = layerMask;
        Logwin.Log("[BaseCharacterController]", $"Collider dimensions recalculated: Height={collider.height}, Radius={collider.radius}, Center={collider.center}");
    }

    protected virtual void OnEnable()
    {
        _context = new CharacterContext();
        _context.Initialize(gameObject);
        RecalculateColliderDimensions();


        _input = new PlayerInputProvider();
        _input?.Initialize(_context);

        _motor = new CharacterMotor();
        _motor?.Initialize(_context);

        SetupSensors(_context);
        _sensorManager.Initialize(_context);
        _sensorManager?.UpdateSensors(_context); // Update sensors immediately to get initial ground state before state machine starts

        SetupStateMachine(_context);
        _stateMachine?.Initialize(_context);
    }

    protected virtual void OnDisable()
    {
        // Cleanup handled by Unity lifecycle
        _input?.Dispose();
        _sensorManager?.Dispose();
        _stateMachine?.Dispose();
        _motor?.Dispose();
        _context.Dispose();
    }

    protected virtual void Awake() { }
    protected virtual void Start()
    {
        // Debug: Check if ground detection is working
        Logwin.Log("CharacterController", $"After sensor update - IsGrounded: {_context.Sensor.IsGrounded}, Position: {transform.position}");
    }

    protected virtual void SetupStateMachine(CharacterContext context)
    {
        // Subclass responsibility
    }

    protected virtual void SetupSensors(CharacterContext context)
    {
        // Subclass responsibility
    }

    protected virtual void Update()
    {
        if (_currentLayer != gameObject.layer)
        {
            RecalculateColliderDimensions(); // Ensure collider dimensions are up-to-date
        }

        _context.Config.DebugMode = _debugMode;

        _input?.UpdateInput(_context);
        _sensorManager?.UpdateSensors(_context);

        _context.Intent.ResetFrameContext();
        _stateMachine?.Update(_context);
    }

    protected virtual void FixedUpdate()
    {
        _stateMachine?.FixedUpdate(_context);
        _motor?.ApplyMotion(_context);
    }

    protected virtual void LateUpdate()
    {
        _stateMachine?.LateUpdate(_context);
    }

}
