using LaunchDarkly.Client;
using LaunchDarkly.Unity;
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

	public string audioLevelsFlagName = "audio-levels";
	public float musicVolume = 1.0f;

	void Awake()
	{
		LdValue defaultValue = LdValue.BuildObject()
			.Add("music", 1.0f)
			.Build();
		LaunchDarklyClientBehavior.Instance.RegisterFeatureFlagChangedCallback(
			audioLevelsFlagName, defaultValue, OnAudioLevelsFlagChanged2, true);
	}

	void Start()
	{
		LdValue defaultValue = LdValue.BuildObject()
			.Add("music", 1.0f)
			.Build();
		LdValue volumes = LaunchDarklyClientBehavior.Instance.JsonVariation(audioLevelsFlagName, defaultValue);
		IReadOnlyDictionary<string, float> al = volumes.AsDictionary<float>(LdValue.Convert.Float);
		musicVolume = al.ContainsKey("music") ? al["music"] : 1.0f;
		musicVolumeActual = musicVolume;

		foreach (DefaultAudioVolume dav in defaultAudioVolumes)
		{
			dav.audioSource.volume = dav.muteOnAwake ? 0.0f : dav.volume * musicVolumeActual;
		}
	}

	void Update()
	{
		if (musicVolume != musicVolumeActual)
		{
			musicVolumeActual = musicVolume;
			foreach (DefaultAudioVolume dav in defaultAudioVolumes)
			{
				dav.audioSource.volume = dav.muteOnAwake ? 0.0f : dav.volume * musicVolumeActual;
			}
		}
	}

	public void OnAudioLevelsFlagChanged2(LdValue audioLevels)
	{
		IReadOnlyDictionary<string, float> al = audioLevels.AsDictionary<float>(LdValue.Convert.Float);
		musicVolume = al.ContainsKey("music") ? al["music"] : 1.0f;
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
				dav.audioSource.volume = dav.volume * musicVolume;
				hasAudioSource = true;
			}
		}
		if (!hasAudioSource)
		{
			audioSource.volume = musicVolume;
		}
	}

	private float musicVolumeActual = 1.0f;
}
