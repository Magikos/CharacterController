using UnityEngine;

/// <summary>
/// The base controller that owns all character motion systems.
/// Designed for both players and AI, modular, generic, and context-driven.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
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

    protected virtual void OnEnable()
    {
        _context = new TContext();
        _context.Initialize(gameObject);

        _input = new PlayerInputProvider();
        _input?.Initialize(_context);

        _motor = new CharacterMotor();
        _motor?.Initialize(_context);

        InitializeSensors(_context);

        _sensorManager?.UpdateSensors(_context); // Update sensors immediately to get initial ground state before state machine starts
        InitializeStateMachine(_context);
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

    protected virtual void InitializeStateMachine(TContext context)
    {
        // Subclass responsibility
    }

    protected virtual void InitializeSensors(TContext context)
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
        _motor?.ApplyMotion(_context);
    }

    protected virtual void LateUpdate()
    {
        _stateMachine?.LateUpdate(_context);
    }

}
