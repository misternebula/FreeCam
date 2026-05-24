using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam.Components;

public class PromptController : MonoBehaviour
{
    private ScreenPrompt
        _togglePrompt, _guiPrompt,
        _teleportOptions, _reparentOptions, _centerPlayerPrompt,
        _resetPrompt, _scrollPrompt, _speedPrompt,
        _rotatePrompt, _horizontalPrompt, _verticalPrompt, _lookPrompt,
        _flashlightPrompt, _flashlightRangePrompt, _flashlightSpeedPrompt;
    private List<ScreenPrompt> _planetPrompts, _timePrompts;

    private CustomFlashlight _customFlashlight;
    private CustomLookAround _customLookAround;

    public void Start()
    {
        _customFlashlight = GetComponent<CustomFlashlight>();
        _customLookAround = GetComponent<CustomLookAround>();

        // Top Left
        _togglePrompt = AddPrompt("Toggle FreeCam", PromptPosition.UpperLeft, MainClass.ToggleFreeCamBind);
        _guiPrompt = AddPrompt("Hide HUD", PromptPosition.UpperLeft, MainClass.ToggleHUDBind);

        _resetPrompt = AddPrompt("Reset", PromptPosition.UpperLeft, MainClass.CameraResetBind);
        _scrollPrompt = AddPrompt("Change speed", PromptPosition.UpperLeft, MainClass.ChangeSpeedBind);
        _speedPrompt = AddPrompt("Speed: " + _customLookAround.MoveSpeed + " m/s", PromptPosition.UpperLeft);

        _rotatePrompt = AddPrompt(
            "<CMD1>" + UITextLibrary.GetString(UITextType.HoldPrompt) + "  +<CMD2>  " + UITextLibrary.GetString(UITextType.RollPrompt), PromptPosition.UpperLeft,
            [InputLibrary.rollMode, InputLibrary.look], ScreenPrompt.MultiCommandType.CUSTOM_BOTH
        );

        _lookPrompt = AddPrompt(UITextLibrary.GetString(UITextType.LookPrompt), PromptPosition.UpperLeft, InputLibrary.look);
        _horizontalPrompt = AddPrompt(UITextLibrary.GetString(UITextType.MovePrompt), PromptPosition.UpperLeft, InputLibrary.moveXZ);
        _verticalPrompt = AddPrompt("Up/Down", PromptPosition.UpperLeft, [InputLibrary.thrustUp, InputLibrary.thrustDown], ScreenPrompt.MultiCommandType.POS_NEG);

        // Flashlight
        _flashlightPrompt = AddPrompt(UITextLibrary.GetString(UITextType.PressPrompt) + " " + UITextLibrary.GetString(UITextType.FlashlightPrompt), PromptPosition.UpperLeft, InputLibrary.flashlight);
        _flashlightRangePrompt = AddPrompt("Flashlight range", PromptPosition.UpperLeft, MainClass.FlashlightRangeBind);
        _flashlightSpeedPrompt = AddPrompt(UITextLibrary.GetString(UITextType.HoldPrompt) + " Adjust range faster", PromptPosition.UpperLeft, MainClass.FlashlightSpeedBind);

        // Time
        _timePrompts = [
            AddPrompt("0% game speed", PromptPosition.LowerLeft, MainClass.Time0Bind),
            AddPrompt("50% game speed", PromptPosition.LowerLeft, MainClass.Time50Bind),
            AddPrompt("100% game speed", PromptPosition.LowerLeft, MainClass.Time100Bind)
        ];

        // Top Right
        _teleportOptions = AddPrompt("Teleport options   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt), PromptPosition.UpperRight, MainClass.TeleportBind);
        _reparentOptions = AddPrompt("Parent options   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt), PromptPosition.UpperRight, MainClass.ReparentBind);
        _centerPlayerPrompt = AddPrompt("Player", PromptPosition.UpperRight, MainClass.CenterOnPlayerBind);

        _planetPrompts = [];
        foreach (var planet in MainClass.CenterOnPlanetBindTypes.Keys)
        {
            _planetPrompts.Add(AddPrompt(AstroObject.AstroObjectNameToString(planet), PromptPosition.UpperRight, MainClass.GetCenterOnPlanetBind(planet)));
        }
    }

    public void Update()
    {
        var baseVisible = !OWTime.IsPaused() && !GUIMode.IsHiddenMode() && PlayerData.GetPromptsEnabled();
        var toggleVisible = baseVisible && MainClass.ShowTogglePrompt;
        var otherVisible = baseVisible && MainClass.ShowPrompts && MainClass.InFreeCam;

        // Top Left
        _togglePrompt.SetVisibility(toggleVisible);
        _guiPrompt.SetVisibility(otherVisible);

        _scrollPrompt.SetVisibility(otherVisible);
        _resetPrompt.SetVisibility(otherVisible);

        _speedPrompt.SetVisibility(otherVisible);
        var moveSpeed = _customLookAround.MoveSpeed;
        string moveSpeedString;
        if (moveSpeed < 0.01f || moveSpeed > 100f) { moveSpeedString = moveSpeed.ToString("0.000e0"); }
        else { moveSpeedString = moveSpeed.ToString("0.000"); }
        _speedPrompt.SetText("Speed: " + moveSpeedString + " m/s");

        _rotatePrompt.SetVisibility(otherVisible);
        _lookPrompt.SetVisibility(otherVisible);
        _horizontalPrompt.SetVisibility(otherVisible);
        _verticalPrompt.SetVisibility(otherVisible);

        // Flashlight
        _flashlightPrompt.SetVisibility(otherVisible);
        _flashlightRangePrompt.SetVisibility(otherVisible && _customFlashlight.FlashlightOn());
        _flashlightSpeedPrompt.SetVisibility(otherVisible && _customFlashlight.FlashlightOn());

        // Time
        foreach (var prompt in _timePrompts)
        {
            prompt.SetVisibility(otherVisible);
        }

        // Top Right
        _teleportOptions.SetVisibility(otherVisible);
        _reparentOptions.SetVisibility(otherVisible);
        _centerPlayerPrompt.SetVisibility(otherVisible && FreeCamController.HoldingTeleport);
        foreach (var planetPrompt in _planetPrompts)
        {
            planetPrompt.SetVisibility(otherVisible && FreeCamController.HoldingTeleport);
        }
    }

    private static ScreenPrompt AddPrompt(string text, PromptPosition position, IInputCommands cmd)
    {
        var prompt = new ScreenPrompt(cmd, text);
        Locator.GetPromptManager().AddScreenPrompt(prompt, position, false);
        return prompt;
    }

    private static ScreenPrompt AddPrompt(string text, PromptPosition position)
    {
        var prompt = new ScreenPrompt(text);
        Locator.GetPromptManager().AddScreenPrompt(prompt, position, false);
        return prompt;
    }

    private static ScreenPrompt AddPrompt(string text, PromptPosition position, List<IInputCommands> commands, ScreenPrompt.MultiCommandType cmdType)
    {
        var prompt = new ScreenPrompt(commands, text, cmdType);
        Locator.GetPromptManager().AddScreenPrompt(prompt, position, false);
        return prompt;
    }
}
