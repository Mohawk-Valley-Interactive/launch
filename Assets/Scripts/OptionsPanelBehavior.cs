using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelBehavior : MonoBehaviour
{

	public const string LdMobileKeyKey = "ld-mobile-key";
	public const string UserEmailKey = "user-email-key";

	private const string DefaultLdMobileKey = "mob-xxx-xxxxxxx";
	private const string DefaultUserEmail = "user@domain.tld";

	public InputField MobileKeyInput;
	public InputField UserEmailInput;

	public void Show()
	{
		if (MobileKeyInput != null)
		{
			if (PlayerPrefs.HasKey(LdMobileKeyKey))
			{
				string mobileKey = PlayerPrefs.GetString(LdMobileKeyKey);
				MobileKeyInput.SetTextWithoutNotify(mobileKey.Length > 0 ? mobileKey : DefaultLdMobileKey);
			}
			else
			{
				MobileKeyInput.SetTextWithoutNotify(DefaultLdMobileKey);
			}
		}

		if (UserEmailInput != null)
		{
			if (PlayerPrefs.HasKey(UserEmailKey))
			{
				string userEmail = PlayerPrefs.GetString(UserEmailKey);
				UserEmailInput.SetTextWithoutNotify(userEmail.Length > 0 ? userEmail : DefaultUserEmail);
			}
			else
			{
				UserEmailInput.SetTextWithoutNotify(DefaultUserEmail);
			}
		}

		gameObject.SetActive(true);
	}

	public void Apply()
	{
		if (MobileKeyInput != null && MobileKeyInput.text != DefaultLdMobileKey)
		{
			PlayerPrefs.SetString(LdMobileKeyKey, MobileKeyInput.text.Trim());
		}

		if (UserEmailInput != null && UserEmailInput.text != DefaultUserEmail)
		{
			PlayerPrefs.SetString(UserEmailKey, UserEmailInput.text.Trim());
		}

		gameObject.SetActive(false);
	}

	public void Discard()
	{
		if (MobileKeyInput != null)
		{
			if (PlayerPrefs.HasKey(LdMobileKeyKey))
			{
				MobileKeyInput.SetTextWithoutNotify(PlayerPrefs.GetString(LdMobileKeyKey));
			}
			else
			{
				MobileKeyInput.SetTextWithoutNotify(DefaultLdMobileKey);
			}
		}

		if (UserEmailInput != null)
		{
			if (PlayerPrefs.HasKey(UserEmailKey))
			{
				UserEmailInput.SetTextWithoutNotify(PlayerPrefs.GetString(UserEmailKey));
			}
			else
			{
				UserEmailInput.SetTextWithoutNotify(DefaultUserEmail);
			}
		}

		gameObject.SetActive(false);
	}
}
