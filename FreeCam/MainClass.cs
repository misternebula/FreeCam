using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FreeCam;

class MainClass : ModBehaviour
{
	private GameObject _freeCam;
	private Camera _camera;
	private OWCamera _owCamera;

	public static bool InFreeCam { get; private set; }

	private InputMode _storedMode;
	private int _fov;
	private ICommonCameraAPI _commonCameraAPI;
	private static MainClass _instance;

	public void Start()
	{
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

		_instance = this;

		try
		{
			_commonCameraAPI = ModHelper.Interaction.TryGetModApi<ICommonCameraAPI>("xen.CommonCameraUtility");
		}
		catch (Exception e)
		{
			WriteError($"{e}");
		}
		finally
		{
			if (_commonCameraAPI == null)
			{
				WriteError($"CommonCameraAPI was not found. FreeCam will not run.");
				enabled = false;
			}
		}

		GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDestroy()
	{
		GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);

		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public override void Configure(IModConfig config)
	{
		_fov = config.GetSettingsValue<int>("fov");

		// If the mod is currently active we can set these immediately
		if (_camera != null)
		{
			_camera.fieldOfView = _fov;
			_owCamera.fieldOfView = _fov;
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode _)
	{
		Write($"Loading scene {scene.name}");

		if (scene.name != "SolarSystem" && scene.name != "EyeOfTheUniverse") return;

		InFreeCam = false;

		(_owCamera, _camera) = _commonCameraAPI.CreateCustomCamera("FREECAM", (OWCamera cam) =>
		{
			cam.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
			cam.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadsUpDisplay"));
			cam.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HelmetUVPass"));
		});
		_freeCam = _camera.gameObject;

		_freeCam.AddComponent<CustomLookAround>();
		_freeCam.AddComponent<CustomFlashlight>();
		_freeCam.AddComponent<FreeCamController>();
		_freeCam.AddComponent<PromptController>();

		_freeCam.SetActive(true);
	}

	private void OnSwitchActiveCamera(OWCamera camera)
	{
		if (InFreeCam && camera != _owCamera)
		{
			InFreeCam = false;
			if (_storedMode == InputMode.None)
			{
				_storedMode = InputMode.Character;
			}
			OWInput.ChangeInputMode(_storedMode);
		}
		else if (!InFreeCam && camera == _owCamera)
		{
			InFreeCam = true;
			_storedMode = OWInput.GetInputMode();
			OWInput.ChangeInputMode(InputMode.None);
		}
	}

	public static void ToggleFreeCam()
	{
		if (InFreeCam)
		{
			_instance._commonCameraAPI.ExitCamera(_instance._owCamera);
		}
		else
		{
			_instance._commonCameraAPI.EnterCamera(_instance._owCamera);
		}
	}

	public static void Write(string msg) => _instance.ModHelper.Console.WriteLine($"[FreeCam] : {msg}", MessageType.Info);
	public static void WriteError(string msg) => _instance.ModHelper.Console.WriteLine($"[FreeCam] : {msg}", MessageType.Error);
}
