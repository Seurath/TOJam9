using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum HydraControllerId
{
	Undefined = -1,
	Left = 0,
	Right = 1,
	Count
}

public class HydraInputManager : MonoBehaviour
{
	public int NumHyrdaControllers { get { return (int) HydraControllerId.Count; } }

	#region Sixense Input
	
	/// <summary>
	/// Sixense input script that comes packed inside the Sixense Unity Plugin.
	/// </summary>
	/// <remarks>
	/// The reference to this script should be set inside ManagerFactory.
	/// </remarks>
	private SixenseInput sixenseInputScript = null;

	public void SetSixenseInputScript (SixenseInput script)
	{
		this.sixenseInputScript = script;
	}
	
	/// <summary>
	/// Whether calls can be made to Sixense input.
	/// </summary>
	public bool HasSixenseInput
	{
		get { return this.sixenseInputScript != null; }
	}
	
	#endregion Sixense Input


	#region Calibration
	
	[SerializeField] private bool canCalibrate = true;
	public bool CanCalibrate
	{
		get { return this.canCalibrate; }
		set { this.canCalibrate = value; }
	}

	public bool IsCalibrated { get; set; }

	private void CalibrateHydra (SixenseInput.Controller controller)
	{
		this.offset = new Vector3(0.0f, 0.0f, controller.Position.z * this.sensitivity.Position);
		this.IsCalibrated = true;
	}

	#endregion Calibration


	#region Controller Sensitivity

	[Serializable]
	public class HydraSensitivity
	{
		[SerializeField] private float triggerPress = 0.9f;
		[SerializeField] private float triggerRelease = 0.05f;
		[SerializeField] private float position = 0.005f;

		public float TriggerPress { get { return this.triggerPress; } }
		public float TriggerRelease { get { return this.triggerRelease; } }
		public float Position { get { return this.position; } }
	}
	private HydraSensitivity sensitivity = new HydraSensitivity();
	public HydraSensitivity Sensitivity { get { return this.sensitivity; } }

	#endregion Controller Sensitivity


	#region Position Information

	[SerializeField] private Vector3 offset;
	[SerializeField] private Vector2 leftStick;
	[SerializeField] private Vector2 rightStick;
	
	private IList<int> hydraBaseOffset = null;
	public IList<int> HydraBaseOffset
	{
		get {
			if (this.hydraBaseOffset == null)
			{
				this.hydraBaseOffset = new List<int>(HydraControllerId.Count.IntValue());
			}
			if (this.hydraBaseOffset.Count == 0)
			{
				// Initialize all Hydra controller callback objects.
				for (int i = 0; i < HydraControllerId.Count.IntValue(); i++)
				{
					this.hydraBaseOffset.Add(0);
				}
			}
			return this.hydraBaseOffset;
		}
	}
	#endregion Position Information


	#region Callbacks

	private List<HydraCallbacks> hydraCallbacks = null;
	public List<HydraCallbacks> HydraCallbacks
	{
		get {
			if (this.hydraCallbacks == null)
			{
				this.hydraCallbacks = new List<HydraCallbacks>(HydraControllerId.Count.IntValue());
			}
			if (this.hydraCallbacks.Count == 0)
			{
				// Initialize all Hydra controller callback objects.
				for (int i = 0; i < HydraControllerId.Count.IntValue(); i++)
				{
					this.hydraCallbacks.Add(new HydraCallbacks());
				}
			}
			return this.hydraCallbacks;
		}
	}

	#endregion Callbacks


	#region Initialization
	
	private bool isEnabled = false;
	public bool IsEnabled { get { return this.isEnabled; } }
	
	public void Enable ()
	{
		if (!this.HasSixenseInput)
		{
			Debug.LogWarning("Attempting to activate HydraInputManager without SixenseInput script.");
			return;
		}
		this.isEnabled = true;
		SixenseInput.ControllerManagerEnabled = false;
	}
	
	public void Disable ()
	{
		this.isEnabled = false;
		SixenseInput.ControllerManagerEnabled = true;
		this.offset = new Vector3(0.0f, 0.0f, 1.0f);
	}
	
	#endregion Initialization


	#region MonoBehaviour

	void Awake ()
	{
		this.isEnabled = false;
		this.IsCalibrated = false;

		if (!this.HasSixenseInput)
		{
			// No SixenseInput script found. Do nothing.
			return;
		}

		// Hydra controller support must be explicitly enabled in an external script.
		Disable();
	}

	#endregion MonoBehaviour


	#region Hydra Input Controls
	
	public void UpdateHydra (HydraControllerId controllerId)
	{
		if (!this.IsEnabled)
		{
			// Hydra controllers are not enabled. Do nothing.
			Debug.LogWarning("Attempting to update disabled Hydra controllers.");
			return;
		}

		int id = controllerId.IntValue();
		SixenseInput.Controller controller = SixenseInput.Controllers[(int) controllerId];
		
		UpdateHydraTrigger(controller, id);
		
		if (!this.IsCalibrated) { return; }
		
		// Update position data.
		UpdateHydraPosition(controller, id);
		
		// Update rotation data.
		UpdateHydraRotation(controller, id);
		
		// Update analog stick values.
		UpdateHydraAnalogStick(controller, id);
	}
	
	private void UpdateHydraTrigger (SixenseInput.Controller controller, int controllerId)
	{
		float triggerValue = controller.Trigger;
		
		if (triggerValue < this.sensitivity.TriggerPress)
		{
			// Trigger is not pressed.
			
			if (controller.Trigger < this.sensitivity.TriggerRelease) 
			{
				// Trigger is released.
				this.hydraCallbacks[controllerId].BroadcastTriggerReleaseAction(triggerValue);
			}
			return;
		}
		
		// Trigger is pressed.
		// First check for calibration.
		if (this.CanCalibrate
		    && !this.IsCalibrated)
		{
			CalibrateHydra(controller);
			return;
		}
		
		// Broatcast trigger press.
		this.hydraCallbacks[controllerId].BroadcastTriggerPressAction(triggerValue);
	}
	
	private void UpdateHydraPosition (SixenseInput.Controller controller, int controllerId)
	{
		Vector3 controllerPosition = controller.Position;
		Vector3 worldLocalPosition = new Vector3(
			controllerPosition.x * this.sensitivity.Position,
			controllerPosition.y * this.sensitivity.Position,
			controllerPosition.z * this.sensitivity.Position) - this.offset;
		
		this.hydraCallbacks[controllerId].BroadcastPositionAction(worldLocalPosition);
	}
	
	private void UpdateHydraRotation (SixenseInput.Controller controller, int controllerId)
	{
		Quaternion worldLocalRotation = new Quaternion(
			controller.Rotation.x,
			controller.Rotation.y,
			controller.Rotation.z,
			controller.Rotation.w);
		
		this.hydraCallbacks[controllerId].BroadcastRotationAction(worldLocalRotation);
	}
	
	private void UpdateHydraAnalogStick (SixenseInput.Controller controller, int controllerId)
	{
		Vector2 stick = new Vector2(controller.JoystickX, controller.JoystickY);
		this.hydraCallbacks[controllerId].BroadcastStickAction(stick);
	}

	#endregion Hydra Input Controls

}

