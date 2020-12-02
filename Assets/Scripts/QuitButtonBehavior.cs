using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitButtonBehavior : MonoBehaviour
{
	public void OnClick()
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene("LaunchScene");
	}
}
