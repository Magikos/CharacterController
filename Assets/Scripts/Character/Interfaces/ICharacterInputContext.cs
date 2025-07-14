using UnityEngine;
using System;

public interface ICharacterInputContext : IDisposable
{
    Vector2 MoveInput { get; set; }
    Vector3 MoveDirection { get; set; }
    float MoveInputMagnitude { get; set; }

    bool IsJumpPressed { get; set; }
    bool IsSprintPressed { get; set; }
    bool IsCrouchPressed { get; set; }
    bool ShouldRotateToMoveDirection { get; set; }

    void Initialize(GameObject owner);
    void ResetFrameContext();
}
