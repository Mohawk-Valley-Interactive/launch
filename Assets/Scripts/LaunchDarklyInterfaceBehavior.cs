using UnityEngine;
using LaunchDarkly.Unity;
using LaunchDarkly.Client;

public class LaunchDarklyInterfaceBehavior : ILaunchDarklyUserAttributeProviderBehavior
{
	public string defaultEmail = "launch.default.user@launchdarkly.com";
	public string defaultMobileKey = "mob-eeeaab3e-a3c7-411f-bac7-e0f8ed5b4919";
	public string defaultCustomAttributeName = "";
	public string defaultCustomAttributeValue = "";

	void Awake()
	{
		if (PlayerPrefs.HasKey(OptionsPanelBehavior.userEmailKey))
		{
			emailField = PlayerPrefs.GetString(OptionsPanelBehavior.userEmailKey);
		}

		if (!LaunchDarklyClientBehavior.IsInitialized)
		{
			InitializeLdClient();
			Debug.LogWarning("LaunchDarklyInterfaceBehavior::InitializeLdClient called.");
		}
	}

	void InitializeLdClient()
	{
		LaunchDarklyClientBehavior.Instance.mobileKey = PlayerPrefs.GetString(OptionsPanelBehavior.ldMobileKeyKey, defaultMobileKey);
		LaunchDarklyClientBehavior.Instance.userKey = (emailField != null ? emailField : defaultEmail).GetHashCode().ToString();

		LaunchDarklyClientBehavior.Instance.Initialize();
	}

	public void UpdateCustomAttribute(string name, string value)
	{
		if (name == null || name.Trim().Length == 0)
		{
			customAttributeName = null;
		}
		else
		{
			customAttributeName = name;
		}
		if (value == null || value.Trim().Length == 0)
		{
			customAttributeValue = null;
		}
		else
		{
			customAttributeValue = value;
		}

		if (customAttributeName != null && customAttributeValue != null)
		{
			LaunchDarklyClientBehavior.Instance.RefreshUserAttributes();
		}
	}

	public void UpdateEmail(string e)
	{
		if (e == null || e.Trim().Length == 0)
		{
			e = defaultEmail;
			this.emailField = null;
		}
		else
		{
			this.emailField = e;
		}

		LaunchDarklyClientBehavior.Instance.userKey = e.GetHashCode().ToString();
		LaunchDarklyClientBehavior.Instance.RefreshUserAttributes();
	}

	public override void InjectAttributes(ref IUserBuilder userBuilder)
	{
		if (emailField != null && emailField.Trim().Length == 0)
		{
			emailField = null;
		}

		userBuilder.Key((emailField != null ? emailField : defaultEmail).GetHashCode().ToString());
		if (emailField != null)
		{
			userBuilder.Email(emailField);
		}

		if (customAttributeValue != null && customAttributeValue != null)
		{
			userBuilder.Custom(customAttributeName, customAttributeValue);
		}
	}

	private string emailField = null;
	private string customAttributeName = null;
	private string customAttributeValue = null;
}
