using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashBehavior : MonoBehaviour
{
	public float fadeInTime = 0.5f;
	public float waitToFadeOutTime = 2.0f;
	public float fadeOutTime = 0.5f;

	public RawImage logo;

	void Start()
	{
		startTime = Time.time;
		targetFadeInTime = startTime + fadeInTime;
		targetFadeOutTime = targetFadeInTime + waitToFadeOutTime;
		targetSceneEndTime = targetFadeOutTime + fadeOutTime;

		materialColor = logo.color;
		materialColor.a = 0.0f;
		logo.color = materialColor;
	}

	void Update()
	{
		if (Time.time < targetFadeInTime)
		{
			materialColor.a = (Time.time - startTime) / fadeInTime;
			logo.color = materialColor;
		}
		else if (Time.time < targetFadeOutTime)
		{
			materialColor.a = 1.0f;
			logo.color = materialColor;
		}
		else if (Time.time >= targetSceneEndTime)
		{
			SceneManager.LoadScene("LaunchScene");
		}
		else
		{
			materialColor.a = 1.0f - ((Time.time - targetFadeOutTime) / fadeOutTime);
			logo.color = materialColor;
		}
	}

	private float startTime;
	private float targetFadeInTime;
	private float targetFadeOutTime;
	private float targetSceneEndTime;
	private Color materialColor;
}
