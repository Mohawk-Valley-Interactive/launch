using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonBehavior : MonoBehaviour
{
	public GameObject pauseMenu;

	public void OnClick()
	{
		gameObject.SetActive(false);
		pauseMenu.SetActive(true);
		Time.timeScale = 0.0f;
	}
}
