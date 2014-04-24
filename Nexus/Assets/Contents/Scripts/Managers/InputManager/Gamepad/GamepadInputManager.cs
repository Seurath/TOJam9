using UnityEngine;
using System.Collections;

public class GamepadInputManager : MonoBehaviour
{
	private GamepadCallbacks gamepadCallbacks = new GamepadCallbacks();
	public GamepadCallbacks GamepadCallbacks
	{
		get { return this.gamepadCallbacks; }
	}

}

