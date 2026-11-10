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
    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }

    public void Awake()
    {
        MainClass.OnFreeCamEntered.AddListener(ResetMoveSpeed);
    }

    public void OnDestroy()
    {
        MainClass.OnFreeCamEntered.RemoveListener(ResetMoveSpeed);
    }

    public void Start() => Cursor.lockState = CursorLockMode.Locked;

    public void Update()
    {
        if (OWTime.IsPaused() || !MainClass.InFreeCam) return;

        var scrollValue = OWInput.GetValue(MainClass.ChangeSpeedBind);
        //if (scrollValue != 0)
        //{
        //    MainClass.Write($"Scroll bind: {scrollValue}");
        //}
        var scrollInOut = Math.Max(-1f, Math.Min(1f, scrollValue));
        _moveSpeed = (float)Math.Pow(Math.E, Math.Log(_moveSpeed) + scrollInOut * 0.1f);

        if (OWInput.IsNewlyPressed(MainClass.CameraResetBind))
        {
            ResetMoveSpeed();
        }

        var lookRate = OWInput.UsingGamepad() ? PlayerCameraController.GAMEPAD_LOOK_RATE_Y : PlayerCameraController.LOOK_RATE;

        // Possibly this should use the ship input version? Since the freecam controls are more like flight
        var look = OWInput.GetAxisValue(InputLibrary.look);
        _degreesY = look.y * lookRate * Time.unscaledDeltaTime;
        _degreesX = look.x * lookRate * Time.unscaledDeltaTime;

        var move = InputLibrary.moveXZ.GetAxisValue(false);
        _moveX = move.x * _moveSpeed * Time.unscaledDeltaTime;
        _moveZ = move.y * _moveSpeed * Time.unscaledDeltaTime;

        _moveY = (OWInput.GetValue(InputLibrary.thrustUp) - OWInput.GetValue(InputLibrary.thrustDown)) * _moveSpeed * Time.unscaledDeltaTime;

        if (OWInput.IsPressed(InputLibrary.rollMode))
        {
            transform.Rotate(Vector3.forward, -_degreesX);
        }
        else
        {
            transform.Rotate(Vector3.up, _degreesX);
        }
        transform.Rotate(Vector3.right, -_degreesY);

        transform.position += _moveZ * transform.forward;
        transform.position += _moveX * transform.right;
        transform.position += _moveY * transform.up;
    }

    public void ResetMoveSpeed()
    {
        _moveSpeed = 5f;
    }
}
