using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

namespace FreeCam
{
	public class MainClass : ModBehaviour
	{
		public static GameObject FreeCam { get; set; }

		public static Camera Camera { get; set; }

		public static float MoveSpeed { get; set; } = 7f;

		public static bool InputEnabled { get; set; }

		private bool _disableLauncher;
		private int _fov;
		private OWCamera _owCamera;
		private bool _mode;
		private InputMode _storedMode;

		public void Start()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;

			ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
			ModHelper.Events.Event += OnEvent;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			FreeCam = new GameObject();
			FreeCam.SetActive(false);
			Camera = FreeCam.AddComponent<Camera>();
			Camera.clearFlags = CameraClearFlags.Color;
			Camera.backgroundColor = Color.black;
			Camera.fieldOfView = 90f;
			Camera.nearClipPlane = 0.1f;
			Camera.farClipPlane = 40000f;
			Camera.depth = 0f;
			Camera.enabled = false;

			FreeCam.AddComponent<CustomLookAround>();
			_owCamera = FreeCam.AddComponent<OWCamera>();
			_owCamera.renderSkybox = true;

			FreeCam.SetActive(true);
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			if (LoadManager.GetCurrentScene() == OWScene.SolarSystem &&
				behaviour.GetType() == typeof(Flashlight) &&
				ev == Events.AfterStart)
			{
				SetupCamera();
			}
			ModHelper.Console.WriteLine(behaviour.name);
		}

		public override void Configure(IModConfig config)
		{
			_disableLauncher = config.GetSettingsValue<bool>("disableLauncher");
			_fov = config.GetSettingsValue<int>("fov");
		}

		private void SetupCamera()
		{
			if (_disableLauncher)
			{
				GameObject.Find("ProbeLauncher").SetActive(false);
				ModHelper.Console.WriteLine("[FreeCam] : Launcher off!");
				GameObject.Find("ProbeLauncher").SetActive(false);
				ModHelper.Console.WriteLine("[FreeCam] : Visor off!");
			}

			if (FreeCam.name == "FREECAM")
			{
				ModHelper.Console.WriteLine("[FreeCam] : Already set up! Aborting...");
				return;
			}

			FreeCam.transform.parent = LoadManager.GetCurrentScene() == OWScene.SolarSystem
				? Locator.GetAstroObject(AstroObject.Name.TimberHearth).gameObject.transform
				: Locator.GetPlayerTransform();

			FreeCam.transform.position = Locator.GetPlayerTransform().position;
			FreeCam.SetActive(false);

			FreeCam.AddComponent<FlashbackScreenGrabImageEffect>()._downsampleShader =
				Locator.GetPlayerCamera().gameObject.GetComponent<FlashbackScreenGrabImageEffect>()._downsampleShader;

			FreeCam.AddComponent<PlanetaryFogImageEffect>().fogShader =
				Locator.GetPlayerCamera().gameObject.GetComponent<PlanetaryFogImageEffect>().fogShader;

			FreeCam.AddComponent<PostProcessingBehaviour>().profile =
				Locator.GetPlayerCamera().gameObject.GetAddComponent<PostProcessingBehaviour>().profile;

			FreeCam.SetActive(true);
			Camera.cullingMask = Locator.GetPlayerCamera().mainCamera.cullingMask & ~(1 << 27) | (1 << 22);

			FreeCam.name = "FREECAM";
		}

		public void Update()
		{
			if (!InputEnabled)
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				SetupCamera();
			}

			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				if (Locator.GetPlayerSuit().IsWearingHelmet())
				{
					Locator.GetPlayerSuit().RemoveHelmet();
				}
				else
				{
					Locator.GetPlayerSuit().PutOnHelmet();
				}
			}

			if (Input.GetKeyDown(KeyCode.KeypadDivide))
			{
				Time.timeScale = 0f;
			}

			if (Input.GetKeyDown(KeyCode.KeypadMultiply))
			{
				Time.timeScale = 0.5f;
			}

			if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				Time.timeScale = 1f;
			}

			if (Input.GetKeyDown(KeyCode.Keypad0))
			{
				FreeCam.transform.parent = Locator.GetPlayerTransform();
			}

			if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				FreeCam.transform.parent = Locator.GetSunTransform();
			}

			if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				FreeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.Comet).gameObject.transform;
			}

			if (Input.GetKeyDown(KeyCode.Keypad3))
			{
				FreeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.CaveTwin).gameObject.transform;
			}

			if (Input.GetKeyDown(KeyCode.Keypad4))
			{
				FreeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.TowerTwin).gameObject.transform;
			}

			if (Input.GetKeyDown(KeyCode.Keypad5))
			{
				FreeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.TimberHearth).gameObject.transform;
			}

			if (Input.GetKeyDown(KeyCode.Keypad6))
			{
				FreeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.BrittleHollow).gameObject.transform;
			}

			if (Input.GetKeyDown(KeyCode.Keypad7))
			{
				FreeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.GiantsDeep).gameObject.transform;
			}

			if (Input.GetKeyDown(KeyCode.Keypad8))
			{
				FreeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.DarkBramble).gameObject.transform;
			}

			if (Input.GetKeyDown(KeyCode.Keypad9))
			{
				FreeCam.transform.position = Locator.GetPlayerTransform().position;
			}

			if (Input.GetKeyDown(KeyCode.KeypadPlus))
			{
				MoveSpeed = 7f;
			}

			if (Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				MoveSpeed = 1000f;
			}

			if (!Input.GetKeyDown(KeyCode.KeypadPeriod))
			{
				return;
			}

			if (_mode)
			{
				_mode = false;
				if (_storedMode == InputMode.None)
				{
					_storedMode = InputMode.Character;
				}
				OWInput.ChangeInputMode(_storedMode);
				GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", Locator.GetPlayerCamera());
				Camera.enabled = false;
				Locator.GetActiveCamera().mainCamera.enabled = true;
			}
			else
			{
				_mode = true;
				_storedMode = OWInput.GetInputMode();
				OWInput.ChangeInputMode(InputMode.None);
				GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", _owCamera);
				Locator.GetActiveCamera().mainCamera.enabled = false;
				Camera.enabled = true;
			}
		}

		public static void MNActivateInput() => InputEnabled = true;

		public static void MNDeactivateInput() => InputEnabled = false;
	}
}
