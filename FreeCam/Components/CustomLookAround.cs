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

    private float _moveSpeed = 1f;

    void Start() => Cursor.lockState = CursorLockMode.Locked;

    void Update()
    {
        if (OWInput.GetInputMode() != InputMode.None)
        {
            return;
        }

        var scrollInOut = Mouse.current.scroll.y.ReadValue();
        _moveSpeed = Math.Max(_moveSpeed + scrollInOut * 0.05f, 0f);

        if (Keyboard.current[Key.DownArrow].wasPressedThisFrame)
        {
            _moveSpeed = 0.1f;
        }

        var look = InputLibrary.look.GetAxisValue(true);
        _degreesY = look.y * 2f;
        _degreesX = look.x * 2f;

        var move = InputLibrary.moveXZ.GetAxisValue(false);
        _moveX = move.x;
        _moveZ = move.y;

        _moveY = OWInput.GetValue(InputLibrary.thrustUp) - OWInput.GetValue(InputLibrary.thrustDown);

        transform.Rotate(Vector3.up, _degreesX);
        transform.Rotate(Vector3.right, -_degreesY);
        transform.position += _moveZ * (transform.forward * 0.02f * _moveSpeed);
        transform.position += _moveX * (transform.right * 0.02f * _moveSpeed);
        transform.position += _moveY * (transform.up * 0.02f * _moveSpeed);

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
