﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam
{
    public class CustomLookAround : MonoBehaviour
    {
        private float _degreesX;
		private float _degreesY;
		private float _moveX;
		private float _moveY;

		void Start() => Cursor.lockState = CursorLockMode.Locked;

		void Update()
		{
			if (OWInput.GetInputMode() != InputMode.None)
			{
				return;
			}

			var look = InputLibrary.look.GetAxisValue(true);
			_degreesY = look.y * 2f;
			_degreesX = look.x * 2f;

			var move = InputLibrary.moveXZ.GetAxisValue(false);
			_moveX = move.x;
			_moveY = move.y;

			MainClass._camera.transform.Rotate(Vector3.up, _degreesX);
			MainClass._camera.transform.Rotate(Vector3.right, -_degreesY);
			MainClass._freeCam.transform.position += _moveY * (MainClass._freeCam.transform.forward * 0.02f * MainClass._moveSpeed);
			MainClass._freeCam.transform.position += _moveX * (MainClass._freeCam.transform.right * 0.02f * MainClass._moveSpeed);

			if (Keyboard.current[Key.Q].isPressed)
			{
				MainClass._freeCam.transform.Rotate(Vector3.forward, 0.25f);
			}

			if (Keyboard.current[Key.E].isPressed)
			{
				MainClass._freeCam.transform.Rotate(Vector3.forward, -0.25f);
			}
		}
	}
}