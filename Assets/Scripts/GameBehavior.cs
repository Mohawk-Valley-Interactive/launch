using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBehavior : MonoBehaviour
{
	[Header("General")]
	public CameraBehavior cam;
	public LanderBehaviour lander;
	public LandscapeGeneratorBehavior landscapeGenerator;
	[Space(5)]

	[Header("UI Gauges")]
	public Text scoreGuageText;
	public Text timeGuageText;
	public Text levelCompleteScreen;
	public Text continueText;
	public GameObject levelCompletePanel;
	public GameObject continuePanel;

	void Start()
	{
		continueText.gameObject.SetActive(false);
		levelCompleteScreen.gameObject.SetActive(false);
		continuePanel.SetActive(false);
		levelCompletePanel.SetActive(false);
	}

	void Update()
	{
		if (!isLevelComplete)
		{
			if (isTimerActive)
			{
				time += Time.deltaTime;
				UpdateTimeUI();
			}
		}
		else
		{
			if(hasReleasedThrottle && Input.GetAxis(lander.ThrustAxisName) != 0)
			{
				StartNextRound();
				isLevelComplete = false;
			}
			hasReleasedThrottle = Input.GetAxis(lander.ThrustAxisName) == 0;
		}
	}

	public void OnSuccessfulLanding(LanderBehaviour lander, float multiplier)
	{
		isLevelComplete = true;
		int points = (int)(50.0f * multiplier);
		totalPoints += points;
		isTimerActive = false;

		lander.ModifyFuel(50.0f);

		UpdateScoreUI();
		ShowSuccessScreen(points);
	}

	public void OnCrashLanding(LanderBehaviour lander)
	{
		isLevelComplete = true;
		isTimerActive = false;
		int fuelDeducted = (int)((Random.value * 200.0f) + 200.0f);
		lander.ModifyFuel(-fuelDeducted);

		if (lander.Fuel < 1)
		{
			ShowGameOverScreen();
		}
		else
		{
			ShowFailureScreen(fuelDeducted);
		}
	}

	private void StartNextRound()
	{
		isTimerActive = true;
		continuePanel.SetActive(false);
		levelCompletePanel.SetActive(false);
		continueText.gameObject.SetActive(false);
		levelCompleteScreen.gameObject.SetActive(false);

		bool isGameOver = lander.Fuel <= 0; 
		if(isGameOver)
		{
			totalPoints = 0;
			time = 0;
		}

		UpdateScoreUI();

		cam.ResetCamera();
		lander.ResetLander(isGameOver);
		landscapeGenerator.IncrementLevel(isGameOver);
	}

	private void UpdateScoreUI()
	{
		const string scorePrecision = "D9";
		scoreGuageText.text = totalPoints.ToString(scorePrecision);
	}

	private void ShowSuccessScreen(int points)
	{
		continuePanel.SetActive(true);
		levelCompletePanel.SetActive(true);
		continueText.gameObject.SetActive(true);
		levelCompleteScreen.gameObject.SetActive(true);
		levelCompleteScreen.text = "Congrats!\nPerfect Landing\n" + points + " points";
	}

	private void ShowFailureScreen(int fuelDeducted)
	{
		continuePanel.SetActive(true);
		levelCompletePanel.SetActive(true);
		continueText.gameObject.SetActive(true);
		levelCompleteScreen.gameObject.SetActive(true);
		string[] flavorText = new string[]
		{
			"you just destroyed a 15 dollar lander!",
			"you left an awesome 2 mile crater",
			"probably didn't hurt a bit",
			"brutal",
		};

		string message = flavorText[Random.Range(0, flavorText.Length)];
		message += "\nauxiliary fuel tanks destroyed\n" + fuelDeducted + " fuel units lost";
		levelCompleteScreen.text = message;
	}

	private void ShowGameOverScreen()
	{
		continuePanel.SetActive(true);
		levelCompletePanel.SetActive(true);
		continueText.gameObject.SetActive(true);
		levelCompleteScreen.gameObject.SetActive(true);

		levelCompleteScreen.text = "out of fuel\nFIN\n\ntime: " + getTimeString() + "\nscore: " + totalPoints.ToString();
	}

	private void UpdateTimeUI()
	{
		timeGuageText.text = getTimeString();
	}

	private string getTimeString()
	{
		const string timePrecision = "D2";
		return ((int)(time / 60)).ToString(timePrecision) + ":" + ((int)(time % 60)).ToString(timePrecision);
	}

	private bool hasReleasedThrottle = false;
	private bool isLevelComplete = false;
	private bool isTimerActive = true;
	private int totalPoints = 0;
	private float time = 0;
}
