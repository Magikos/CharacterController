using UnityEngine;

public class GroundSensor : RaycastSensor
{
    public override SensorUpdateMode DefaultMode => SensorUpdateMode.Reduced;

    public override void UpdateSensor(CharacterContext context)
    {
        context.Sensor.GroundAdjustmentVelocity = Vector3.zero;
        context.Sensor.BaseCastLength = context.Config.DebugMode ? context.Config.MaxGroundCheckDistance : context.Sensor.BaseCastLength;
        base.UpdateSensor(context);

        if (!IsHit()) return;

        context.Sensor.GroundDistance = GetHitDistance();
        var upperLimit = context.References.Collider.height * _transform.localScale.x * (1f - context.Config.StepHeightRatio) * 0.5f;
        var middle = upperLimit + context.Config.ColliderHeight * _transform.localScale.x * context.Config.StepHeightRatio;
        var distanceToGo = middle - context.Sensor.GroundDistance;

        context.Sensor.GroundAdjustmentVelocity = _transform.up * (distanceToGo / Time.fixedDeltaTime);
        context.Sensor.IsGrounded = context.Sensor.GroundDistance <= context.Config.GroundedTolerance;
        context.Sensor.GroundNormal = GetNormal();
        context.Sensor.GroundContactPoint = GetPosition();
        context.Sensor.GroundPosition = new Vector3(_transform.position.x, GetPosition().y, _transform.position.z);



        /* old code
        // Calculate feet position in world space
        Vector3 feetPosition = context.References.Transform.position + context.References.Transform.rotation * (context.References.Collider.center - Vector3.up * (context.References.Collider.height / 2f - context.References.Collider.radius));
        context.Sensor.FeetPosition = feetPosition;

        int layerMask = context.Sensor.ExcludeLayers;
        float radius = context.References.Collider.radius * 0.95f;

        // Use SphereCastNonAlloc for performance
        int hitCount = Physics.SphereCastNonAlloc(
            feetPosition + Vector3.up * 0.05f,
            radius,
            Vector3.down,
            _hits,
            maxGroundCheckDistance,
            layerMask,
            QueryTriggerInteraction.Ignore
        );

        bool hitSomething = false;
        RaycastHit closestHit = new RaycastHit();
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            if (_selfColliders.Contains(_hits[i].collider)) continue;
            float distance = Mathf.Abs(feetPosition.y - _hits[i].point.y);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHit = _hits[i];
                hitSomething = true;
            }
        }

        context.Sensor.GroundDistance = closestDistance;
        context.Sensor.IsGrounded = hitSomething && closestDistance <= groundedTolerance;

        if (hitSomething)
        {
            context.Sensor.GroundNormal = closestHit.normal;
            context.Sensor.GroundContactPoint = closestHit.point;
            context.Sensor.GroundPosition = new Vector3(
                context.References.Transform.position.x,
                closestHit.point.y,
                context.References.Transform.position.z
            );
            if (context.Sensor.IsGrounded)
            {
                context.Sensor.TimeSinceLastGrounded = 0f;
                context.Sensor.LastGroundedTime = Time.time;
                context.Sensor.LastGroundedHeight = context.References.Transform.position.y;
            }
            else
            {
                context.Sensor.TimeSinceLastGrounded += context.DeltaTime;
            }
        }
        else
        {
            context.Sensor.IsGrounded = false;
            context.Sensor.GroundDistance = float.MaxValue;
            context.Sensor.TimeSinceLastGrounded += context.DeltaTime;
        }
        */

    }
}