using UnityEngine;

/// <summary>
/// Input provider for player-controlled characters using the Unity Input System.
/// Converts raw input into world-relative movement intent.
/// </summary>
[RequireComponent(typeof(PlayerControls))]
public class PlayerInputProvider : MonoBehaviour, IInputProvider
{
    private PlayerControls _controls;
    private Transform _cameraTransform;

    public void Initialize(ICharacterContext context)
    {
        _controls = new PlayerControls();
        _controls.Character.Enable();

        _cameraTransform = Camera.main.transform;
        context.CameraTransform = _cameraTransform;
    }

    public void Destroy()
    {
        Dispose();
    }

    public void Dispose()
    {
        _controls.Character.Disable();
        _controls.Dispose();
    }

    public void UpdateInput(ICharacterContext context)
    {
        Vector2 input = _controls.Character.Move.ReadValue<Vector2>();

        // Camera-relative movement
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * input.y + camRight * input.x;

        context.Input.MoveInput = input;
        context.Input.MoveInputMagnitude = input.magnitude;
        context.Input.MoveDirection = moveDir;

        context.Input.IsJumpPressed = _controls.Character.Jump.IsPressed();
        context.Input.IsSprintPressed = _controls.Character.Sprint.IsPressed();
        context.Input.IsCrouchPressed = false; // Add if you bind a crouch action
    }
}
