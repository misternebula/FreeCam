using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam
{
    class CustomLookAround : MonoBehaviour
    {

        protected float _degreesX;

        protected float _degreesY;

		void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

		void Awake()
		{
			Debug.LogWarning("Awake!");
		}

        void Update()
        {
			if (OWInput.GetInputMode() == InputMode.None)
			{
				var look = InputLibrary.look.GetAxisValue(true);
				this._degreesY = look.y * 2f;
				this._degreesX = look.x * 2f;

				MainClass._camera.transform.Rotate(Vector3.up, _degreesX);
				MainClass._camera.transform.Rotate(Vector3.right, -_degreesY);

				if (Keyboard.current[Key.LeftShift].isPressed)
				{
					if (MainClass._moveSpeed == 7f)
					{
						MainClass._moveSpeed = 14f;
					}

					if (MainClass._moveSpeed == 1000f)
					{
						MainClass._moveSpeed = 2000;
					}
				}
				else
				{
					if (MainClass._moveSpeed == 14f)
					{
						MainClass._moveSpeed = 7f;
					}

					if (MainClass._moveSpeed == 2000f)
					{
						MainClass._moveSpeed = 1000;
					}
				}

				if (Keyboard.current[Key.W].isPressed)
				{
					MainClass._freeCam.transform.position += MainClass._freeCam.transform.forward * 0.02f * MainClass._moveSpeed;
				}

				if (Keyboard.current[Key.S].isPressed)
				{
					MainClass._freeCam.transform.position -= MainClass._freeCam.transform.forward * 0.02f * MainClass._moveSpeed;
				}

				if (Keyboard.current[Key.A].isPressed)
				{
					MainClass._freeCam.transform.position -= MainClass._freeCam.transform.right * 0.02f * MainClass._moveSpeed;
				}

				if (Keyboard.current[Key.D].isPressed)
				{
					MainClass._freeCam.transform.position += MainClass._freeCam.transform.right * 0.02f * MainClass._moveSpeed;
				}

				if (Keyboard.current[Key.Q].isPressed)
				{
					MainClass._freeCam.transform.Rotate(Vector3.forward, 1);
				}

				if (Keyboard.current[Key.E].isPressed)
				{
					MainClass._freeCam.transform.Rotate(Vector3.forward, -1);
				}
			}
		}
	}
}
