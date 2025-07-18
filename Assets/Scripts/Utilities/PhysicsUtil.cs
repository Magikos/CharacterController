using UnityEngine;

public static class PhysicsUtil
{
    public static void GetCapsuleWorldPoints(CapsuleCollider collider, out Vector3 p1, out Vector3 p2, out float radius)
    {
        Transform t = collider.transform;
        radius = collider.radius * Mathf.Max(t.lossyScale.x, t.lossyScale.z);
        float height = Mathf.Max(collider.height * t.lossyScale.y, radius * 2f);
        Vector3 center = t.TransformPoint(collider.center);
        Vector3 axis = t.up;
        float offset = (height / 2f) - radius;
        p1 = center + axis * offset;
        p2 = center - axis * offset;
    }
}