using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace FreeCam;

public class PromptController : MonoBehaviour
{
	private ScreenPrompt _togglePrompt, _guiPrompt, _teleportOptions, _scrollPrompt, _rotateLeftPrompt, _rotateRightPrompt, _resetSpeed;
	private ScreenPrompt _centerPrompt;
	private List<ScreenPrompt> _planetPrompts;

	private ScreenPrompt _flashlightPrompt, _flashlightRangePrompt, _flashlightSpeedPrompt;

	private CustomFlashlight _customFlashlight;

	private bool _loaded;

	private void Start()
	{
		_customFlashlight = GetComponent<CustomFlashlight>();

		_loaded = true;

		// Top right
		_togglePrompt = AddPrompt("Toggle FreeCam", PromptPosition.UpperRight, FreeCamController.ToggleKey);
		_guiPrompt = AddPrompt("Hide HUD", PromptPosition.UpperRight, FreeCamController.GUIKey);
		
		_scrollPrompt = AddPrompt("Movement speed   <CMD> (Scroll)", PromptPosition.UpperRight, KeyCode.Mouse2);
		_resetSpeed = AddPrompt("Reset movement speed", PromptPosition.UpperRight, KeyCode.DownArrow);

		_rotateLeftPrompt = AddPrompt("Rotate left", PromptPosition.UpperRight, KeyCode.Q);
		_rotateRightPrompt = AddPrompt("Rotate right", PromptPosition.UpperRight, KeyCode.E);

		// Top Left
		_teleportOptions = AddPrompt("Parent options   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt), PromptPosition.UpperLeft, FreeCamController.TeleportKey);
		_centerPrompt = AddPrompt("Player", PromptPosition.UpperLeft, FreeCamController.CenterOnPlayerKey);
		
		_planetPrompts = new();
		foreach (var planet in FreeCamController.CenterOnPlanetKey.Keys)
		{
			_planetPrompts.Add(AddPrompt(AstroObject.AstroObjectNameToString(planet), PromptPosition.UpperLeft, FreeCamController.CenterOnPlanetKey[planet].key));
		}

		// Bottom right
		_flashlightPrompt = new ScreenPrompt(InputLibrary.flashlight, UITextLibrary.GetString(UITextType.FlashlightPrompt) + "   <CMD>" + UITextLibrary.GetString(UITextType.PressPrompt));
		Locator.GetPromptManager().AddScreenPrompt(_flashlightPrompt, PromptPosition.LowerLeft, false);

		_flashlightRangePrompt = new ScreenPrompt(InputLibrary.toolOptionLeft, InputLibrary.toolOptionRight, "Flashlight range   <CMD1> <CMD2>", ScreenPrompt.MultiCommandType.CUSTOM_BOTH);
		Locator.GetPromptManager().AddScreenPrompt(_flashlightRangePrompt, PromptPosition.LowerLeft, false);

		_flashlightSpeedPrompt = AddPrompt("Adjust range faster <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt), PromptPosition.LowerLeft, Key.LeftShift);
	}

	private void Update()
	{
		if (!_loaded) return;

		var paused = OWTime.IsPaused();

		// Top right
		_togglePrompt.SetVisibility(!paused);

		_guiPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_scrollPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_rotateLeftPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_rotateRightPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_resetSpeed.SetVisibility(!paused && MainClass.InFreeCam);

		// Top left
		_teleportOptions.SetVisibility(!paused && MainClass.InFreeCam);
		_centerPrompt.SetVisibility(!paused && MainClass.InFreeCam && FreeCamController.HoldingTeleport);
		foreach (var planetPrompt in _planetPrompts)
		{
			planetPrompt.SetVisibility(!paused && MainClass.InFreeCam && FreeCamController.HoldingTeleport);
		}

		// Bottom left
		_flashlightPrompt.SetVisibility(!paused && MainClass.InFreeCam && !FreeCamController.HoldingTeleport);
		_flashlightRangePrompt.SetVisibility(!paused && MainClass.InFreeCam && _customFlashlight.FlashlightOn() && !FreeCamController.HoldingTeleport);
		_flashlightSpeedPrompt.SetVisibility(!paused && MainClass.InFreeCam && _customFlashlight.FlashlightOn() && !FreeCamController.HoldingTeleport);
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
