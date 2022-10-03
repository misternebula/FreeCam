using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace FreeCam;

public class PromptController : MonoBehaviour
{
	private ScreenPrompt _togglePrompt, _guiPrompt, _centerPrompt, _flashlightPrompt, _flashlightRangePrompt;

	private CustomFlashlight _customFlashlight;

	private bool _loaded;

	private void Start()
	{
		_customFlashlight = GetComponent<CustomFlashlight>();

		_loaded = true;

		_togglePrompt = AddPrompt("Toggle FreeCam", PromptPosition.UpperRight, FreeCamController.ToggleKey);
		_guiPrompt = AddPrompt("Hide HUD", PromptPosition.UpperRight, FreeCamController.GUIKey);
		_centerPrompt = AddPrompt("Center on player", PromptPosition.UpperLeft, FreeCamController.CenterOnPlayerKey);

		_flashlightPrompt = new ScreenPrompt(InputLibrary.flashlight, UITextLibrary.GetString(UITextType.FlashlightPrompt) + "   <CMD>" + UITextLibrary.GetString(UITextType.PressPrompt));
		Locator.GetPromptManager().AddScreenPrompt(_flashlightPrompt, PromptPosition.UpperLeft, false);

		_flashlightRangePrompt = new ScreenPrompt(InputLibrary.toolOptionLeft, InputLibrary.toolOptionRight, "Flashlight range   <CMD1> <CMD2>", ScreenPrompt.MultiCommandType.CUSTOM_BOTH);
		Locator.GetPromptManager().AddScreenPrompt(_flashlightRangePrompt, PromptPosition.UpperLeft, false);
	}

	private void Update()
	{
		if (!_loaded) return;

		var paused = OWTime.IsPaused();

		_togglePrompt.SetVisibility(!paused);

		_guiPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_centerPrompt.SetVisibility(!paused && MainClass.InFreeCam);

		_flashlightPrompt.SetVisibility(!paused && MainClass.InFreeCam);
		_flashlightRangePrompt.SetVisibility(!paused && MainClass.InFreeCam && _customFlashlight.FlashlightOn());
	}

	private static ScreenPrompt AddPrompt(string text, PromptPosition position, Key key)
	{
		Enum.TryParse(key.ToString(), out KeyCode keyCode);

		var texture = ButtonPromptLibrary.SharedInstance.GetButtonTexture(keyCode);
		var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, Vector4.zero, false);
		sprite.name = texture.name;

		var prompt = new ScreenPrompt(text, sprite);
		Locator.GetPromptManager().AddScreenPrompt(prompt, position, false);

		return prompt;
	}
}
