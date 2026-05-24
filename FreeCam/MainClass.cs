using FreeCam.Components;
using HarmonyLib;
using OWML.Common;
using OWML.Common.Enums;
using OWML.ModHelper;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using static InputConsts;

namespace FreeCam;

class MainClass : ModBehaviour
{
	private GameObject _freeCam;
	private Camera _camera;
	private OWCamera _owCamera;

	public static UnityEvent OnFreeCamEntered = new();
	public static UnityEvent OnFreeCamExited = new();

	public static bool InFreeCam { get; private set; }

	public static bool ShowPrompts { get; private set; }
	public static bool ShowTogglePrompt { get; private set; }
	public static bool ResetParent { get; private set; }

	public static InputCommandType ToggleFreeCamBindType { get; private set; }
	public static InputCommandType ChangeSpeedBindType { get; private set; }
	public static InputCommandType CameraResetBindType { get; private set; }
	public static InputCommandType FlashlightRangeBindType { get; private set; }
	public static InputCommandType ToggleHUDBindType { get; private set; }
	public static InputCommandType TeleportBindType { get; private set; }
	public static InputCommandType ReparentBindType { get; private set; }
	public static InputCommandType CenterOnPlayerBindType { get; private set; }
	public static InputCommandType FlashlightSpeedBindType { get; private set; }
	public static InputCommandType Time0BindType { get; private set; }
	public static InputCommandType Time50BindType { get; private set; }
	public static InputCommandType Time100BindType { get; private set; }

	public static IInputCommands ToggleFreeCamBind => InputLibrary.GetInputCommand(ToggleFreeCamBindType);
	public static IInputCommands ToggleHUDBind => InputLibrary.GetInputCommand(ToggleHUDBindType);
	public static IInputCommands ChangeSpeedBind => InputLibrary.GetInputCommand(ChangeSpeedBindType);
	public static IInputCommands CameraResetBind => InputLibrary.GetInputCommand(CameraResetBindType);
	public static IInputCommands FlashlightRangeBind => InputLibrary.GetInputCommand(FlashlightRangeBindType);
	public static IInputCommands TeleportBind => InputLibrary.GetInputCommand(TeleportBindType);
	public static IInputCommands ReparentBind => InputLibrary.GetInputCommand(ReparentBindType);
	public static IInputCommands CenterOnPlayerBind => InputLibrary.GetInputCommand(CenterOnPlayerBindType);
	public static IInputCommands FlashlightSpeedBind => InputLibrary.GetInputCommand(FlashlightSpeedBindType);
	public static IInputCommands Time0Bind => InputLibrary.GetInputCommand(Time0BindType);
	public static IInputCommands Time50Bind => InputLibrary.GetInputCommand(Time50BindType);
	public static IInputCommands Time100Bind => InputLibrary.GetInputCommand(Time100BindType);

	public static readonly System.Collections.Generic.Dictionary<AstroObject.Name, InputCommandType> CenterOnPlanetBindTypes = new();

	public static IInputCommands GetCenterOnPlanetBind(AstroObject.Name name)
	{
		if (CenterOnPlanetBindTypes.TryGetValue(name, out var bindType))
		{
			return InputLibrary.GetInputCommand(bindType);
		}
		return null;
	}

	private InputMode _storedMode;
	private int _fov;
	private int _nearClipPlane;
	private ICommonCameraAPI _commonCameraAPI;
	private GameObject _hud;
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

		// Toggles
		ToggleFreeCamBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Toggle Free Cam",
			"Key to toggle free cam on/off.",
			Key.Semicolon,
			GamepadBinding.None,
			false
		);
		ToggleHUDBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Toggle HUD",
			"Toggle the helmet HUD while in free cam.",
			Key.Quote,
			GamepadBinding.None,
			false
		);

		// Camera
		ChangeSpeedBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Move Speed Adjust",
			"Axis to adjust move speed.",
			MouseBinding.ScrollUp,
			GamepadBinding.DPadUp,
			MouseBinding.ScrollDown,
			GamepadBinding.DPadDown,
			true
		);
		CameraResetBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Camera Reset",
			"Reset camera to default position.",
			Key.DownArrow,
			GamepadBinding.None,
			false
		);

		// Flashlight
		FlashlightRangeBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Flashlight Range",
			"",
			Key.LeftBracket,
			GamepadBinding.DPadLeft,
			Key.RightBracket,
			GamepadBinding.DPadRight,
			true
		);
		FlashlightSpeedBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Flashlight Fast Adjust",
			"Hold to adjust flashlight range faster.",
			Key.LeftShift,
			GamepadBinding.None,
			false
		);

		// Time shortcuts
		Time0BindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Time 0%",
			"Set game time scale to 0%.",
			Key.Comma,
			GamepadBinding.None,
			false
		);
		Time50BindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Time 50%",
			"Set game time scale to 50%.",
			Key.Period,
			GamepadBinding.None,
			false
		);
		Time100BindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Time 100%",
			"Set game time scale to 100%.",
			Key.Slash,
			GamepadBinding.None,
			false
		);

		// Teleport / Reparent hold keys
		TeleportBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Teleport Hold",
			"Hold to teleport or parent when selecting targets.",
			Key.T,
			GamepadBinding.None,
			false
		);
		ReparentBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Reparent Hold",
			"Hold to reparent when selecting targets.",
			Key.Y,
			GamepadBinding.None,
			false
		);

		CenterOnPlayerBindType = ModHelper.RebindingHelper.RegisterRebindable(
			"Center On Player",
			"Center free cam on the player.",
			Key.Digit0,
			GamepadBinding.None,
			false
		);

		// Center on planets
		CenterOnPlanetBindTypes[AstroObject.Name.Sun] = ModHelper.RebindingHelper.RegisterRebindable("Center On Sun", "Center free cam on the Sun.", Key.Digit1, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.Comet] = ModHelper.RebindingHelper.RegisterRebindable("Center On Interloper", "Center free cam on the Interloper.", Key.Digit2, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.CaveTwin] = ModHelper.RebindingHelper.RegisterRebindable("Center On Ember Twin", "Center free cam on Ember Twin.", Key.Digit3, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.TowerTwin] = ModHelper.RebindingHelper.RegisterRebindable("Center On Ash Twin", "Center free cam on Ash Twin.", Key.Digit4, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.TimberHearth] = ModHelper.RebindingHelper.RegisterRebindable("Center On Timber Hearth", "Center free cam on Timber Hearth.", Key.Digit5, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.BrittleHollow] = ModHelper.RebindingHelper.RegisterRebindable("Center On Brittle Hollow", "Center free cam on Brittle Hollow.", Key.Digit6, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.GiantsDeep] = ModHelper.RebindingHelper.RegisterRebindable("Center On Giant's Deep", "Center free cam on Giant's Deep.", Key.Digit7, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.DarkBramble] = ModHelper.RebindingHelper.RegisterRebindable("Center On Dark Bramble", "Center free cam on Dark Bramble.", Key.Digit8, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.RingWorld] = ModHelper.RebindingHelper.RegisterRebindable("Center On Stranger", "Center free cam on Stranger.", Key.Digit9, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.WhiteHole] = ModHelper.RebindingHelper.RegisterRebindable("Center On White Hole", "Center free cam on White Hole.", Key.F1, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.QuantumMoon] = ModHelper.RebindingHelper.RegisterRebindable("Center On Quantum Moon", "Center free cam on Quantum Moon.", Key.F2, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.ProbeCannon] = ModHelper.RebindingHelper.RegisterRebindable("Center On Probe Cannon", "Center free cam on Probe Cannon.", Key.F3, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.TimberMoon] = ModHelper.RebindingHelper.RegisterRebindable("Center On Attlerock", "Center free cam on Attlerock.", Key.F4, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.VolcanicMoon] = ModHelper.RebindingHelper.RegisterRebindable("Center On Hollow's Lantern", "Center free cam on Hollow's Lantern.", Key.F5, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.SunStation] = ModHelper.RebindingHelper.RegisterRebindable("Center On Sun Station", "Center free cam on Sun Station.", Key.F6, GamepadBinding.None, false);
		CenterOnPlanetBindTypes[AstroObject.Name.MapSatellite] = ModHelper.RebindingHelper.RegisterRebindable("Center On Map Satellite", "Center free cam on Map Satellite.", Key.F7, GamepadBinding.None, false);
		
		
		GlobalMessenger<OWCamera>.AddListener("SwitchActiveCamera", OnSwitchActiveCamera);

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void OnDestroy()
	{
		GlobalMessenger<OWCamera>.RemoveListener("SwitchActiveCamera", OnSwitchActiveCamera);

		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public override void Configure(IModConfig config)
	{
		_fov = config.GetSettingsValue<int>("FOV");
		_nearClipPlane = config.GetSettingsValue<int>("Near Clip Plane Distance");
		ShowPrompts = !config.GetSettingsValue<bool>("Hide Prompts");
		ShowTogglePrompt = !config.GetSettingsValue<bool>("Hide Toggle Prompt");
		ResetParent = config.GetSettingsValue<bool>("Reset Parent");

		// If the mod is currently active we can set these immediately
		if (_camera != null)
		{
			_camera.fieldOfView = _fov;
			_owCamera.fieldOfView = _fov;
			_camera.nearClipPlane = _nearClipPlane;
			_owCamera.nearClipPlane = _nearClipPlane;
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

		_hud = GameObject.Find("Player_Body/PlayerCamera/Helmet/HelmetRoot/HelmetMesh").gameObject;
	}

	private void OnSwitchActiveCamera(OWCamera camera)
	{
		if (InFreeCam && camera != _owCamera)
		{
			InFreeCam = false;
			Write($"Changing back to stored input mode {_storedMode}");
			if (_storedMode == InputMode.None)
			{
				_storedMode = InputMode.Character;
			}
			OWInput.ChangeInputMode(_storedMode);
			ResetTimeScale();
			ShowHUD();
			try
			{
				OnFreeCamExited?.Invoke();
			}
			catch (Exception e)
			{
				WriteError($"Error invoking OnFreeCamExited event: {e}");
			}
		}
		else if (!InFreeCam && camera == _owCamera)
		{
			InFreeCam = true;
			Write($"Storing input mode {_storedMode}");
			_storedMode = OWInput.GetInputMode();
			OWInput.ChangeInputMode(InputMode.None);
			try
			{
				OnFreeCamEntered?.Invoke();
			}
			catch (Exception e)
			{
				WriteError($"Error invoking OnFreeCamEntered event: {e}");
			}
		}
	}

	public static void ResetTimeScale()
	{
		Time.timeScale = OWTime.IsPaused() ? 0 : OWTime.GetTimeScale();
	}

	public static void ToggleFreeCam()
	{
		if (InFreeCam)
		{
			Write("Exiting free cam");
			_instance._commonCameraAPI.ExitCamera(_instance._owCamera);

			// Only re-enable the helmet HUD if we aren't already hiding the GUI
			if (!GUIMode.IsHiddenMode())
			{
				_instance._hud.SetActive(true);
			}
		}
		else
		{
			Write("Entering free cam");
			_instance._commonCameraAPI.EnterCamera(_instance._owCamera);
			_instance._hud.SetActive(false);
		}
	}

	public static void EnterFreeCam()
	{
		if (!InFreeCam)
		{
			ToggleFreeCam();
		}
	}

	public static void ExitFreeCam()
	{
		if (InFreeCam)
		{
			ToggleFreeCam();
		}
	}

	public static void ToggleHUD()
	{
		if (GUIMode.IsHiddenMode())
		{
			Write("Showing HUD");
			GUIMode.SetRenderMode(GUIMode.RenderMode.FPS);

			// Turning the HUD back on while in free cam also shows the helmet HUD, which we don't want
			if (InFreeCam)
			{
				_instance._hud.SetActive(false);
			}
		}
		else
		{
			Write("Hiding HUD");
			GUIMode.SetRenderMode(GUIMode.RenderMode.Hidden);
		}
	}

	public static void ShowHUD()
	{
		if (GUIMode.IsHiddenMode())
		{
			ToggleHUD();
		}
	}

	public static void HideHUD()
	{
		if (!GUIMode.IsHiddenMode())
		{
			ToggleHUD();
		}
	}

	public static void Write(string msg) => _instance.ModHelper.Console.WriteLine($"[FreeCam] : {msg}", MessageType.Info);
	public static void WriteError(string msg) => _instance.ModHelper.Console.WriteLine($"[FreeCam] : {msg}", MessageType.Error);

	public override object GetApi()
	{
		return new FreeCamAPI();
	}


	public static bool TryGetSharedAxisID(InputAction first, InputAction second, bool gamepad, out AxisIdentifier axis)
	{
		axis = AxisIdentifier.NONE;
		UnityEngine.InputSystem.InputBinding bindingMask = (gamepad ? InputActionUtil.GamepadBindingMask : InputActionUtil.DesktopBindingMask);
		int bindingIndex = first.GetBindingIndex(bindingMask);
		int bindingIndex2 = second.GetBindingIndex(bindingMask);
		if (bindingIndex == -1 || bindingIndex2 == -1)
		{
			return false;
		}
		UnityEngine.InputSystem.InputBinding inputBinding = first.bindings[bindingIndex];
		UnityEngine.InputSystem.InputBinding inputBinding2 = second.bindings[bindingIndex2];
		if (inputBinding.effectivePath == inputBinding2.effectivePath)
		{
			string text;
			string path;
			inputBinding.ToDisplayString(out text, out path, UnityEngine.InputSystem.InputBinding.DisplayStringOptions.DontUseShortDisplayNames, null);
			return InputTransitionUtil.TryGetAxisIdentifier(path, out axis);
		}
		InputControl inputControl = InputSystem.FindControl(inputBinding.effectivePath) as AxisControl;
		InputControl inputControl2 = InputSystem.FindControl(inputBinding2.effectivePath) as AxisControl;
		if (inputControl == null || inputControl2 == null)
		{
			return gamepad && InputTransitionUtil.TryGetAxisIdentifier(inputBinding.effectivePath, inputBinding2.effectivePath, out axis);
		}
		return !(inputControl is DiscreteButtonControl) && !(inputControl2 is DiscreteButtonControl) && inputControl.parent != null && inputControl2.parent != null && inputControl.parent == inputControl2.parent && (InputTransitionUtil.TryGetAxisIdentifier(inputBinding.effectivePath, inputBinding2.effectivePath, out axis) || InputTransitionUtil.TryGetAxisIdentifier(inputControl.parent.name, out axis));
	}
}
