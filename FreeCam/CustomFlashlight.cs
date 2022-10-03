using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam;

[RequireComponent(typeof(OWCamera))]
public class CustomFlashlight : MonoBehaviour
{
	private Light _light;

	private static float _range = 1000f;

	private static readonly float _minRange = 5f;
	private static readonly float _maxRange = 5000f;
	private static readonly float _slowRangeAdjust = 250f;
	private static readonly float _fastRangeAdjust = 2500f;

	private OWCamera _owCamera;

	void Start()
	{
		_light = gameObject.AddComponent<Light>();
		_light.range = _range;
		_light.enabled = false;

		_owCamera = gameObject.GetComponent<OWCamera>();

		// Turn off the flashlight when the camera changes
		GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);
	}

	void OnDestroy()
	{
		GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);
	}

	private void OnSwitchActiveCamera(OWCamera camera)
	{
		if (camera != _owCamera)
		{
			_light.enabled = false;
		}
	}

	void Update()
	{
		if (Locator.GetActiveCamera() != _owCamera) return;

		if (OWInput.IsNewlyPressed(InputLibrary.flashlight))
		{
			_light.enabled = !_light.enabled;
		}

		// Adjust range of the light
		if (Keyboard.current[Key.LeftBracket].IsPressed())
		{
			var rate = Keyboard.current[Key.LeftShift].IsPressed() ? _fastRangeAdjust : _slowRangeAdjust;

			_range = Mathf.Clamp(_range - (rate * Time.deltaTime), _minRange, _maxRange);
			_light.range = _range;
		}

		if (Keyboard.current[Key.RightBracket].IsPressed())
		{
			var rate = Keyboard.current[Key.LeftShift].IsPressed() ? _fastRangeAdjust : _slowRangeAdjust;

			_range = Mathf.Clamp(_range + (rate * Time.deltaTime), _minRange, _maxRange);
			_light.range = _range;
		}
	}
}
