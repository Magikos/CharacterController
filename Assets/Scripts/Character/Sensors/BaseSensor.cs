using UnityEngine;
using System.Collections.Generic;

public abstract class BaseSensor<TContext> : ISensor<TContext> where TContext : ICharacterContext
{
    public abstract SensorUpdateMode DefaultMode { get; }

    protected LayerMask excludeLayers;
    protected CapsuleCollider _collider;
    protected HashSet<Collider> _selfColliders;
    protected RaycastHit[] _hits = new RaycastHit[10];
    protected Vector3 _colliderCenter;
    protected float _sensorRadius;
    protected float _colliderHalfHeight;

    public abstract void UpdateSensor(TContext context);
    public virtual void Initialize(TContext context)
    {
        // Initialize sensor components, colliders, etc.
        _collider = context.References.Transform.GetComponent<CapsuleCollider>();
        if (_collider == null)
        {
            Debug.LogError("BaseSensor requires a CapsuleCollider component on the character.");
            return;
        }

        excludeLayers = context.References.ExcludeLayers;
        InitializeSelfColliders(context);
        InitializeColliderVariables(context);
    }

    protected void InitializeColliderVariables(TContext context)
    {
        _colliderCenter = context.References.Transform.position + _collider.center;
        _colliderHalfHeight = _collider.height / 2f;
        _sensorRadius = _collider.radius * 0.95f;



    }

    private void InitializeSelfColliders(TContext context)
    {
        _selfColliders = new HashSet<Collider>();
        var colliders = context.References.GameObject.GetComponentsInChildren<Collider>();

        foreach (var col in colliders)
        {
            _selfColliders.Add(col);
        }

        Logwin.Log("IntegratedSensor", $"Initialized {_selfColliders.Count} self-colliders for exclusion");
    }

}
