using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam.Components;

public class PromptController : MonoBehaviour
{
    private ScreenPrompt
        _togglePrompt, _guiPrompt, _teleportOptions, _centerPlayerPrompt,
        _scrollPromptKeyboard, _scrollPromptGamepad, _speedPrompt,
        _rotatePrompt, _horizontalPrompt, _verticalPrompt, _lookPrompt,
        _flashlightPrompt, _flashlightRangePrompt, _flashlightSpeedPrompt;
    private List<ScreenPrompt> _planetPrompts, _timePrompts;

    private CustomFlashlight _customFlashlight;
    private CustomLookAround _customLookAround;

    private static readonly UIInputCommands _rotateLeftCmd = new("FREECAM - RotateLeft", KeyCode.Q);
    private static readonly UIInputCommands _rotateRightCmd = new("FREECAM - RotateRight", KeyCode.E);
    private static readonly UIInputCommands _scrollCmd = new("FREECAM - Scroll", KeyCode.Mouse2);
    private static readonly UIInputCommands _resetCmd = new("FREECAM - Reset", KeyCode.DownArrow);
    private static readonly UIInputCommands _rangeDown = new("FREECAM - RangeDown", KeyCode.LeftBracket);
    private static readonly UIInputCommands _rangeUp = new("FREECAM - RangeUp", KeyCode.RightBracket);

    private void Start()
    {
        _customFlashlight = GetComponent<CustomFlashlight>();
        _customLookAround = GetComponent<CustomLookAround>();
        
        // Top right
        _togglePrompt = AddPrompt("Toggle FreeCam", PromptPosition.UpperLeft, FreeCamController.ToggleKey);
        _guiPrompt = AddPrompt("Hide HUD", PromptPosition.UpperLeft, FreeCamController.GUIKey);

        _scrollPromptKeyboard = AddPrompt("Change speed   <CMD1> Reset   <CMD2>", PromptPosition.UpperLeft, [_scrollCmd, _resetCmd], ScreenPrompt.MultiCommandType.CUSTOM_BOTH);
        _scrollPromptGamepad = AddPrompt("Change speed   <CMD>", PromptPosition.UpperLeft, [InputLibrary.toolOptionUp, InputLibrary.toolOptionDown], ScreenPrompt.MultiCommandType.POS_NEG);
        _speedPrompt = AddPrompt("Speed: " + _customLookAround.MoveSpeed + " m/s", PromptPosition.UpperLeft);

        _rotatePrompt = AddPrompt(
            UITextLibrary.GetString(UITextType.RollPrompt) + " <CMD1>" + UITextLibrary.GetString(UITextType.HoldPrompt) + "  +<CMD2>", PromptPosition.UpperLeft,
            [InputLibrary.rollMode, InputLibrary.look], ScreenPrompt.MultiCommandType.CUSTOM_BOTH
        );

        _lookPrompt = AddPrompt(UITextLibrary.GetString(UITextType.LookPrompt) + "   <CMD>", PromptPosition.UpperLeft, InputLibrary.look);
        _horizontalPrompt = AddPrompt(UITextLibrary.GetString(UITextType.MovePrompt) + "   <CMD>", PromptPosition.UpperLeft, InputLibrary.moveXZ);
        _verticalPrompt = AddPrompt("Up/Down   <CMD>", PromptPosition.UpperLeft, [InputLibrary.thrustUp, InputLibrary.thrustDown], ScreenPrompt.MultiCommandType.POS_NEG);

        // Top Left
        _teleportOptions = AddPrompt("Parent options   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt), PromptPosition.UpperRight, FreeCamController.TeleportKey);
        _centerPlayerPrompt = AddPrompt("Player", PromptPosition.UpperRight, FreeCamController.CenterOnPlayerKey);

        _planetPrompts = [];
        foreach (var planet in FreeCamController.CenterOnPlanetKey.Keys)
        {
            _planetPrompts.Add(AddPrompt(AstroObject.AstroObjectNameToString(planet), PromptPosition.UpperRight, FreeCamController.CenterOnPlanetKey[planet].key));
        }

        // Flashlight
        _flashlightPrompt = AddPrompt(UITextLibrary.GetString(UITextType.FlashlightPrompt) + "   <CMD>" + UITextLibrary.GetString(UITextType.PressPrompt), PromptPosition.UpperLeft, InputLibrary.flashlight);
        _flashlightRangePrompt = AddPrompt("Flashlight range   <CMD1> <CMD2>", PromptPosition.UpperLeft, [_rangeDown, _rangeUp], ScreenPrompt.MultiCommandType.CUSTOM_BOTH);
        _flashlightSpeedPrompt = AddPrompt("Adjust range faster   <CMD>" + UITextLibrary.GetString(UITextType.HoldPrompt), PromptPosition.UpperLeft, Key.RightShift);

        _timePrompts = [
            AddPrompt("0% game speed", PromptPosition.LowerLeft, Key.Comma),
            AddPrompt("50% game speed", PromptPosition.LowerLeft, Key.Period),
            AddPrompt("100% game speed", PromptPosition.LowerLeft, Key.Slash)
        ];
    }

    private void Update()
    {
        var visible = !OWTime.IsPaused() && !GUIMode.IsHiddenMode() && PlayerData.GetPromptsEnabled() && MainClass.ShowPrompts;

        // Top right
        _togglePrompt.SetVisibility(visible);
        _guiPrompt.SetVisibility(visible && MainClass.InFreeCam);

        var usingGamepad = Locator.GetPromptManager()._usingGamepad;
        _scrollPromptGamepad.SetVisibility(visible && MainClass.InFreeCam && usingGamepad);
        _scrollPromptKeyboard.SetVisibility(visible && MainClass.InFreeCam && !usingGamepad);

        _speedPrompt.SetVisibility(visible && MainClass.InFreeCam);
        var moveSpeed = _customLookAround.MoveSpeed;
        string moveSpeedString;
        if (moveSpeed < 0.01f || moveSpeed > 100f) { moveSpeedString = moveSpeed.ToString("0.000e0"); }
        else { moveSpeedString = moveSpeed.ToString("0.000"); }
        _speedPrompt.SetText("Speed: " + moveSpeedString + " m/s");

        _rotatePrompt.SetVisibility(visible && MainClass.InFreeCam);
        _lookPrompt.SetVisibility(visible && MainClass.InFreeCam);
        _horizontalPrompt.SetVisibility(visible && MainClass.InFreeCam);
        _verticalPrompt.SetVisibility(visible && MainClass.InFreeCam);

        // Top left
        _teleportOptions.SetVisibility(visible && MainClass.InFreeCam);
        _centerPlayerPrompt.SetVisibility(visible && MainClass.InFreeCam && FreeCamController.HoldingTeleport);
        foreach (var planetPrompt in _planetPrompts)
        {
            planetPrompt.SetVisibility(visible && MainClass.InFreeCam && FreeCamController.HoldingTeleport);
        }

        // Flashlight
        _flashlightPrompt.SetVisibility(visible && MainClass.InFreeCam);
        _flashlightRangePrompt.SetVisibility(visible && MainClass.InFreeCam && _customFlashlight.FlashlightOn());
        _flashlightSpeedPrompt.SetVisibility(visible && MainClass.InFreeCam && _customFlashlight.FlashlightOn());

        // Time
        foreach (var prompt in _timePrompts)
        {
            prompt.SetVisibility(visible && MainClass.InFreeCam);
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
