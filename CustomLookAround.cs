using UnityEngine;

namespace FreeCam
{
    public class CustomLookAround : MonoBehaviour
    {
		private float _degreesX;
		private float _degreesY;

		public void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

		public void Awake()
		{
			Debug.LogWarning("Awake!");
		}

		public void Update()
        {
			if (OWInput.GetInputMode() == InputMode.None)
			{
				_degreesY = OWInput.GetValue(InputLibrary.look).y * 2f;
				_degreesX = OWInput.GetValue(InputLibrary.look).x * 2f;

				MainClass.Camera.transform.Rotate(Vector3.up, _degreesX);
				MainClass.Camera.transform.Rotate(Vector3.right, -_degreesY);

				if (Input.GetKey(KeyCode.LeftShift))
				{
					if (MainClass.MoveSpeed == 7f)
					{
						MainClass.MoveSpeed = 14f;
					}

					if (MainClass.MoveSpeed == 1000f)
					{
						MainClass.MoveSpeed = 2000;
					}
				}
				else
				{
					if (MainClass.MoveSpeed == 14f)
					{
						MainClass.MoveSpeed = 7f;
					}

					if (MainClass.MoveSpeed == 2000f)
					{
						MainClass.MoveSpeed = 1000;
					}
				}

				if (Input.GetKey(KeyCode.W))
				{
					MainClass.FreeCam.transform.position += MainClass.FreeCam.transform.forward * 0.02f * MainClass.MoveSpeed;
				}

				if (Input.GetKey(KeyCode.S))
				{
					MainClass.FreeCam.transform.position -= MainClass.FreeCam.transform.forward * 0.02f * MainClass.MoveSpeed;
				}

				if (Input.GetKey(KeyCode.A))
				{
					MainClass.FreeCam.transform.position -= MainClass.FreeCam.transform.right * 0.02f * MainClass.MoveSpeed;
				}

				if (Input.GetKey(KeyCode.D))
				{
					MainClass.FreeCam.transform.position += MainClass.FreeCam.transform.right * 0.02f * MainClass.MoveSpeed;
				}

				if (Input.GetKey(KeyCode.Q))
				{
					MainClass.FreeCam.transform.Rotate(Vector3.forward, 1);
				}

				if (Input.GetKey(KeyCode.E))
				{
					MainClass.FreeCam.transform.Rotate(Vector3.forward, -1);
				}
			}
		}
	}
}
