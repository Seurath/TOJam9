using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public enum InputId
{
	Undefined = -1,
	MouseAndKeyboard,
	Gamepad,
	Hydra
}

public class InputManager : MonoBehaviour
{
	
	[SerializeField] private InputId inputId;
	/// <summary>
	/// The type of input currently set, as defined by the InputId enum.
	/// </summary>
	public InputId InputId 
	{
		get { return this.inputId; }
		set 
		{
			this.inputId = value;
			InitializeInputManagers();
		}
	}

	public bool HasSixenseInput
	{
		get { return this.hydra.HasSixenseInput; }
	}

	public void SetSixenseInputScript (SixenseInput script)
	{
		this.hydra.SetSixenseInputScript(script);
	}

	public bool CanCalibrateHydra
	{
		get { return this.hydra.CanCalibrate; }
		set { this.hydra.CanCalibrate = value; }
	}

	public bool IsHydraCalibrated
	{
		get { return this.hydra.IsCalibrated; }
	}

	// Input managers.
	private KeyboardInputManager keyboard;
	private MouseInputManager mouse;
	private GamepadInputManager gamepad;
	private HydraInputManager hydra;


	#region Callbacks
	
	public List<HydraCallbacks> HydraCallbacks
	{
		get { return this.hydra.HydraCallbacks; }
	}

	public GamepadCallbacks GamepadCallbacks
	{
		get { return this.gamepad.GamepadCallbacks; }
	}
	
	#endregion Callbacks
	

	#region Initialization

	private void InitializeInputManagers ()
	{
		if (this.inputId == InputId.MouseAndKeyboard) 	{ InitializeMouseAndKeyboard(); }
		else if (this.inputId == InputId.Gamepad) 		{ InitializeGamepad(); }
		else if (this.inputId == InputId.Hydra) 		{ InitializeHydra(); }
	}

	private void InitializeMouseAndKeyboard ()
	{
		if (this.keyboard == null)
		{
			this.keyboard = this.gameObject.AddComponent<KeyboardInputManager>();
		}
		
		if (this.mouse == null)
		{
			this.mouse = this.gameObject.AddComponent<MouseInputManager>();
		}
	}

	private void InitializeGamepad ()
	{
		if (this.gamepad == null)
		{
			this.gamepad = this.gameObject.AddComponent<GamepadInputManager>();
		}
	}

	private void InitializeHydra ()
	{
		if (this.hydra == null)
		{
			this.hydra = this.gameObject.AddComponent<HydraInputManager>();
		}
		this.hydra.Enable();
	}

	#endregion Initialization


	#region MonoBehaviour

	void Awake ()
	{
		InitializeInputManagers();
	}

	void Update () 
	{
		if (this.InputId == InputId.Hydra)
		{
			// Using Razer Hydra controller.
			this.hydra.UpdateHydra(HydraControllerId.Left);
			this.hydra.UpdateHydra(HydraControllerId.Right);
		}
		else 
		{
			// Using gamepad controller.
			UpdateGamepad();
		}
	}
	
	#endregion MonoBehaviour
	

	#region Gamepad Input Controls
	
	private void UpdateGamepad ()
	{
		//float triggerValue = Input.GetAxis("Triggers");

		// TODO
	}
	
	#endregion Gamepad Input Controls
	
	
	#region Mouse and Keyboard Input Controlls

	private void UpdateMouse ()
	{
		// TODO
	}

	private void UpdateKeyboard ()
	{
		// TODO
	}

	#endregion Mouse and Keyboard Input Controlls
}

