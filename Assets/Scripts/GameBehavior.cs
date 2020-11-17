using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBehavior : MonoBehaviour
{
	[Header("UI Gauges")]
	public Text scoreGuageText;
	public Text timeGuageText;
	public Text levelCompleteScreen;
	public Text continueText;
	public float parTime = 60000;

	private void Start()
	{
		continueText.gameObject.SetActive(false);
		levelCompleteScreen.gameObject.SetActive(false);
	}

	void Update()
	{
		if (isTimerActive)
		{
			time += Time.deltaTime;
			UpdateTimeUI();
		}
	}

	public void OnSuccessfulLanding(LanderBehaviour lander, float multiplier)
	{
		int points = (int)(50.0f * multiplier);
		totalPoints += points;
		isTimerActive = false;

		lander.ModifyFuel(50.0f);

		UpdateScoreUI();
		ShowSuccessScreen(points);
	}

	public void OnCrashLanding(LanderBehaviour lander)
	{
		isTimerActive = false;
		int fuelDeducted = (int)((Random.value * 200.0f) + 200.0f);
		lander.ModifyFuel(-fuelDeducted);

		if(lander.Fuel < 1)
		{
			ShowGameOverScreen();
		} 
		else
		{
			ShowFailureScreen(fuelDeducted);
		}
	}

	private void UpdateScoreUI()
	{
		const string scorePrecision = "D9";
		scoreGuageText.text = totalPoints.ToString(scorePrecision);
	}

	private void ShowSuccessScreen(int points)
	{
		continueText.gameObject.SetActive(true);
		levelCompleteScreen.gameObject.SetActive(true);
		levelCompleteScreen.text = "Congrats!\nPerfect Landing\n" + points + " points";
	}

	private void ShowFailureScreen(int fuelDeducted)
	{
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
		continueText.gameObject.SetActive(true);
		levelCompleteScreen.gameObject.SetActive(true);

		levelCompleteScreen.text = "out of fuel\nFIN";
	}

	private void UpdateTimeUI()
	{
		const string timePrecision = "D2";
		timeGuageText.text = ((int)(time / 60)).ToString(timePrecision) + ":" + ((int)(time % 60)).ToString(timePrecision);
	}

	private bool isTimerActive = true;
	private int totalPoints = 0;
	private float time = 0;
}
