using UnityEngine;
/// <summary>
/// Subcontext that holds all input-related fields for clarity and modularity.
/// </summary>
public class CharacterInputContext : ICharacterInputContext
{
    public Vector2 MoveInput { get; set; }
    public Vector3 MoveDirection { get; set; }
    public float MoveInputMagnitude { get; set; }

    public bool IsJumpPressed { get; set; }
    public bool IsSprintPressed { get; set; }
    public bool IsCrouchPressed { get; set; }
    public bool ShouldRotateToMoveDirection { get; set; } = true;

    public void Initialize(GameObject owner)
    {
        // Initialize any references or state here
    }

    public void Dispose()
    {
        // Clean up any resources if necessary
    }

    public void ResetFrameContext()
    {
        MoveInput = Vector2.zero;
        MoveDirection = Vector3.zero;
        MoveInputMagnitude = 0f;

        IsJumpPressed = false;
        IsSprintPressed = false;
        IsCrouchPressed = false;

        ShouldRotateToMoveDirection = true;
    }
}
