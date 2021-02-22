using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBehavior : MonoBehaviour
{
	public GameObject confirmationPanel;

	public void OnClick()
	{
		confirmationPanel.SetActive(true);
	}

	public void OnConfirmNo()
	{
		confirmationPanel.SetActive(false);
	}

	public void OnConfirmYes()
	{
		Application.Quit();
	}
}
