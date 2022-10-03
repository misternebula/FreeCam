using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam;

public class PromptController : MonoBehaviour
{
	private ScreenPrompt _togglePrompt, _guiPrompt;

	private bool _loaded;

	private void Start()
	{
		_loaded = true;

		_togglePrompt = AddPrompt("Toggle FreeCam", FreeCamController.ToggleKey);
		_guiPrompt = AddPrompt("Hide HUD", FreeCamController.GUIKey);
	}

	private void Update()
	{
		if (!_loaded) return;

		var paused = OWTime.IsPaused();

		_togglePrompt.SetVisibility(!paused);
		_guiPrompt.SetVisibility(!paused && MainClass.InFreeCam);
	}

	private static ScreenPrompt AddPrompt(string text, Key key)
	{
		Enum.TryParse(key.ToString(), out KeyCode keyCode);

		var texture = ButtonPromptLibrary.SharedInstance.GetButtonTexture(keyCode);
		var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, Vector4.zero, false);
		sprite.name = texture.name;

		var prompt = new ScreenPrompt(text, sprite);
		Locator.GetPromptManager().AddScreenPrompt(prompt, PromptPosition.UpperRight, false);

		return prompt;
	}
}
