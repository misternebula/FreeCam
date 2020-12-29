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
