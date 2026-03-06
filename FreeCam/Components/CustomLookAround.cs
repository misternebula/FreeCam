using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam.Components;

public class CustomLookAround : MonoBehaviour
{
    private float _degreesX;
    private float _degreesY;
    private float _moveX;
    private float _moveY;
    private float _moveZ;

    private float _moveSpeed = 5f;
    public float MoveSpeed {
        get { return _moveSpeed; }
    }

    void Start() => Cursor.lockState = CursorLockMode.Locked;

    void Update()
    {
        if (OWInput.GetInputMode() != InputMode.None)
        {
            return;
        }

        var scrollInOut =
            Math.Max(-1f, Math.Min(1f, Mouse.current.scroll.y.ReadValue())) +
            InputLibrary.toolOptionUp.GetValue() - InputLibrary.toolOptionDown.GetValue();
        _moveSpeed = (float)Math.Pow(Math.E, Math.Log(_moveSpeed) + scrollInOut * 0.1f);

        if (Keyboard.current[Key.DownArrow].wasPressedThisFrame)
        {
            _moveSpeed = 5f;
        }

        var lookRate = OWInput.UsingGamepad() ? PlayerCameraController.GAMEPAD_LOOK_RATE_Y : PlayerCameraController.LOOK_RATE;
        
        // Possibly this should use the ship input version? Since the freecam controls are more like flight
        var look = OWInput.GetAxisValue(InputLibrary.look, InputMode.All);
        _degreesY = look.y * lookRate * Time.unscaledDeltaTime;
        _degreesX = look.x * lookRate * Time.unscaledDeltaTime;

        var move = InputLibrary.moveXZ.GetAxisValue(false);
        _moveX = move.x * _moveSpeed * Time.unscaledDeltaTime;
        _moveZ = move.y * _moveSpeed * Time.unscaledDeltaTime;

        _moveY = (OWInput.GetValue(InputLibrary.thrustUp) - OWInput.GetValue(InputLibrary.thrustDown)) * _moveSpeed * Time.unscaledDeltaTime;

        if (OWInput.IsPressed(InputLibrary.rollMode)) {
            transform.Rotate(Vector3.forward, -_degreesX);
        }
        else {
            transform.Rotate(Vector3.up, _degreesX);
        }
        transform.Rotate(Vector3.right, -_degreesY);

        transform.position += _moveZ * transform.forward;
        transform.position += _moveX * transform.right;
        transform.position += _moveY * transform.up;

        if (Keyboard.current[Key.Q].isPressed)
        {
            transform.Rotate(Vector3.forward, (float)Math.Log(_moveSpeed * 0.1f + 1));
        }

        if (Keyboard.current[Key.E].isPressed)
        {
            transform.Rotate(Vector3.forward, -(float)Math.Log(_moveSpeed * 0.1f + 1));
        }
    }
}
