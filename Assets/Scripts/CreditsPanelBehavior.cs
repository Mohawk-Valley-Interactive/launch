using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsPanelBehavior : MonoBehaviour
{
	public GameObject creditsPanel;
	public GameObject logo;
	public GameObject buttons;

	public void Show()
	{
		creditsPanel.SetActive(true);
		logo.SetActive(false);
		buttons.SetActive(false);

	}

	public void Hide()
	{
		creditsPanel.SetActive(false);
		logo.SetActive(true);
		buttons.SetActive(true);
	}
}
