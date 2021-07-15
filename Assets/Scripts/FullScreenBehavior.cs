using LaunchDarkly.Client;
using LaunchDarkly.Unity;
using UnityEngine;

public class FullScreenBehavior : MonoBehaviour
{
	public string fullscreenFlagName = "is-full-screen";
	public bool defaultValue = false;

	public void Awake()
	{
		LaunchDarklyClientBehavior.Instance.RegisterFeatureFlagChangedCallback(
			fullscreenFlagName, LdValue.Of(defaultValue), OnFullScreenValueChanged, true);
	}

	public void Start()
	{
		Screen.fullScreen = LaunchDarklyClientBehavior.Instance.BoolVariation(fullscreenFlagName, defaultValue);
	}

	public void OnFullScreenValueChanged(LdValue newValue)
	{
		Screen.fullScreen = newValue.AsBool;
	}
}
