using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButtonBehavior : MonoBehaviour
{
	public GameObject pauseButton;
	public GameObject menuPanel;

	public void OnClick()
	{
		menuPanel.SetActive(false);
		pauseButton.SetActive(true);
		Time.timeScale = 1.0f;
	}
}
