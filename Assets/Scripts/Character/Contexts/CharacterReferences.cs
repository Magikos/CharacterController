using UnityEngine;

public class CharacterReferences
{
    public GameObject GameObject { get; private set; }
    public Transform Transform { get; private set; }
    public Camera Camera { get; private set; }
    public Transform CameraTransform { get; set; }
    public Animator Animator { get; set; }
    public Rigidbody Rigidbody { get; set; }
    public CapsuleCollider Collider { get; set; }

    public void Initialize(GameObject owner)
    {
        GameObject = owner;
        Transform = owner.transform;

        Camera = Camera.main;
        CameraTransform = Camera?.transform;
        if (CameraTransform == null)
        {
            Debug.LogWarning("No main camera found. CameraTransform will be null.");
        }

        Animator = owner.GetComponent<Animator>();
        if (Animator == null)
        {
            Debug.LogWarning("Animator component not found on the character. Animation states may not work correctly.");
        }

        Rigidbody = owner.GetComponent<Rigidbody>();
        if (Rigidbody == null)
        {
            Debug.LogWarning("Rigidbody component not found on the character. Physics interactions may not work correctly.");
        }

        Collider = owner.GetComponent<CapsuleCollider>();
        if (Collider == null)
        {
            Debug.LogWarning("CapsuleCollider component not found on the character. Collision detection may not work correctly.");
        }
    }

    public void Dispose()
    {
        // Clean up any resources if necessary
    }
}