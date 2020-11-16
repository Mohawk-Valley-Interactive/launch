using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBehavior : MonoBehaviour
{
	[Header("UI Gauges")]
	public Text scoreGuageText;
	public Text timeGuageText;
	public float parTime = 60000;

	public void OnSuccessfulLanding(float fuel, float multiplier)
	{
		totalPoints += (int)((fuel + (parTime - time)) * multiplier);
		isTimerActive = false;

		UpdateScoreUI();
	}

	private void Update()
	{
		if (isTimerActive)
		{
			time += Time.deltaTime;
			UpdateTimeUI();
		}
	}

	private void UpdateScoreUI()
	{
		const string scorePrecision = "D9";
		scoreGuageText.text = totalPoints.ToString(scorePrecision);
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
