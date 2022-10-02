using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace FreeCam
{
    class MainClass : ModBehaviour
    {
        public static GameObject FreeCam { get; private set; }
        public static Camera Camera { get; private set; }
        public static OWCamera OWCamera { get; private set; }

        public static float _moveSpeed = 0.1f;

        InputMode _storedMode;
        bool _inFreeCam = false;
        public int _fov;

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
            if (Camera != null)
            {
                Camera.fieldOfView = _fov;
                OWCamera.fieldOfView = _fov;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode _)
        {
			Write($"Loading scene {scene.name}");

            if (scene.name != "SolarSystem" && scene.name != "EyeOfTheUniverse") return;

            _inFreeCam = false;

			(OWCamera, Camera) = _commonCameraAPI.CreateCustomCamera("FREECAM", (OWCamera cam) =>
			{
				cam.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
				cam.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HeadsUpDisplay"));
				cam.mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("HelmetUVPass"));
			});
			FreeCam = Camera.gameObject;

			FreeCam.AddComponent<CustomLookAround>();
			FreeCam.AddComponent<CustomFlashlight>();
		}

        void Update()
        {
            if (Keyboard.current[Key.DownArrow].wasPressedThisFrame)
            {
                _moveSpeed = 0.1f;
            }

            if (Keyboard.current[Key.LeftArrow].wasPressedThisFrame)
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

            if (Keyboard.current[Key.NumpadDivide].wasPressedThisFrame || Keyboard.current[Key.Comma].wasPressedThisFrame)
            {
                Time.timeScale = 0f;
            }

            if (Keyboard.current[Key.NumpadMultiply].wasPressedThisFrame || Keyboard.current[Key.Period].wasPressedThisFrame)
            {
                Time.timeScale = 0.5f;
            }

            if (Keyboard.current[Key.NumpadMinus].wasPressedThisFrame || Keyboard.current[Key.Slash].wasPressedThisFrame)
            {
                Time.timeScale = 1f;
            }

            if (Keyboard.current[Key.Numpad0].wasPressedThisFrame || Keyboard.current[Key.Digit0].wasPressedThisFrame)
            {
                FreeCam.transform.parent = Locator.GetPlayerTransform();
                FreeCam.transform.position = Locator.GetPlayerTransform().position;
            }

            if (Keyboard.current[Key.Numpad1].wasPressedThisFrame || Keyboard.current[Key.Digit1].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.Sun).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            if (Keyboard.current[Key.Numpad2].wasPressedThisFrame || Keyboard.current[Key.Digit2].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.Comet).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            if (Keyboard.current[Key.Numpad3].wasPressedThisFrame || Keyboard.current[Key.Digit3].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.CaveTwin).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            if (Keyboard.current[Key.Numpad4].wasPressedThisFrame || Keyboard.current[Key.Digit4].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.TowerTwin).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            if (Keyboard.current[Key.Numpad5].wasPressedThisFrame || Keyboard.current[Key.Digit5].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.TimberHearth).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            if (Keyboard.current[Key.Numpad6].wasPressedThisFrame || Keyboard.current[Key.Digit6].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.BrittleHollow).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            if (Keyboard.current[Key.Numpad7].wasPressedThisFrame || Keyboard.current[Key.Digit7].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.GiantsDeep).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            if (Keyboard.current[Key.Numpad8].wasPressedThisFrame || Keyboard.current[Key.Digit8].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.DarkBramble).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            if (Keyboard.current[Key.Numpad9].wasPressedThisFrame || Keyboard.current[Key.Digit9].wasPressedThisFrame)
            {
                var go = Locator.GetAstroObject(AstroObject.Name.RingWorld).gameObject.transform;
                FreeCam.transform.parent = go;
                FreeCam.transform.position = go.position;
            }

            var scrollInOut = Mouse.current.scroll.y.ReadValue();
            _moveSpeed += scrollInOut * 0.05f;
            if (_moveSpeed < 0)
            {
                _moveSpeed = 0;
            }

            if (Keyboard.current[Key.NumpadPeriod].wasPressedThisFrame || Keyboard.current[Key.Semicolon].wasPressedThisFrame)
            {
                if (_inFreeCam)
                {
                    _commonCameraAPI.ExitCamera(OWCamera);
                }
                else
                {
					_commonCameraAPI.EnterCamera(OWCamera);
				}
            }
        }

		private void OnSwitchActiveCamera(OWCamera camera)
        {
            if (_inFreeCam && camera != OWCamera)
            {
				_inFreeCam = false;
				if (_storedMode == InputMode.None)
				{
					_storedMode = InputMode.Character;
				}
				OWInput.ChangeInputMode(_storedMode);
			}
            else if (!_inFreeCam && camera == OWCamera)
            {
				_inFreeCam = true;
				_storedMode = OWInput.GetInputMode();
				OWInput.ChangeInputMode(InputMode.None);
			}
        }

		public static void Write(string msg) => _instance.ModHelper.Console.WriteLine($"[FreeCam] : {msg}", MessageType.Info);
        public static void WriteError(string msg) => _instance.ModHelper.Console.WriteLine($"[FreeCam] : {msg}", MessageType.Error);
	}
}
