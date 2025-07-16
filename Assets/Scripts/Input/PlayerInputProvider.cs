using UnityEngine;

public class PlayerInputProvider
{
    private PlayerControls _controls;
    private Transform _cameraTransform;

    public void Initialize(CharacterContext context)
    {
        _controls = new PlayerControls();
        _controls.Character.Enable();

        _cameraTransform = Camera.main.transform;
        context.References.CameraTransform = _cameraTransform;
    }

    public void Dispose()
    {
        _controls.Character.Disable();
        _controls.Dispose();
    }

    public void UpdateInput(CharacterContext context)
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
