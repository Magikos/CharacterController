using UnityEngine;

public class TestColliderSpammer : MonoBehaviour
{
    public float testDistance = 1.0f;
    public float capsuleRadius = 0.25f;
    public float capsuleHeight = 1.8f;
    public LayerMask testLayers = ~0;

    private void Update()
    {

        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * 1f, Color.yellow);
        Debug.Log(Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 1f));


        Vector3 origin = transform.position;
        Vector3 center = origin + Vector3.up * (capsuleHeight / 2f);

        Vector3 top = center + Vector3.up * (capsuleHeight / 2f - capsuleRadius);
        Vector3 bottom = center - Vector3.up * (capsuleHeight / 2f - capsuleRadius);

        Debug.DrawLine(origin, origin + Vector3.down * testDistance, Color.yellow);
        Debug.DrawLine(top, top + Vector3.down * testDistance, Color.cyan);
        Debug.DrawLine(bottom, bottom + Vector3.down * testDistance, Color.magenta);

        // SphereCast
        if (Physics.SphereCast(origin, capsuleRadius, Vector3.down, out RaycastHit hitSphere, testDistance, testLayers))
            Debug.Log($"[SphereCast] Hit at {hitSphere.point}");

        // Raycast
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hitRay, testDistance, testLayers))
            Debug.Log($"[Raycast] Hit at {hitRay.point}");

        // CapsuleCast
        if (Physics.CapsuleCast(top, bottom, capsuleRadius, Vector3.down, out RaycastHit hitCapsule, testDistance, testLayers))
            Debug.Log($"[CapsuleCast] Hit at {hitCapsule.point}");
    }
}
