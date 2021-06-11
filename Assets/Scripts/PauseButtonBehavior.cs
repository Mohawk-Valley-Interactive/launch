using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonBehavior : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameBehavior gameBehavior;

    public void Update()
    {
        if (Input.GetButtonUp("Cancel"))
        {
            Pause();
        }
    }

    public void OnClick()
    {
        Pause();
    }

    protected void Pause()
    {
        if(gameBehavior) {
            gameBehavior.Pause();
        }
        gameObject.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
