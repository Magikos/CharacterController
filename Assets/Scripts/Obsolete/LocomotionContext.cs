// using UnityEngine;

// public class LocomotionContext
// {
//     public Animator Animator { get; internal set; }
//     public CharacterController Controller { get; internal set; }
//     public ICharacterStats Stats { get; internal set; } = null!;
//     public Vector3 MoveDirection { get; internal set; }
//     public float MoveSpeed { get; internal set; }
//     public float MoveInputMagnitude { get; internal set; }
//     public bool IsSprintPressed { get; internal set; }
//     public bool IsGrounded { get; internal set; }
//     public float DeltaTime { get; internal set; }
//     public float VerticalVelocity { get; internal set; }
//     public float JumpHeight { get; internal set; }
//     public bool IsJumpPressed { get; internal set; }
//     public float FixedDeltaTime { get; internal set; }
//     public float StartFallHeight { get; internal set; }
//     public float EndFallHeight { get; internal set; }
//     public float FallDistance => Mathf.Abs(EndFallHeight - StartFallHeight);

//     // Coyote Time
//     public float TimeSinceLastGrounded { get; internal set; }
// }
