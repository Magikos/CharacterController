using UnityEngine;

/// <summary>
/// The base controller that owns all character motion systems.
/// Designed for both players and AI, modular, generic, and context-driven.
/// </summary>
[RequireComponent(typeof(Animator))]
public class BaseCharacterController<TContext> : MonoBehaviour where TContext : class, ICharacterContext, new()
{
    // --- Shared Context ---
    protected TContext _context;

    // --- Injected Modules ---
    protected IInputProvider _input;
    protected ICharacterMotor _motor;

    // --- Manager Systems
    protected IAdaptiveStateMachine<TContext> _stateMachine;
    protected ISensorManager<TContext> _sensorManager;

    // --- Flags ---
    public bool IsPlayerControlled = false;
    public bool IsLocal = true; // Can be used for multiplayer authority

    // --- Properties ---
    public TContext GetContext() => _context;

    protected virtual void Awake()
    {
        var animator = GetComponent<Animator>();
        _input = GetComponent<IInputProvider>();
        _motor = GetComponent<ICharacterMotor>();

        _context = new TContext();
        _context.Initialize(gameObject, animator);

        _input?.Initialize(_context);
        _motor?.Initialize(_context);
    }

    protected virtual void Start()
    {
        InitializeSensors();

        // Initialize sensors first
        _sensorManager?.Initialize(_context);

        // Update sensors immediately to get initial ground state before state machine starts
        _sensorManager?.UpdateSensors(_context);

        // Debug: Check if ground detection is working
        Logwin.Log("CharacterController", $"After sensor update - IsGrounded: {_context.Sensor.IsGrounded}, Position: {transform.position}");

        InitializeStateMachine();

        // Debug: Check state machine initial state
        Logwin.Log("CharacterController", $"State machine initialized, IsGrounded: {_context.Sensor.IsGrounded}");
    }

    protected virtual void InitializeStateMachine()
    {
        // Subclass responsibility
    }

    protected virtual void InitializeSensors()
    {
        // Subclass responsibility
    }

    protected virtual void Update()
    {
        _context.DeltaTime = Time.deltaTime;

        _input?.UpdateInput(_context);
        _sensorManager?.UpdateSensors(_context);

        _context.Intent.ResetFrameContext();
        _stateMachine?.Update(_context);
    }

    protected virtual void FixedUpdate()
    {
        _context.FixedDeltaTime = Time.fixedDeltaTime;

        _stateMachine?.FixedUpdate(_context);

#if UNITY_EDITOR && DEBUG
        // Validate that the current state set appropriate intent (debug only)
        if (_context.Intent is CharacterIntentContext intentContext)
        {
            intentContext.ValidateIntentWasSet();
        }
#endif

        _motor?.ApplyMotion(_context);
    }

    protected virtual void LateUpdate()
    {
        _stateMachine?.LateUpdate(_context);
    }

    protected virtual void OnDestroy()
    {
        // Cleanup handled by Unity lifecycle
    }
}
