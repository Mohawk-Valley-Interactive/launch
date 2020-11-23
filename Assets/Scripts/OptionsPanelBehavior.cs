using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelBehavior : MonoBehaviour
{

	public const string LdMobileKeyKey = "ld-mobile-key";

	public InputField MobileKeyInput;
	public void Show()
	{
		if (MobileKeyInput != null)
		{
			if (PlayerPrefs.HasKey(LdMobileKeyKey))
			{
				MobileKeyInput.SetTextWithoutNotify(PlayerPrefs.GetString(LdMobileKeyKey));
			}
			else
			{
				MobileKeyInput.SetTextWithoutNotify("mob-xxx-xxxxxxx");
			}
		}

		gameObject.SetActive(true);
	}

	public void Apply()
	{
		if (MobileKeyInput != null)
		{
			PlayerPrefs.SetString(LdMobileKeyKey, MobileKeyInput.text);
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
				MobileKeyInput.SetTextWithoutNotify("mob-xxx-xxxxxxx");
			}
		}

		gameObject.SetActive(false);
	}
}
