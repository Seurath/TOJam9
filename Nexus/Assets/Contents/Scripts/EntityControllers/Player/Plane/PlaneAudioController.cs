using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlaneAudioController : MonoBehaviour
{
	[SerializeField] private AudioSource engineHum = null;
	[SerializeField] private AudioSource bulletSound = null;
	
	public void PlayBulletSound ()
	{
		if (this.bulletSound == null) { return; }
		this.bulletSound.Play();
	}
	
	public void PlayEngineSound (bool isEnabled)
	{
		if (this.engineHum == null) { return; }
		if (isEnabled)
		{
			this.engineHum.Play();
		} else {
			this.engineHum.Stop();
		}
		
	}
}
