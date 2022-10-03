using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam;

public class PromptController : MonoBehaviour
{
	private ScreenPrompt _togglePrompt, _guiPrompt, _teleportOptions, _scrollPrompt, _rotatePrompt;
	private ScreenPrompt _centerPrompt;
	private List<ScreenPrompt> _planetPrompts;

	private List<ScreenPrompt> _timePrompts;

	private ScreenPrompt _flashlightPrompt, _flashlightRangePrompt, _flashlightSpeedPrompt;

	private CustomFlashlight _customFlashlight;

	private bool _loaded;

	private static readonly UIInputCommands _rotateLeftCmd = new ("FREECAM - RotateLeft", KeyCode.Q);
	private static readonly UIInputCommands _rotateRightCmd = new ("FREECAM - RotateRight", KeyCode.E);
	private static readonly UIInputCommands _scrollCmd = new ("FREECAM - Scroll", KeyCode.Mouse2);
	private static readonly UIInputCommands _resetCmd = new ("FREECAM - Reset", KeyCode.DownArrow);
	private static readonly UIInputCommands _rangeDown = new ("FREECAM - RangeDown", KeyCode.LeftBracket);
	private static readonly UIInputCommands _rangeUp = new ("FREECAM - RangeUp", KeyCode.RightBracket);

	private void Start()
	{
		_customFlashlight = GetComponent<CustomFlashlight>();

		_loaded = true;

		// Top right
		_togglePrompt = AddPrompt("Toggle FreeCam", PromptPosition.UpperLeft, FreeCamController.ToggleKey);
		_guiPrompt = AddPrompt("Hide HUD", PromptPosition.UpperLeft, FreeCamController.GUIKey);

		_scrollPrompt = new ScreenPrompt(_scrollCmd, _resetCmd, "Movement speed   <CMD1> Reset   <CMD2>", ScreenPrompt.MultiCommandType.CUSTOM_BOTH);
		Locator.GetPromptManager().AddScreenPrompt(_scrollPrompt, PromptPosition.UpperLeft, false);

		_rotatePrompt = new ScreenPrompt(_rotateLeftCmd, _rotateRightCmd, "Rotate   <CMD1> <CMD2>", ScreenPrompt.MultiCommandType.CUSTOM_BOTH);
		Locator.GetPromptManager().AddScreenPrompt(_rotatePrompt, PromptPosition.UpperLeft, false);

		// Top Left
		_teleportOptions = AddPrompt("Parent options   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt), PromptPosition.UpperRight, FreeCamController.TeleportKey);
		_centerPrompt = AddPrompt("Player", PromptPosition.UpperRight, FreeCamController.CenterOnPlayerKey);
		
		_planetPrompts = new();
		foreach (var planet in FreeCamController.CenterOnPlanetKey.Keys)
		{
			_planetPrompts.Add(AddPrompt(AstroObject.AstroObjectNameToString(planet), PromptPosition.UpperRight, FreeCamController.CenterOnPlanetKey[planet].key));
		}

		// Flashlight
		_flashlightPrompt = new ScreenPrompt(InputLibrary.flashlight, UITextLibrary.GetString(UITextType.FlashlightPrompt) + "   <CMD>" + UITextLibrary.GetString(UITextType.PressPrompt));
		Locator.GetPromptManager().AddScreenPrompt(_flashlightPrompt, PromptPosition.UpperLeft, false);

		_flashlightRangePrompt = new ScreenPrompt(_rangeDown, _rangeUp, "Flashlight range   <CMD1> <CMD2>", ScreenPrompt.MultiCommandType.CUSTOM_BOTH);
		Locator.GetPromptManager().AddScreenPrompt(_flashlightRangePrompt, PromptPosition.UpperLeft, false);

		_flashlightSpeedPrompt = AddPrompt("Adjust range faster   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt), PromptPosition.UpperLeft, Key.RightShift);

		_timePrompts = new()
		{
			AddPrompt("0% game speed", PromptPosition.LowerLeft, Key.Comma),
			AddPrompt("50% game speed", PromptPosition.LowerLeft, Key.Period),
			AddPrompt("100% game speed", PromptPosition.LowerLeft, Key.Slash)
		};
	}

	private void Update()
	{
		if (!_loaded) return;

		var paused = OWTime.IsPaused();

		// Top right
		_togglePrompt.SetVisibility(!paused);

		_guiPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_scrollPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_rotatePrompt.SetVisibility(!paused && MainClass.InFreeCam);

		// Top left
		_teleportOptions.SetVisibility(!paused && MainClass.InFreeCam);
		_centerPrompt.SetVisibility(!paused && MainClass.InFreeCam && FreeCamController.HoldingTeleport);
		foreach (var planetPrompt in _planetPrompts)
		{
			planetPrompt.SetVisibility(!paused && MainClass.InFreeCam && FreeCamController.HoldingTeleport);
		}

		// Flashlight
		_flashlightPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_flashlightRangePrompt.SetVisibility(!paused && MainClass.InFreeCam && _customFlashlight.FlashlightOn());
		_flashlightSpeedPrompt.SetVisibility(!paused && MainClass.InFreeCam && _customFlashlight.FlashlightOn());

		// Time
		foreach (var prompt in _timePrompts)
		{
			prompt.SetVisibility(!paused && MainClass.InFreeCam);
		}
	}

	private static ScreenPrompt AddPrompt(string text, PromptPosition position, Key key)
	{
		Enum.TryParse(key.ToString().Replace("Digit", "Alpha"), out KeyCode keyCode);

		return AddPrompt(text, position, keyCode);
	}

	private static ScreenPrompt AddPrompt(string text, PromptPosition position, KeyCode keyCode)
	{
		var texture = ButtonPromptLibrary.SharedInstance.GetButtonTexture(keyCode);
		var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, Vector4.zero, false);
		sprite.name = texture.name;

		var prompt = new ScreenPrompt(text, sprite);
		Locator.GetPromptManager().AddScreenPrompt(prompt, position, false);

		return prompt;
	}
}
