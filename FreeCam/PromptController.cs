using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam;

public class PromptController : MonoBehaviour
{
	private ScreenPrompt _togglePrompt;

	private void Start()
	{
		if (_togglePrompt == null)
		{
			_togglePrompt = new ScreenPrompt("Toggle free cam", GetButtonSprite(KeyCode.Semicolon));
			Locator.GetPromptManager().AddScreenPrompt(_togglePrompt, PromptPosition.UpperRight, false);
		}
	}

	private void Update()
	{
		if (_togglePrompt != null)
		{
			_togglePrompt.SetVisibility(!OWTime.IsPaused());
		}
	}

	private static Sprite GetButtonSprite(KeyCode key)
	{
		var texture = ButtonPromptLibrary.SharedInstance.GetButtonTexture(key);
		var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, Vector4.zero, false);
		sprite.name = texture.name;
		return sprite;
	}
}
