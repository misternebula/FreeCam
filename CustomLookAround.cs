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
		protected float _moveX;
		protected float _moveY;
		protected float _boost = 1f;

		void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
			if (OWInput.GetInputMode() == InputMode.None)
			{
				this._degreesY = OWInput.GetValue(InputLibrary.look, InputMode.All).y * 2f;
				this._degreesX = OWInput.GetValue(InputLibrary.look, InputMode.All).x * 2f;
				this._moveX = OWInput.GetValue(InputLibrary.moveXZ, InputMode.All).x;
				this._moveY = OWInput.GetValue(InputLibrary.moveXZ, InputMode.All).y;

				MainClass._camera.transform.Rotate(Vector3.up, _degreesX);
				MainClass._camera.transform.Rotate(Vector3.right, -_degreesY);
				MainClass._freeCam.transform.position += _moveY * ((MainClass._freeCam.transform.forward * 0.02f * MainClass._moveSpeed) * _boost);
				MainClass._freeCam.transform.position += _moveX * ((MainClass._freeCam.transform.right * 0.02f * MainClass._moveSpeed) * _boost);

				if (Input.GetKey(KeyCode.LeftShift))
				{
					_boost = 2;
				}
				else
				{
					_boost = 1;
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
