using UnityEngine;

public interface ICharacterReferences
{
    GameObject GameObject { get; }
    Transform Transform { get; }
    Camera Camera { get; }
    Animator Animator { get; set; }
    Transform CameraTransform { get; set; }
    int ExcludeLayers { get; }
    void Initialize(GameObject owner);
}