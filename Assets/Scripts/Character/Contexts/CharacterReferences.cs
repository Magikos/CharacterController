using UnityEngine;

internal class CharacterReferences : ICharacterReferences
{
    public GameObject GameObject { get; private set; }
    public Transform Transform { get; private set; }
    public int ExcludeLayers { get; private set; }
    public Camera Camera { get; private set; }
    public Transform CameraTransform { get; set; }
    public Animator Animator { get; set; }

    public void Initialize(GameObject owner)
    {
        GameObject = owner;
        Transform = owner.transform;
        ExcludeLayers = LayerMask.GetMask("UI", "Ignore Raycast");

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
    }

    public void Dispose()
    {
        // Clean up any resources if necessary
    }
}