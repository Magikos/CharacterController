using NUnit.Framework;
using UnityEngine;

public abstract class RaycastSensor : ISensor<CharacterContext>
{
    public enum CastDirection
    {
        Forward,
        Backward,
        Up,
        Down,
        Left,
        Right
    }

    public abstract SensorUpdateMode DefaultMode { get; }
    public LayerMask ExcludeLayers = 255;

    protected Transform _transform;
    protected CastDirection _direction;
    protected RaycastHit _hit;

    public virtual void Dispose()
    {
        // Clean up resources if necessary
        _transform = null;
        _hit = default;
    }

    public virtual void Initialize(CharacterContext context)
    {
        _transform = context.References.Transform;
        _direction = CastDirection.Down;
    }

    public virtual void UpdateSensor(CharacterContext context)
    {
        Vector3 worldOrigin = _transform.TransformPoint(context.Sensor.CastOrigin);
        Vector3 castDirection = GetCastDirection();
        if (Physics.Raycast(worldOrigin, castDirection, out _hit, context.Sensor.BaseCastLength, ~ExcludeLayers, QueryTriggerInteraction.Ignore))
        {
            // Handle hit
            OnHit(_hit);
        }
    }

    protected virtual void OnHit(RaycastHit hit) { }

    public virtual void SetCastDirection(CastDirection direction) => _direction = direction;

    public virtual bool IsHit() => _hit.collider != null;
    public virtual float GetHitDistance() => _hit.distance;
    public virtual Vector3 GetNormal() => _hit.normal;
    public virtual Vector3 GetPosition() => _hit.point;
    public virtual Collider GetCollider() => _hit.collider;
    public virtual Transform GetTransform() => _hit.transform;

    protected virtual Vector3 GetCastDirection()
    {
        return _direction switch
        {
            CastDirection.Forward => _transform.forward,
            CastDirection.Backward => -_transform.forward,
            CastDirection.Up => _transform.up,
            CastDirection.Down => -_transform.up,
            CastDirection.Left => -_transform.right,
            CastDirection.Right => _transform.right,
            _ => throw new System.NotImplementedException()
        };
    }
}