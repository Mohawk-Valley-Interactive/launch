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
		public bool muteOnAwake;
	}

	public List<DefaultAudioVolume> defaultAudioVolumes = new List<DefaultAudioVolume>();
	public List<AudioSource> muteOnAwake = new List<AudioSource>();

	public void Start()
	{
		foreach (DefaultAudioVolume dav in defaultAudioVolumes)
		{
			dav.audioSource.volume = dav.muteOnAwake ? 0.0f : dav.volume;
		}
	}

	public void OnLowFuel()
	{
		enableAudioSource(lowFuelAudioSource);
	}

	public void onLowAltitudeEntered()
	{
		lowAltitudeIndicationAudioSource.volume = 0.0f;
	}

	public void onLowAltitudeExited()
	{
		enableAudioSource(lowAltitudeIndicationAudioSource);
	}

	public void onHighVelocityEntered()
	{
		highVelocityIndicationAudioSource.volume = 0.0f;
	}

	public void onHighVelocityExited()
	{
		enableAudioSource(highVelocityIndicationAudioSource);
	}

	public void onBadAngleEntered()
	{
		badAngleIndicationAudioSource.volume = 0.0f;
	}

	public void onBadAngleExited()
	{
		enableAudioSource(badAngleIndicationAudioSource);
	}

	private void enableAudioSource(AudioSource audioSource)
	{
		bool hasAudioSource = false;
		foreach (DefaultAudioVolume dav in defaultAudioVolumes)
		{
			if (dav.audioSource == audioSource)
			{
				dav.audioSource.volume = dav.volume;
				hasAudioSource = true;
			}
		}
		if (!hasAudioSource)
		{
			audioSource.volume = 1.0f;
		}
	}
}
