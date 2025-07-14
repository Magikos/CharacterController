using UnityEngine;

public class GroundSensor : BaseSensor<CharacterContext>
{
    private float _groundCheckDistance = 0.3f;

    public override SensorUpdateMode DefaultMode => SensorUpdateMode.Reduced;

    public override void UpdateSensor(CharacterContext context)
    {

        Vector3 castStart = _colliderCenter - Vector3.up * (_colliderHalfHeight - _sensorRadius);

        int layerMask = ~excludeLayers.value;
        int hitCount = Physics.SphereCastNonAlloc(
            castStart, _sensorRadius, Vector3.down, _hits,
            _groundCheckDistance, layerMask, QueryTriggerInteraction.Ignore
        );

        // Find closest valid hit
        bool grounded = false;
        RaycastHit closestHit = new RaycastHit();
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            if (_selfColliders.Contains(_hits[i].collider)) continue;

            if (_hits[i].distance < closestDistance)
            {
                closestDistance = _hits[i].distance;
                closestHit = _hits[i];
                grounded = true;
            }
        }

        // Update ground state
        bool wasGrounded = context.Sensor.IsGrounded;
        context.Sensor.IsGrounded = grounded;

        if (grounded)
        {
            context.Sensor.TimeSinceLastGrounded = 0f;
            context.Sensor.LastGroundedTime = Time.time;
            context.Sensor.LastGroundedHeight = context.References.Transform.position.y;
            context.Sensor.GroundNormal = closestHit.normal;
            context.Sensor.GroundContactPoint = closestHit.point;
            context.Sensor.DesiredGroundPosition = new Vector3(context.References.Transform.position.x, closestHit.point.y, context.References.Transform.position.z);
        }
        else
        {
            context.Sensor.TimeSinceLastGrounded += context.DeltaTime;
        }

        if (wasGrounded != grounded) Logwin.Log("GroundSensor", $"Ground state changed: {wasGrounded} -> {grounded}");
        if (grounded) Logwin.Log("GroundSensor", $"Ground hit: {closestHit.collider.name}, Distance: {closestHit.distance:F3}, Normal: {closestHit.normal}", grounded ? LogwinParam.Color(Color.green) : LogwinParam.Color(Color.red));
    }
}