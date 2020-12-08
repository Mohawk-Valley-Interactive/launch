using UnityEngine;
using LaunchDarkly.Unity;
using LaunchDarkly.Client;

public class LaunchDarklyInterfaceBehavior : IUserAttributeProviderBehavior
{
	public string defaultEmail = "launch.default.user@launchdarkly.com";
	public string defaultMobileKey = "mob-eeeaab3e-a3c7-411f-bac7-e0f8ed5b4919";

	void Awake()
	{
		if(PlayerPrefs.HasKey(OptionsPanelBehavior.userEmailKey))
		{
			email = PlayerPrefs.GetString(OptionsPanelBehavior.userEmailKey);
		}

		if (!ClientBehavior.IsInitialized)
		{
			InitializeLdClient();
		}
	}

	void InitializeLdClient()
	{
		ClientBehavior.Instance.mobileKey = PlayerPrefs.GetString(OptionsPanelBehavior.ldMobileKeyKey, defaultMobileKey);
		ClientBehavior.Instance.userKey = (email != null ? email : defaultEmail).GetHashCode().ToString();

		ClientBehavior.Instance.Initialize();
	}

	public void UpdateEmail(string email)
	{
		if(email == null)
		{
			email = defaultEmail;
		}
		ClientBehavior.Instance.userKey = email.GetHashCode().ToString();
		ClientBehavior.Instance.RefreshUserAttributes();
	}

	public override void InjectAttributes(ref IUserBuilder userBuilder)
	{
		userBuilder.Key((email != null ? email : defaultEmail).GetHashCode().ToString());
		if(email != null)
		{
			userBuilder.Email(email);
		}
	}

	private string email = null;
}
