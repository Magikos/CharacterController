using UnityEngine;
using System.Collections.Generic;

public abstract class BaseSensor : ISensor<CharacterContext>
{
    public abstract SensorUpdateMode DefaultMode { get; }

    protected HashSet<Collider> _selfColliders;
    protected RaycastHit[] _hits = new RaycastHit[10];
    protected Vector3 _colliderCenter;
    protected float _sensorRadius;
    protected float _colliderHalfHeight;

    public abstract void UpdateSensor(CharacterContext context);
    public virtual void Initialize(CharacterContext context)
    {
        // Initialize sensor components, colliders, etc.        
        if (context.References.Collider == null)
        {
            Debug.LogError("BaseSensor requires a CapsuleCollider component on the character.");
            return;
        }

        InitializeSelfColliders(context);
        InitializeColliderVariables(context);
    }

    protected void InitializeColliderVariables(CharacterContext context)
    {
        _colliderCenter = context.References.Transform.position + context.References.Collider.center;
        _colliderHalfHeight = context.References.Collider.height / 2f;
        _sensorRadius = context.References.Collider.radius * 0.95f;

    }

    private void InitializeSelfColliders(CharacterContext context)
    {
        _selfColliders = new HashSet<Collider>();
        var colliders = context.References.GameObject.GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
        {
            _selfColliders.Add(col);
        }

        Logwin.Log("IntegratedSensor", $"Initialized {_selfColliders.Count} self-colliders for exclusion");
    }

    public virtual void Dispose()
    {
        // Clean up resources if necessary
        _selfColliders.Clear();
        _hits = null;
    }
}
