using UnityEngine;
using System.Collections;

public class DroneAudioController : MonoBehaviour
{

	[SerializeField] private AudioSource bulletSound = null;
	[SerializeField] private AudioSource deathSound = null;
	
	public void PlayBulletSound ()
	{
		if (this.bulletSound == null) { return; }
		this.bulletSound.Play();
	}
	
	public void PlayDeathSound ()
	{
		if (this.deathSound == null) { return; }
		this.deathSound.Play();
	}
	
}

