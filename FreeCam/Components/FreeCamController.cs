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
    public const Key ReparentKey = Key.Y;

    public static bool HoldingTeleport { get; private set; }

    public void Start() => ParentToPlayer(true);

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
        if (Keyboard.current[TeleportKey].isPressed || Keyboard.current[ReparentKey].isPressed)
        {
            HoldingTeleport = true;

            if (Keyboard.current[CenterOnPlayerKey].wasPressedThisFrame || Keyboard.current[CenterOnPlayerKeyAlt].wasPressedThisFrame)
            {
                ParentToPlayer(Keyboard.current[TeleportKey].isPressed);
            }

            foreach (var planet in CenterOnPlanetKey.Keys)
            {
                var (key, alt) = CenterOnPlanetKey[planet];
                if (Keyboard.current[key].wasPressedThisFrame || Keyboard.current[alt].wasPressedThisFrame)
                {
                    ParentToAstroObject(Locator.GetAstroObject(planet), Keyboard.current[TeleportKey].isPressed);
                }
            }
        }

        if (Keyboard.current[GUIKey].wasPressedThisFrame)
        {
            MainClass.ToggleHUD();
        }

        if (Keyboard.current[ToggleKey].wasPressedThisFrame || Keyboard.current[ToggleKeyAlt].wasPressedThisFrame)
        {
            MainClass.ToggleFreeCam();
        }
    }

    public void ParentToPlayer(bool warp = false)
    {
        var playerCameraTransform = Locator.GetPlayerCamera().transform;
        transform.parent = playerCameraTransform;
        if (warp) {
            transform.position = playerCameraTransform.position;
            transform.rotation = playerCameraTransform.rotation;
        }
    }

    public void ParentToAstroObject(AstroObject astroObject, bool warp = false)
    {
        var astroObjectTransform = astroObject.gameObject.transform;
        transform.parent = astroObjectTransform;
        if (warp) {
            transform.position = astroObjectTransform.position;
        }
    }
}
