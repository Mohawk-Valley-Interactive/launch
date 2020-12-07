using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaunchDarkly.Unity;

public class LaunchDarklyInterfaceBehavior : MonoBehaviour
{
	public string defaultEmail = "launch.default.user@launchdarkly.com";
	public string defaultMobileKey = "mob-eeeaab3e-a3c7-411f-bac7-e0f8ed5b4919";

	void Awake()
	{
		ClientBehavior[] foundClientBehaviors = FindObjectsOfType<ClientBehavior>();
		BuiltInUserAttributeProviderBehavior[] foundAttributeBehaviors = FindObjectsOfType<BuiltInUserAttributeProviderBehavior>();
		if (foundClientBehaviors.Length > 1)
		{
			Debug.LogError("More than one LD Client Behavior detected in scene. Defaulting to the first instance. Total found: " + foundClientBehaviors.Length);
		}
		else if (foundClientBehaviors.Length == 0)
		{
			Debug.LogError("No LD Client Behavior detected in scene.");
			return;
		}

		if (foundAttributeBehaviors.Length > 1)
		{
			Debug.LogError("More than one BuiltInUserAttributeBehavior detected in scene. Defaulting to the first instance. Total found: " + foundClientBehaviors.Length);
		}
		else if (foundAttributeBehaviors.Length == 0)
		{
			Debug.LogError("No BuiltInUserAttributeBehavior detected in scene.");
			return;
		}

		clientBehavior = foundClientBehaviors[0];
		attributesBehavior = foundAttributeBehaviors[0];

		if (!ClientBehavior.IsInitialized)
		{
			InitializeLdClient();
		}
	}

	void InitializeLdClient()
	{
		clientBehavior.mobileKey = PlayerPrefs.GetString(OptionsPanelBehavior.ldMobileKeyKey, defaultMobileKey);

		string email = PlayerPrefs.GetString(OptionsPanelBehavior.userEmailKey);
		clientBehavior.userKey = email.GetHashCode().ToString();
		attributesBehavior.email.isSet = true;
		attributesBehavior.email.isPrivate = false;
		attributesBehavior.email.value = email;

		clientBehavior.Initialize();
	}

	public void UpdateEmail(string email)
	{
		clientBehavior.userKey = email.GetHashCode().ToString();
		attributesBehavior.email.isSet = true;
		attributesBehavior.email.isPrivate = false;
		attributesBehavior.email.value = email;

		clientBehavior.UpdateUser(attributesBehavior);
		clientBehavior.IdentifyUser();
	}

	private ClientBehavior clientBehavior;
	private BuiltInUserAttributeProviderBehavior attributesBehavior;
}
