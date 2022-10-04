using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam.Components;

public class FreeCamController : MonoBehaviour
{
    public const Key ToggleKey = Key.Semicolon;
    public const Key ToggleKeyAlt = Key.NumpadPeriod;

    public const Key GUIKey = Key.Quote;

    public const Key CenterOnPlayerKey = Key.Digit0;
    public const Key CenterOnPlayerKeyAlt = Key.Numpad0;

    public static readonly Dictionary<AstroObject.Name, (Key key, Key alt)> CenterOnPlanetKey = new()
    {
        { AstroObject.Name.Sun, (Key.Digit1, Key.Numpad1) },
        { AstroObject.Name.Comet, (Key.Digit2, Key.Numpad2) },
        { AstroObject.Name.CaveTwin, (Key.Digit3, Key.Numpad3) },
        { AstroObject.Name.TowerTwin, (Key.Digit4, Key.Numpad4) },
        { AstroObject.Name.TimberHearth, (Key.Digit5, Key.Numpad5) },
        { AstroObject.Name.BrittleHollow, (Key.Digit6, Key.Numpad6) },
        { AstroObject.Name.GiantsDeep, (Key.Digit7, Key.Numpad7) },
        { AstroObject.Name.DarkBramble, (Key.Digit8, Key.Numpad8) },
        { AstroObject.Name.RingWorld, (Key.Digit9, Key.Numpad9) }
    };

    public const Key TeleportKey = Key.T;

    public static bool HoldingTeleport { get; private set; }

    public void Update()
    {
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

        HoldingTeleport = false;
        if (Keyboard.current[TeleportKey].isPressed)
        {
            HoldingTeleport = true;

            if (Keyboard.current[CenterOnPlayerKey].wasPressedThisFrame || Keyboard.current[CenterOnPlayerKeyAlt].wasPressedThisFrame)
            {
                transform.parent = Locator.GetPlayerTransform();
                transform.position = Locator.GetPlayerTransform().position;
            }

            foreach (var planet in CenterOnPlanetKey.Keys)
            {
                var (key, alt) = CenterOnPlanetKey[planet];
                if (Keyboard.current[key].wasPressedThisFrame || Keyboard.current[alt].wasPressedThisFrame)
                {
                    var go = Locator.GetAstroObject(planet).gameObject.transform;
                    transform.parent = go;
                    transform.position = go.position;
                }
            }
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
