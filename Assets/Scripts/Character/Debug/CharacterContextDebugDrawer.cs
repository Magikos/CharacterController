using UnityEngine;
using CryingOnion.Tools.Runtime;
using System; // Oh My Gizmos namespace


public class CharacterContextDebugDrawer : IDisposable
{
    // Unique IDs for Gizmos
    private readonly Guid _characterColliderId = Guid.NewGuid();
    private readonly Guid _rigidbodyVelocityId = Guid.NewGuid();
    private readonly Guid _stepHeightId = Guid.NewGuid();
    private readonly Guid _desiredVelocityId = Guid.NewGuid();

    // Colors
    private readonly Gradient _yellowGradient;
    private readonly Color _redColor = new Color(1f, 0f, 0f, 0.25f);
    private readonly Color _cyanColor = new Color(0f, 1f, 1f, 0.25f);

    // Cached references
    private ColliderCacheData _colliderCache;

    public bool Enabled { get { return OhMyGizmos.Enabled; } set { OhMyGizmos.Enabled = value; } }

    public CharacterContextDebugDrawer()
    {
        _yellowGradient = new Gradient();
        _yellowGradient.colorKeys = new GradientColorKey[] {
            new GradientColorKey(Color.yellow, 0f),
            new GradientColorKey(Color.yellow, 1f)
        };
        _yellowGradient.alphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        };
    }

    public void Initialize(CharacterContext context)
    {
        if (context == null) return;

        //cache calculations that don't change often
        _colliderCache = new ColliderCacheData(context.References.Collider);
        if (_colliderCache.Collider == null)
        {
            Debug.LogWarning("CharacterContextDebugDrawer: CapsuleCollider is null. Ensure the context has a valid collider.");
            return;
        }
    }

    private class ColliderCacheData
    {
        public CapsuleCollider Collider;
        public Vector3 WorldCenter;
        public float Height;
        public float Radius;

        public ColliderCacheData(CapsuleCollider collider)
        {
            Collider = collider;
            Update();
        }

        public void Update()
        {
            if (Collider == null) return;

            WorldCenter = Collider.transform.TransformPoint(Collider.center);
            Height = Collider.height * Collider.transform.lossyScale.y;
            Radius = Collider.radius * Mathf.Max(Collider.transform.lossyScale.x, Collider.transform.lossyScale.z);
        }
    }


    public void Update(CharacterContext context)
    {
        if (context == null) return;
        if (_colliderCache == null) Initialize(context);

        _colliderCache.Update();

        // Calculate the world position of the collider's center
        _colliderCache.WorldCenter = _colliderCache.Collider.transform.TransformPoint(_colliderCache.Collider.center);
        Matrix4x4 matrix = Matrix4x4.TRS(_colliderCache.WorldCenter, _colliderCache.Collider.transform.rotation, _colliderCache.Collider.transform.lossyScale);

        // Collider overlap visualization (red if overlapping, cyan otherwise)
        Color capsuleColor = context.Sensor.IsColliderOverlapping ? _redColor : _cyanColor;
        OhMyGizmos.Capsule(
            _characterColliderId,
            matrix,
            capsuleColor,
            _colliderCache.Height / 2f, // halve the height
            _colliderCache.Radius
        );


        // Possible other visualizations: 

        // // Step height indicator (horizontal line at max step height above feet)        
        // OhMyGizmos.Lines(_stepHeightId, new System.Collections.Generic.List<Vector3> {  //todo: don't calculate this every frame
        //             stepPos - _collider.transform.right * lineLength * 0.5f,
        //             stepPos + _collider.transform.right * lineLength * 0.5f
        //         }, Color.magenta);



        // // Draw a sphere at the ground check position, colored by IsGrounded
        // Color groundColor = context.Sensor.IsGrounded ? Color.green : Color.red;
        // OhMyGizmos.Sphere(context.Sensor.GroundPosition, 0.1f, groundColor);

        // // 1. Ground normal visualization
        // if (context.Sensor.IsGrounded)
        // {
        //     OhMyGizmos.Arrow(
        //         Guid.NewGuid(),
        //         context.Sensor.GroundPosition,
        //         context.Sensor.GroundNormal,
        //         0.05f,
        //         0.5f,
        //         Color.blue //todo: use a consistent color scheme
        //     );
        // }

        // // 3. Sensor/probe rays (if available)
        // if (context.Sensor.ProbeOrigins != null && context.Sensor.ProbeDirections != null)
        // {
        //     for (int i = 0; i < context.Sensor.ProbeOrigins.Length; i++)
        //     {
        //         Vector3 origin = context.Sensor.ProbeOrigins[i];
        //         Vector3 dir = context.Sensor.ProbeDirections[i];
        //         OhMyGizmos.Lines(Guid.NewGuid(), new System.Collections.Generic.List<Vector3> {
        //                 origin,
        //                 origin + dir * 1.0f // scale as needed
        //             }, Color.cyan); //todo: use a consistent color scheme
        //     }
        // }

        // // Draw desired velocity vector as an arrow (magenta)
        // if (context.Motor != null && context.Intent.DesiredVelocity != null)
        // {
        //     Vector3 desired = context.Intent.DesiredVelocity;
        //     OhMyGizmos.Arrow(
        //         _desiredVelocityId,
        //         context.References.Transform.position,
        //         desired.normalized,
        //         0.1f,
        //         desired.magnitude,
        //         Color.magenta //todo: use a consistent color scheme
        //     );
        // }

        // // Draw velocity vector as an arrow (yellow)
        // var rb = context.References?.Rigidbody;
        // if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        // {
        //     OhMyGizmos.Arrow(
        //         _rigidbodyVelocityId,
        //         rb.position,
        //         rb.linearVelocity.normalized,
        //         0.1f,
        //         rb.linearVelocity.magnitude,
        //         _yellowGradient
        //     );
        // }

        // // 4. Movement input vector (if available)
        // if (context.Input != null && context.Input.Move != Vector2.zero)
        // {
        //     Vector3 inputDir = new Vector3(context.Input.Move.x, 0, context.Input.Move.y);
        //     Vector3 start = rb != null ? rb.position : (_collider != null ? _collider.transform.position : Vector3.zero);
        //     OhMyGizmos.Arrow(
        //         Guid.NewGuid(),
        //         start,
        //         inputDir.normalized,
        //         0.1f,
        //         inputDir.magnitude,
        //         Color.green
        //     );
        // }

        // // 8. Jump arc prediction (simple ballistic arc)
        // if (context.Motor != null && context.Motor.CanJump && rb != null)
        // {
        //     Vector3 jumpStart = rb.position;
        //     Vector3 jumpVel = context.Motor.JumpVelocity;
        //     float gravity = Physics.gravity.y;
        //     int steps = 20;
        //     float dt = 0.1f;
        //     var arcPoints = new System.Collections.Generic.List<Vector3>();
        //     for (int i = 0; i < steps; i++)
        //     {
        //         float t = i * dt;
        //         Vector3 pos = jumpStart + jumpVel * t + 0.5f * new Vector3(0, gravity, 0) * t * t;
        //         arcPoints.Add(pos);
        //     }
        //     OhMyGizmos.Lines(Guid.NewGuid(), arcPoints, Color.yellow);
        // }
    }

    public void Dispose()
    {
        // Cleanup if necessary        
    }
}
