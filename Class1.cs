using System;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FreeCam
{
	// Token: 0x02000002 RID: 2
	class MainClass : ModBehaviour
	{
		public static GameObject _freeCam;
		public static Camera _camera;
		OWCamera _OWCamera;
		public static float _moveSpeed = 7f;
		InputMode _storedMode;

		public static bool inputEnabled = false;

		bool mode = false;

		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public void Start()
		{
			SceneManager.sceneLoaded += this.OnSceneLoaded;

			base.ModHelper.Events.Subscribe<Flashlight>(Events.AfterStart);
			IModEvents events = base.ModHelper.Events;
			events.OnEvent = (Action<MonoBehaviour, Events>)Delegate.Combine(events.OnEvent, new Action<MonoBehaviour, Events>(this.OnEvent));
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020C8 File Offset: 0x000002C8
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

		// Token: 0x06000003 RID: 3 RVA: 0x00002228 File Offset: 0x00000428
		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			bool flag = behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart;
			if (flag)
			{
				_freeCam.transform.parent = Locator.GetAstroObject(AstroObject.Name.TimberHearth).gameObject.transform;
				_freeCam.transform.position = Locator.GetPlayerTransform().position;
				_freeCam.SetActive(false);
				FlashbackScreenGrabImageEffect temp = _freeCam.AddComponent<FlashbackScreenGrabImageEffect>();
				base.ModHelper.Console.WriteLine(Locator.GetPlayerCamera().gameObject.name);
				base.ModHelper.Console.WriteLine(Locator.GetPlayerCamera().gameObject.GetComponent<FlashbackScreenGrabImageEffect>().name);
				temp._downsampleShader = Locator.GetPlayerCamera().gameObject.GetComponent<FlashbackScreenGrabImageEffect>()._downsampleShader;

				PlanetaryFogImageEffect _image = _freeCam.AddComponent<PlanetaryFogImageEffect>();
				base.ModHelper.Console.WriteLine(Locator.GetPlayerCamera().gameObject.name);
				base.ModHelper.Console.WriteLine(Locator.GetPlayerCamera().gameObject.GetComponent<FlashbackScreenGrabImageEffect>().name);
				_image.fogShader = Locator.GetPlayerCamera().gameObject.GetComponent<PlanetaryFogImageEffect>().fogShader;

				_freeCam.SetActive(true);
				_camera.cullingMask = Locator.GetPlayerCamera().mainCamera.cullingMask & ~(1 << 27) | (1 << 22);

			}
		}

		void Update()
		{
			if (inputEnabled)
			{
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
