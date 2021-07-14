using LaunchDarkly.Client;
using LaunchDarkly.Unity;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
	public ParticleSystem defaultExplosion = null;
	public string explosionFlagName = "explosion-type";
	public string explosionFlagDefault = "standard";

	public bool isEmitting
	{
		get { return explosionActual != null && explosionActual.isEmitting; }
	}

	void Start()
	{
		LaunchDarklyClientBehavior.Instance.RegisterFeatureFlagChangedCallback(
			explosionFlagName, 
			LdValue.Of(explosionFlagDefault), 
			OnExplosionFlagChanged, 
			true);
	}

	void Update()
	{
		if (explosionNameActual != explosionName)
		{
			explosionNameActual = explosionName;
			SetExplosion(explosionNameActual);
		}
	}

	public void Play()
	{
		if (explosionActual)
		{
			explosionActual.Play();
		}
	}

	public void Stop()
	{
		if (explosionActual)
		{
			explosionActual.Stop();
		}
	}

	public void OnExplosionFlagChanged(LdValue value)
	{
		explosionName = value.AsString;
	}

	private void SetExplosion(string explosionId)
	{
		bool explosionFound = false;
		foreach (Transform child in transform)
		{
			if (child.name == explosionId)
			{
				child.gameObject.SetActive(true);
				explosionActual = child.gameObject.GetComponent<ParticleSystem>();
				explosionFound = true;
			}
			else
			{
				child.gameObject.SetActive(false);
			}
		}

		if (!explosionFound && defaultExplosion != null)
		{
			defaultExplosion.gameObject.SetActive(true);
			explosionActual = defaultExplosion;
		}
	}

	private ParticleSystem explosionActual = null;
	private string explosionName;
	private string explosionNameActual;
}
