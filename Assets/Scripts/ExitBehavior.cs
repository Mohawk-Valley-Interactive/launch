using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBehavior : MonoBehaviour
{
	public GameObject confirmationPanel;
	public GameObject logo;
	public GameObject buttons;

	public void OnClick()
	{
		confirmationPanel.SetActive(true);
		logo.SetActive(false);
		buttons.SetActive(false);

	}

	public void OnConfirmNo()
	{
		confirmationPanel.SetActive(false);
		logo.SetActive(true);
		buttons.SetActive(true);
	}

	public void OnConfirmYes()
	{
		Application.Quit();
	}
}
