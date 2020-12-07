using LaunchDarkly.Client;
using LaunchDarkly.Unity;
using UnityEngine;

public class AvatarBehavior : MonoBehaviour
{
	public string avatarFlagName = "avatar-id";
	public string avatarDefault = "standard";
	public GameObject defaultAvatar;

	void Start()
	{
		landerBehavior = transform.parent.GetComponent<LanderBehaviour>();
		ClientBehavior.Instance.RegisterFeatureFlagChangedCallback(avatarFlagName, LdValue.Of(avatarDefault), OnAvatarFlagChanged, true);
	}

	void Update()
	{
		if(avatarId != avatarIdActual)
		{
			avatarIdActual = avatarId;
			SetAvatar(avatarIdActual);
		}
	}

	private void OnAvatarFlagChanged(LdValue value)
	{
		avatarId = value.AsString;
	}

	private void SetAvatar(string avatarId)
	{
		bool avatarFound = false;
		foreach (Transform child in transform)
		{
			if (child.name == avatarId)
			{
				child.gameObject.SetActive(true);
				avatarFound = true;
				if (landerBehavior)
				{
					landerBehavior.landerAvatar = child.gameObject;
				}
			}
			else
			{
				child.gameObject.SetActive(false);
			}
		}

		if (!avatarFound && defaultAvatar != null)
		{
			defaultAvatar.SetActive(true);
			if (landerBehavior)
			{
				landerBehavior.landerAvatar = defaultAvatar;
			}
		}
	}

	private string avatarId;
	private string avatarIdActual;
	private LanderBehaviour landerBehavior;
}
