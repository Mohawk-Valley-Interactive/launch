using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelBehavior : MonoBehaviour
{
	public const string ldMobileKeyKey = "ld-mobile-key";
	public const string userEmailKey = "user-email-key";
	public const string customAttributeNameKey = "custom-attribute-name-key";
	public const string customAttributeValueKey = "custom-attribute-value-key";

	private const string defaultLdMobileKey = "mob-xxx-xxxxxxx";
	private const string defaultUserEmail = "user@domain.tld";
	private const string defaultCustomAttributeName = "your_custom_attribute_name_here";
	private const string defaultCustomAttributeValue = "your_custom_attribute_value_here";

	public LaunchDarklyInterfaceBehavior launchDarklyInterfaceBehavior;
	public InputField mobileKeyInput;
	public InputField userEmailInput;
	public InputField customAttributeNameInput;
	public InputField customAttributeValueInput;
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

		if (customAttributeNameInput != null)
		{
			if (PlayerPrefs.HasKey(customAttributeNameKey))
			{
				string customAttributeName = PlayerPrefs.GetString(customAttributeNameKey);
				customAttributeNameInput.SetTextWithoutNotify(customAttributeName.Length > 0 ? customAttributeName : defaultUserEmail);
			}
			else
			{
				customAttributeNameInput.SetTextWithoutNotify(defaultCustomAttributeName);
			}
		}

		if (customAttributeValueInput != null)
		{
			if (PlayerPrefs.HasKey(customAttributeValueKey))
			{
				string customAttributeValue = PlayerPrefs.GetString(customAttributeValueKey);
				customAttributeValueInput.SetTextWithoutNotify(customAttributeValue.Length > 0 ? customAttributeValue : defaultUserEmail);
			}
			else
			{
				customAttributeValueInput.SetTextWithoutNotify(defaultCustomAttributeValue);
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

		if (customAttributeNameInput != null && customAttributeNameInput.text != defaultCustomAttributeName && customAttributeNameInput.text.Trim().Length > 0)
		{
			string customAttributeName = customAttributeNameInput.text.Trim();
			PlayerPrefs.SetString(customAttributeNameKey, customAttributeName);
		}
		else
		{
			PlayerPrefs.DeleteKey(customAttributeNameKey);
		}

		if (customAttributeValueInput != null && customAttributeValueInput.text != defaultCustomAttributeValue && customAttributeValueInput.text.Trim().Length > 0)
		{
			string customAttributeValue = customAttributeValueInput.text.Trim();
			PlayerPrefs.SetString(customAttributeValueKey, customAttributeValue);
		}
		else
		{
			PlayerPrefs.DeleteKey(customAttributeValueKey);
		}

		if (PlayerPrefs.HasKey(customAttributeNameKey) && PlayerPrefs.HasKey(customAttributeValueKey))
		{
			launchDarklyInterfaceBehavior.UpdateCustomAttribute(PlayerPrefs.GetString(customAttributeNameKey), PlayerPrefs.GetString(customAttributeValueKey));
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

		if (customAttributeNameInput != null)
		{
			if (PlayerPrefs.HasKey(customAttributeNameKey))
			{
				customAttributeNameInput.SetTextWithoutNotify(PlayerPrefs.GetString(customAttributeNameKey));
			}
			else
			{
				customAttributeNameInput.SetTextWithoutNotify(customAttributeNameKey);
			}
		}

		if (customAttributeValueInput != null)
		{
			if (PlayerPrefs.HasKey(customAttributeValueKey))
			{
				customAttributeValueInput.SetTextWithoutNotify(PlayerPrefs.GetString(customAttributeValueKey));
			}
			else
			{
				customAttributeValueInput.SetTextWithoutNotify(customAttributeValueKey);
			}
		}

		buttons.SetActive(true);
		logoObject.SetActive(true);
		gameObject.SetActive(false);
	}
}
