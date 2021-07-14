using LaunchDarkly.Client;
using LaunchDarkly.Unity;
using UnityEngine;

public class ThrusterBehavior : MonoBehaviour
{
	public bool playOnChange = false;
	public ParticleSystem defaultThruster = null;
	public string thrusterFlagName = "thruster-type";
	public string thrusterFlagDefault = "standard";

	public bool isEmitting
	{
		get { return thrusterActual != null && thrusterActual.isEmitting; }
	}

	void Start()
	{
		LaunchDarklyClientBehavior.Instance.RegisterFeatureFlagChangedCallback(thrusterFlagName, LdValue.Of(thrusterFlagDefault), OnThrusterFlagChanged, true);
	}

	void Update()
	{
		if (thrusterNameActual != thrusterName)
		{
			thrusterNameActual = thrusterName;
			SetThruster(thrusterNameActual);
			if (playOnChange)
			{
				Play();
			}
		}
	}

	public void Play()
	{
		if (thrusterActual)
		{
			thrusterActual.Play();
		}
	}

	public void Stop()
	{
		if (thrusterActual)
		{
			thrusterActual.Stop();
		}
	}

	public void OnThrusterFlagChanged(LdValue value)
	{
		thrusterName = value.AsString;
	}

	private void SetThruster(string thrusterId)
	{
		bool thrusterFound = false;
		foreach (Transform child in transform)
		{
			if (child.name == thrusterId)
			{
				child.gameObject.SetActive(true);
				thrusterActual = child.gameObject.GetComponent<ParticleSystem>();
				thrusterFound = true;
			}
			else
			{
				child.gameObject.SetActive(false);
			}
		}

		if (!thrusterFound && defaultThruster != null)
		{
			defaultThruster.gameObject.SetActive(true);
			thrusterActual = defaultThruster;
		}

		if (playOnChange)
		{
			Play();
		}
	}

	private ParticleSystem thrusterActual = null;
	private string thrusterName;
	private string thrusterNameActual;
}
