using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FreeCam;

internal class UIInputCommands : IInputCommands
{
	private Texture2D[] _textures;
	private readonly KeyCode[] _keys;
	private readonly InputConsts.InputCommandType _cmd;

	public UIInputCommands(string name, params KeyCode[] keys)
	{
		_keys = keys;
		_textures = keys.Select(key => ButtonPromptLibrary.SharedInstance.GetButtonTexture(key)).ToArray();
		_cmd = EnumUtils.Create<InputConsts.InputCommandType>(name);

		InputCommandManager.MappedInputActions.Add(_cmd, this);
	}

	public List<Texture2D> GetUITextures(bool gamepad, bool forceRefresh = false)
	{
		if (forceRefresh)
		{
			_textures = _keys.Select(key => ButtonPromptLibrary.SharedInstance.GetButtonTexture(key)).ToArray();
		}

		return _textures.ToList();
	} 

	public InputConsts.InputCommandType CommandType => _cmd;

	#region Unused interface

	public bool IsRebindable => false;

	public InputConsts.InputValueType ValueType => InputConsts.InputValueType.BUTTON;

	public AxisIdentifier AxisID => AxisIdentifier.NONE;

	public float PressedThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public float Sensitivity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	public int InversionFactor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public float PressDuration => throw new NotImplementedException();

	public event Action OnStarted;
	public event Action OnPerformed;
	public event Action OnCancelled;

	public void BlockNextRelease() { }

	public void ConsumeInput() { }

	public void EnableAllActions(bool enable) { }

	public InputControl GetActiveDevice() => null;

	public Vector2 GetAxisValue(bool useSensitivity = true) => Vector2.zero;

	public float GetValue() => 0f;

	public bool HasSameBinding(IInputCommands toCompare, bool usingGamepad) => false;

	public bool IsNewlyPressed() => false;

	public bool IsNewlyReleased() => false;

	public bool IsPressed(float minPressDuration = 0) => false;

	public void Update() { }
	#endregion
}
