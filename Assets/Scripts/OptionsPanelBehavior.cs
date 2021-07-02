using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelBehavior : MonoBehaviour
{
	public const string ldMobileKeyKey = "ld-mobile-key";
	public const string userEmailKey = "user-email-key";

	private const string defaultLdMobileKey = "mob-xxx-xxxxxxx";
	private const string defaultUserEmail = "user@domain.tld";

	public LaunchDarklyInterfaceBehavior launchDarklyInterfaceBehavior;
	public InputField mobileKeyInput;
	public InputField userEmailInput;
	public GameObject logoObject;
	public GameObject buttons;

	public void Show()
	{
		if (mobileKeyInput != null)
		{
			if (PlayerPrefs.HasKey(ldMobileKeyKey))
			{
				string mobileKey = PlayerPrefs.GetString(ldMobileKeyKey);
				mobileKeyInput.SetTextWithoutNotify(mobileKey.Length > 0 ? mobileKey : defaultLdMobileKey);
			}
			else
			{
				mobileKeyInput.SetTextWithoutNotify(defaultLdMobileKey);
			}
		}

		if (userEmailInput != null)
		{
			if (PlayerPrefs.HasKey(userEmailKey))
			{
				string userEmail = PlayerPrefs.GetString(userEmailKey);
				userEmailInput.SetTextWithoutNotify(userEmail.Length > 0 ? userEmail : defaultUserEmail);
			}
			else
			{
				userEmailInput.SetTextWithoutNotify(defaultUserEmail);
			}
		}

		logoObject.SetActive(false);
		buttons.SetActive(false);
		gameObject.SetActive(true);
	}

	public void Apply()
	{
		if (mobileKeyInput != null && mobileKeyInput.text != defaultLdMobileKey)
		{
			PlayerPrefs.SetString(ldMobileKeyKey, mobileKeyInput.text.Trim());
		}
		else
		{
			PlayerPrefs.DeleteKey(ldMobileKeyKey);
		}

		if (userEmailInput != null && userEmailInput.text != defaultUserEmail && userEmailInput.text.Trim().Length > 0)
		{
			string email = userEmailInput.text.Trim();
			PlayerPrefs.SetString(userEmailKey, email);
			launchDarklyInterfaceBehavior.UpdateEmail(email);
		}
		else
		{
			PlayerPrefs.DeleteKey(userEmailKey);
			launchDarklyInterfaceBehavior.UpdateEmail(null);
		}

		buttons.SetActive(true);
		logoObject.SetActive(true);
		gameObject.SetActive(false);
	}

	public void Discard()
	{
		if (mobileKeyInput != null)
		{
			if (PlayerPrefs.HasKey(ldMobileKeyKey))
			{
				mobileKeyInput.SetTextWithoutNotify(PlayerPrefs.GetString(ldMobileKeyKey));
			}
			else
			{
				mobileKeyInput.SetTextWithoutNotify(defaultLdMobileKey);
			}
		}

		if (userEmailInput != null)
		{
			if (PlayerPrefs.HasKey(userEmailKey))
			{
				userEmailInput.SetTextWithoutNotify(PlayerPrefs.GetString(userEmailKey));
			}
			else
			{
				userEmailInput.SetTextWithoutNotify(defaultUserEmail);
			}
		}

		buttons.SetActive(true);
		logoObject.SetActive(true);
		gameObject.SetActive(false);
	}
}
