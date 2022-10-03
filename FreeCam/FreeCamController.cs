using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam;

public class FreeCamController : MonoBehaviour
{
	public const Key ToggleKey = Key.Semicolon;
	public const Key ToggleKeyAlt = Key.NumpadPeriod;

	public const Key GUIKey = Key.Quote;

	public void Update()
	{
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
			transform.parent = Locator.GetPlayerTransform();
			transform.position = Locator.GetPlayerTransform().position;
		}

		if (Keyboard.current[Key.Numpad1].wasPressedThisFrame || Keyboard.current[Key.Digit1].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.Sun).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[Key.Numpad2].wasPressedThisFrame || Keyboard.current[Key.Digit2].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.Comet).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[Key.Numpad3].wasPressedThisFrame || Keyboard.current[Key.Digit3].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.CaveTwin).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[Key.Numpad4].wasPressedThisFrame || Keyboard.current[Key.Digit4].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.TowerTwin).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[Key.Numpad5].wasPressedThisFrame || Keyboard.current[Key.Digit5].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.TimberHearth).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[Key.Numpad6].wasPressedThisFrame || Keyboard.current[Key.Digit6].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.BrittleHollow).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[Key.Numpad7].wasPressedThisFrame || Keyboard.current[Key.Digit7].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.GiantsDeep).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[Key.Numpad8].wasPressedThisFrame || Keyboard.current[Key.Digit8].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.DarkBramble).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[Key.Numpad9].wasPressedThisFrame || Keyboard.current[Key.Digit9].wasPressedThisFrame)
		{
			var go = Locator.GetAstroObject(AstroObject.Name.RingWorld).gameObject.transform;
			transform.parent = go;
			transform.position = go.position;
		}

		if (Keyboard.current[GUIKey].wasPressedThisFrame)
		{
			GUIMode.SetRenderMode(GUIMode.IsHiddenMode() ? GUIMode.RenderMode.FPS : GUIMode.RenderMode.Hidden);
		}

		if (Keyboard.current[ToggleKey].wasPressedThisFrame || Keyboard.current[ToggleKeyAlt].wasPressedThisFrame)
		{
			MainClass.ToggleFreeCam();
		}
	}
}
