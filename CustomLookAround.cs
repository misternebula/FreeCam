using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
				this._degreesY = OWInput.GetValue(InputLibrary.look, InputMode.All).y * 2f;
				this._degreesX = OWInput.GetValue(InputLibrary.look, InputMode.All).x * 2f;

				MainClass._camera.transform.Rotate(Vector3.up, _degreesX);
				MainClass._camera.transform.Rotate(Vector3.right, -_degreesY);

				if (Input.GetKey(KeyCode.LeftShift))
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

				if (Input.GetKey(KeyCode.W))
				{
					MainClass._freeCam.transform.position += MainClass._freeCam.transform.forward * 0.02f * MainClass._moveSpeed;
				}

				if (Input.GetKey(KeyCode.S))
				{
					MainClass._freeCam.transform.position -= MainClass._freeCam.transform.forward * 0.02f * MainClass._moveSpeed;
				}

				if (Input.GetKey(KeyCode.A))
				{
					MainClass._freeCam.transform.position -= MainClass._freeCam.transform.right * 0.02f * MainClass._moveSpeed;
				}

				if (Input.GetKey(KeyCode.D))
				{
					MainClass._freeCam.transform.position += MainClass._freeCam.transform.right * 0.02f * MainClass._moveSpeed;
				}

				if (Input.GetKey(KeyCode.Q))
				{
					MainClass._freeCam.transform.Rotate(Vector3.forward, 1);
				}

				if (Input.GetKey(KeyCode.E))
				{
					MainClass._freeCam.transform.Rotate(Vector3.forward, -1);
				}
			}
		}
	}
}
