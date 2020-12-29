using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

namespace FreeCam
{
	public class MainClass : ModBehaviour
	{
		public static GameObject _freeCam;
		public static Camera _camera;
		public static float _moveSpeed = 7f;
		public static bool inputEnabled;
		
		public bool _disableLauncher;
		public int _fov;

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
			_freeCam = new GameObject();
			_freeCam.SetActive(false);
			_camera = _freeCam.AddComponent<Camera>();
			_camera.clearFlags = CameraClearFlags.Color;
			_camera.backgroundColor = Color.black;
			_camera.fieldOfView = 90f;
			_camera.nearClipPlane = 0.1f;
			_camera.farClipPlane = 40000f;
			_camera.depth = 0f;
			_camera.enabled = false;

			_freeCam.AddComponent<CustomLookAround>();
			_owCamera = _freeCam.AddComponent<OWCamera>();
			_owCamera.renderSkybox = true;

			_freeCam.SetActive(true);
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
			{
				var flag = behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart;
				if (flag)
				{
					SetupCamera();
				}
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

			if (_freeCam.name == "FREECAM")
			{
				ModHelper.Console.WriteLine("[FreeCam] : Already set up! Aborting...");
			}
			else
			{
				if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
				{
					_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.TimberHearth).gameObject.transform;
				}
				else
				{
					_freeCam.transform.parent = Locator.GetPlayerTransform();
				}

				_freeCam.transform.position = Locator.GetPlayerTransform().position;
				_freeCam.SetActive(false);

				var temp = _freeCam.AddComponent<FlashbackScreenGrabImageEffect>();
				temp._downsampleShader = Locator.GetPlayerCamera().gameObject.GetComponent<FlashbackScreenGrabImageEffect>()._downsampleShader;

				var _image = _freeCam.AddComponent<PlanetaryFogImageEffect>();
				_image.fogShader = Locator.GetPlayerCamera().gameObject.GetComponent<PlanetaryFogImageEffect>().fogShader;

                var _postProcessiong = _freeCam.AddComponent<PostProcessingBehaviour>();
                _postProcessiong.profile = Locator.GetPlayerCamera().gameObject.GetAddComponent<PostProcessingBehaviour>().profile;

				_freeCam.SetActive(true);
				_camera.cullingMask = Locator.GetPlayerCamera().mainCamera.cullingMask & ~(1 << 27) | (1 << 22);

				_freeCam.name = "FREECAM";
			}
		}

		public void Update()
		{
			if (inputEnabled)
			{
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
					_freeCam.transform.parent = Locator.GetPlayerTransform();
				}

				if (Input.GetKeyDown(KeyCode.Keypad1))
				{
					_freeCam.transform.parent = Locator.GetSunTransform();
				}

				if (Input.GetKeyDown(KeyCode.Keypad2))
				{
					_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.Comet).gameObject.transform;
				}

				if (Input.GetKeyDown(KeyCode.Keypad3))
				{
					_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.CaveTwin).gameObject.transform;
				}

				if (Input.GetKeyDown(KeyCode.Keypad4))
				{
					_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.TowerTwin).gameObject.transform;
				}

				if (Input.GetKeyDown(KeyCode.Keypad5))
				{
					_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.TimberHearth).gameObject.transform;
				}

				if (Input.GetKeyDown(KeyCode.Keypad6))
				{
					_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.BrittleHollow).gameObject.transform;
				}

				if (Input.GetKeyDown(KeyCode.Keypad7))
				{
					_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.GiantsDeep).gameObject.transform;
				}

				if (Input.GetKeyDown(KeyCode.Keypad8))
				{
					_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.DarkBramble).gameObject.transform;
				}

				if (Input.GetKeyDown(KeyCode.Keypad9))
				{
					_freeCam.transform.position = Locator.GetPlayerTransform().position;
				}

				if (Input.GetKeyDown(KeyCode.KeypadPlus))
				{
					_moveSpeed = 7f;
				}

				if (Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					_moveSpeed = 1000f;
				}

				if (Input.GetKeyDown(KeyCode.KeypadPeriod))
				{
					if (_mode)
					{
						_mode = false;
						if (_storedMode == InputMode.None)
						{
							_storedMode = InputMode.Character;
						}
						OWInput.ChangeInputMode(_storedMode);
						GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", Locator.GetPlayerCamera());
						_camera.enabled = false;
						Locator.GetActiveCamera().mainCamera.enabled = true;
					}
					else
					{
						_mode = true;
						_storedMode = OWInput.GetInputMode();
						OWInput.ChangeInputMode(InputMode.None);
						GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", _owCamera);
						Locator.GetActiveCamera().mainCamera.enabled = false;
						_camera.enabled = true;
					}
				}
			}
		}

		public static void MNActivateInput()
		{
			inputEnabled = true;
		}

		public static void MNDeactivateInput()
		{
			inputEnabled = false;
		}
	}
}
