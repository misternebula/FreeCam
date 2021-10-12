using System;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

namespace FreeCam
{
	class MainClass : ModBehaviour
	{
		public static GameObject _freeCam;
		public static Camera _camera;
		OWCamera _OWCamera;
		public static float _moveSpeed = 7f;
		InputMode _storedMode;

		public static bool inputEnabled = false;

		bool mode = false;

		public bool _disableLauncher;
		public int _fov;

		public void Start()
		{
			SceneManager.sceneLoaded += this.OnSceneLoaded;

			base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
			IModEvents events = base.ModHelper.Events;
			events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));
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
			_OWCamera = _freeCam.AddComponent<OWCamera>();
			_OWCamera.renderSkybox = true;

			_freeCam.SetActive(true);
		}

		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			if (LoadManager.GetCurrentScene() == OWScene.SolarSystem)
			{
				bool flag = behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart;
				if (flag)
				{
					SetupCamera();
				}
			}

			base.ModHelper.Console.WriteLine(behaviour.name);
		}

		public override void Configure(IModConfig config)
		{
			this._disableLauncher = config.GetSettingsValue<bool>("disableLauncher");
			this._fov = config.GetSettingsValue<int>("fov");
		}

		private void SetupCamera()
		{
			if (_disableLauncher)
			{
				GameObject.Find("ProbeLauncher").SetActive(false);
				base.ModHelper.Console.WriteLine("[FreeCam] : Launcher off!");
				GameObject.Find("ProbeLauncher").SetActive(false);
				base.ModHelper.Console.WriteLine("[FreeCam] : Visor off!");
			}

			if (_freeCam.name == "FREECAM")
			{
				base.ModHelper.Console.WriteLine("[FreeCam] : Already set up! Aborting...");
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

				FlashbackScreenGrabImageEffect temp = _freeCam.AddComponent<FlashbackScreenGrabImageEffect>();
				temp._downsampleShader = Locator.GetPlayerCamera().gameObject.GetComponent<FlashbackScreenGrabImageEffect>()._downsampleShader;

				PlanetaryFogImageEffect _image = _freeCam.AddComponent<PlanetaryFogImageEffect>();
				_image.fogShader = Locator.GetPlayerCamera().gameObject.GetComponent<PlanetaryFogImageEffect>().fogShader;

                PostProcessingBehaviour _postProcessiong = _freeCam.AddComponent<PostProcessingBehaviour>();
                _postProcessiong.profile = Locator.GetPlayerCamera().gameObject.GetAddComponent<PostProcessingBehaviour>().profile;

				_freeCam.SetActive(true);
				_camera.cullingMask = Locator.GetPlayerCamera().mainCamera.cullingMask & ~(1 << 27) | (1 << 22);

				_freeCam.name = "FREECAM";
			}
		}

		void Update()
		{
			if (inputEnabled)
			{
				if (Keyboard.current[Key.UpArrow].wasPressedThisFrame)
				{
					SetupCamera();
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

				if (Keyboard.current[Key.NumpadDivide].wasPressedThisFrame)
				{
					Time.timeScale = 0f;
				}

				if (Keyboard.current[Key.NumpadMultiply].wasPressedThisFrame)
				{
					Time.timeScale = 0.5f;
				}

				if (Keyboard.current[Key.NumpadMinus].wasPressedThisFrame)
				{
					Time.timeScale = 1f;
				}

				if (Keyboard.current[Key.Numpad0].wasPressedThisFrame)
				{
					_freeCam.transform.parent = Locator.GetPlayerTransform();
					_freeCam.transform.position = Locator.GetPlayerTransform().position;
				}

				if (Keyboard.current[Key.Numpad1].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.Sun).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.Numpad2].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.Comet).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.Numpad3].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.CaveTwin).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.Numpad4].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.TowerTwin).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.Numpad5].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.TimberHearth).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.Numpad6].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.BrittleHollow).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.Numpad7].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.GiantsDeep).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.Numpad8].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.DarkBramble).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.Numpad9].wasPressedThisFrame)
				{
					var go = Locator.GetAstroObject(AstroObject.Name.RingWorld).gameObject.transform;
					_freeCam.transform.parent = go;
					_freeCam.transform.position = go.position;
				}

				if (Keyboard.current[Key.NumpadPlus].wasPressedThisFrame)
				{
					_moveSpeed = 7f;
				}

				if (Keyboard.current[Key.NumpadEnter].wasPressedThisFrame)
				{
					_moveSpeed = 1000f;
				}

				if (Keyboard.current[Key.NumpadPeriod].wasPressedThisFrame)
				{
					if (mode)
					{
						mode = false;
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
						mode = true;
						_storedMode = OWInput.GetInputMode();
						OWInput.ChangeInputMode(InputMode.None);
						GlobalMessenger<OWCamera>.FireEvent("SwitchActiveCamera", _OWCamera);
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
