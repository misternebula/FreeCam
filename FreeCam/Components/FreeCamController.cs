using OWML.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam.Components;

public class FreeCamController : MonoBehaviour
{
    public const Key GUIKey = Key.Quote;

    public const Key CenterOnPlayerKey = Key.Digit0;
    public const Key CenterOnPlayerKeyAlt = Key.Numpad0;

    // Planet center bindings are managed via MainClass rebindable inputs

    public const Key TeleportKey = Key.T;
    public const Key ReparentKey = Key.Y;

    public static bool HoldingTeleport { get; private set; }

    public void Awake()
    {
        OWTime.OnPause += OnPause;
        MainClass.OnFreeCamEntered.AddListener(OnFreeCamEntered);
        GlobalMessenger<bool>.AddListener("StartSleepingAtCampfire", OnStartSleeping);
    }

    public void OnDestroy()
    {
        OWTime.OnPause -= OnPause;
        MainClass.OnFreeCamEntered.RemoveListener(OnFreeCamEntered);
        GlobalMessenger<bool>.RemoveListener("StartSleepingAtCampfire", OnStartSleeping);
    }

    private void OnPause(OWTime.PauseType pauseType)
    {
        MainClass.ExitFreeCam();
    }

    private void OnStartSleeping(bool dreamFire)
    {
        MainClass.ExitFreeCam();
    }

    private void OnFreeCamEntered()
    {
        if (MainClass.ResetParent)
        {
            ResetParent();
        }
    }

    public void Start() => ResetParent();

    public void Update()
    {
        if (OWTime.IsPaused()) return;

        if (OWInput.IsNewlyPressed(MainClass.ToggleFreeCamBind))
        {
            MainClass.ToggleFreeCam();
        }

        if (!MainClass.InFreeCam) return;

        if (OWInput.IsNewlyPressed(MainClass.Time0Bind))
        {
            Time.timeScale = 0f;
            Locator.GetMenuAudioController().PlayButtonFocus();
        }

        if (OWInput.IsNewlyPressed(MainClass.Time50Bind))
        {
            Time.timeScale = 0.5f;
            Locator.GetMenuAudioController().PlayButtonFocus();
        }

        if (OWInput.IsNewlyPressed(MainClass.Time100Bind))
        {
            Time.timeScale = 1f;
            Locator.GetMenuAudioController().PlayButtonFocus();
        }

        HoldingTeleport = false;
        if (OWInput.IsPressed(MainClass.TeleportBind) || OWInput.IsPressed(MainClass.ReparentBind))
        {
            HoldingTeleport = true;

            if (OWInput.IsNewlyPressed(MainClass.CenterOnPlayerBind))
            {
                ParentToPlayer(OWInput.IsPressed(MainClass.TeleportBind));
            }

            foreach (var planet in MainClass.CenterOnPlanetBindTypes.Keys)
            {
                var cmd = MainClass.GetCenterOnPlanetBind(planet);
                if (cmd != null && OWInput.IsNewlyPressed(cmd))
                {
                    ParentToAstroObject(Locator.GetAstroObject(planet), OWInput.IsPressed(MainClass.TeleportBind));
                }
            }
        }

        if (OWInput.IsNewlyPressed(MainClass.ToggleHUDBind))
        {
            MainClass.ToggleHUD();
        }
    }

    public void ResetParent()
    {
        ParentToPlayer(true);
    }

    public void ParentToPlayer(bool warp = false)
    {
        var playerCameraTransform = Locator.GetPlayerCamera().transform;
        transform.parent = playerCameraTransform;
        if (warp)
        {
            transform.position = playerCameraTransform.position;
            transform.rotation = playerCameraTransform.rotation;
        }
    }

    public void ParentToAstroObject(AstroObject astroObject, bool warp = false)
    {
        var astroObjectTransform = astroObject.gameObject.transform;
        transform.parent = astroObjectTransform;
        if (warp)
        {
            transform.position = astroObjectTransform.position;
        }
    }
}
