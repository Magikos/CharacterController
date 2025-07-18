using UnityEngine;

public class CharacterMotor
{
    // Motion settings
    private float gravity = -9.81f;
    private float terminalVelocity = -15f;

    // Slope/slide settings
    private float maxWalkableAngle = 45f;
    private float slideAngle = 60f;
    private AnimationCurve slopeSpeedCurve = AnimationCurve.Linear(0f, 1f, 45f, 0.5f);
    private float slopeFrictionMultiplier = 0.8f;
    private float slideSpeed = 8f;

    // Step/penetration settings
    private float maxStepHeight = 0.3f;
    private float skinWidth = 0.01f;

    private Transform _transform;
    private Rigidbody _rigidbody;
    private CharacterContext _context;

    public void Initialize(CharacterContext context)
    {
        //cache references
        _transform = context.References.Transform;

        _rigidbody = context.References.Rigidbody;
        //_rigidbody.isKinematic = true;
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = false;
        //_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        //_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        _context = context;
        _context.Motor.CurrentVelocity = Vector3.zero;
        _context.Motor.Gravity = gravity;
    }

    public void Dispose()
    {
        _rigidbody = null;
        _context = null;
    }

    public void ApplyMotion(CharacterContext context)
    {

    }

    public Vector3 GetMomentum(CharacterContext context) => context.Config.UseLocalMomentum ? _transform.localToWorldMatrix * context.Motor.CurrentVelocity : context.Motor.CurrentVelocity;
    public void SetVelocity(CharacterContext context, Vector3 velocity) => _rigidbody.linearVelocity = velocity + context.Sensor.GroundAdjustmentVelocity;
}