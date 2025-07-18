using System.Linq;
using UnityEngine;

public class ColliderOverlappingSensor : ISensor<CharacterContext>
{
    public SensorUpdateMode DefaultMode => SensorUpdateMode.Reduced;

    public void Dispose()
    {
    }

    public void Initialize(CharacterContext context)
    {

    }

    public void UpdateSensor(CharacterContext context)
    {
        CheckOverlaps(context);
    }

    private void CheckOverlaps(CharacterContext context)
    {
        var collider = context.References?.Collider;
        if (collider != null)
        {
            Vector3 point1, point2;
            float radius;

            // Calculate capsule endpoints in world space
            PhysicsUtil.GetCapsuleWorldPoints(collider, out point1, out point2, out radius);

            // Check for overlaps (excluding the character's own collider)
            Collider[] overlaps = Physics.OverlapCapsule(point1, point2, radius, ~0, QueryTriggerInteraction.Ignore);
            context.Sensor.IsColliderOverlapping = overlaps.Any(c => c != collider);
        }
    }
}