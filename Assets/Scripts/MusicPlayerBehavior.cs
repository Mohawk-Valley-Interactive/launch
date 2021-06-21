using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerBehavior : MonoBehaviour
{
	public GameObject lander;
	public AudioSource lowFuelAudioSource;
	public AudioSource lowAltitudeIndicationAudioSource;
	public AudioSource highVelocityIndicationAudioSource;
	public AudioSource badAngleIndicationAudioSource;

	[System.Serializable]
	public struct DefaultAudioVolume
	{
		public AudioSource audioSource;
		public float volume;
	}

	public List<DefaultAudioVolume> defaultAudioVolumes = new List<DefaultAudioVolume>();

	public void Start()
	{
		foreach (DefaultAudioVolume dav in defaultAudioVolumes)
		{
			dav.audioSource.volume = dav.volume;
		}
	}

	public void OnLowFuel()
	{
		lowFuelAudioSource.volume = 1.0f;
	}

	public void onLowAltitudeEntered()
	{
		lowAltitudeIndicationAudioSource.volume = 0.0f;
	}

	public void onLowAltitudeExited()
	{
		lowAltitudeIndicationAudioSource.volume = 1.0f;
	}

	public void onHighVelocityEntered()
	{
		highVelocityIndicationAudioSource.volume = 0.0f;
	}

	public void onHighVelocityExited()
	{
		highVelocityIndicationAudioSource.volume = 1.0f;
	}

	public void onBadAngleEntered()
	{
		badAngleIndicationAudioSource.volume = 0.0f;
	}

	public void onBadAngleExited()
	{
		badAngleIndicationAudioSource.volume = 1.0f;
	}
}
