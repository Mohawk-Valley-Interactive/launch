using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButtonBehavior : MonoBehaviour
{
    public GameObject pauseButton;
    public GameObject menuPanel;
    public GameBehavior gameBehavior;

    public void Update()
    {

        if (Input.GetButtonUp("Cancel"))
        {
            if (gameObject.activeSelf)
            {
                Resume();
            }
        }
    }

    public void OnClick()
    {
        Resume();
    }

    protected void Resume()
    {
        if(gameBehavior) {
            gameBehavior.Resume();
        }
        menuPanel.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1.0f;
    }
}
