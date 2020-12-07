using System.Collections.Generic;
using LaunchDarkly.Client;
using LaunchDarkly.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
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
	[Space(5)]

	[Header("Feature Flags")]
	public string gravityFeatureFlagName = "gravity";
	public float defaultGravityValue = -1.62f;
	private float flagGravityValue;
	private bool hasNewFlagGravityValue = false;
	[Space(1)]
	public string failedLandingFlavorTextFlagName = "failed-landing-flavor-text";
	public string[] defaultFailedLandingFlavorText = { "Flavor text belongs here..." };
	private string[] flavorText = { "flavor text unset" };
	[Space(1)]
	public string additionalFuelFlagName = "fuel-reward";
	public float defaultFuelReward = 50.0f;
	public float fuelReward = 50.0f;


	void Start()
	{
		continueText.gameObject.SetActive(false);
		levelCompleteScreen.gameObject.SetActive(false);
		continuePanel.SetActive(false);
		levelCompletePanel.SetActive(false);

		ClientBehavior.Instance.RegisterFeatureFlagChangedCallback(gravityFeatureFlagName, LdValue.Of(defaultGravityValue), OnGravityFeatureFlagChanged, true);
		ClientBehavior.Instance.RegisterFeatureFlagChangedCallback(additionalFuelFlagName, LdValue.Of(defaultFuelReward), OnFuelRewardFeatureFlagChanged, true);
		ClientBehavior.Instance.RegisterFeatureFlagChangedCallback(failedLandingFlavorTextFlagName, MakeLdValueFromStringArray(defaultFailedLandingFlavorText), OnFailedLandingFlavorTextFeatureFlagChanged, true);
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
			if (hasReleasedThrottle && Input.GetAxis(lander.ThrustAxisName) != 0)
			{
				StartNextRound();
				isLevelComplete = false;
			}
			hasReleasedThrottle = Input.GetAxis(lander.ThrustAxisName) == 0;
		}

		if (hasNewFlagGravityValue)
		{
			Physics2D.gravity = new Vector2(0.0f, -flagGravityValue);
			hasNewFlagGravityValue = false;
		}
	}

	public void OnSuccessfulLanding(LanderBehaviour lander, float multiplier)
	{
		isLevelComplete = true;
		int points = (int)(50.0f * multiplier);
		totalPoints += points;
		isTimerActive = false;

		lander.ModifyFuel(fuelReward);

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
		bool isGameOver = lander.Fuel <= 0;
		if (isGameOver)
		{
			SceneManager.LoadScene("LaunchScene");
			totalPoints = 0;
			time = 0;
		}

		isTimerActive = true;
		continuePanel.SetActive(false);
		levelCompletePanel.SetActive(false);
		continueText.gameObject.SetActive(false);
		levelCompleteScreen.gameObject.SetActive(false);

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

	// BEGIN - Feature flag support
	private LdValue MakeLdValueFromStringArray(string[] stringArray)
	{
		LdValue.ArrayBuilder arrayBuilder = LdValue.BuildArray();
		foreach (string s in stringArray)
		{
			arrayBuilder.Add(s);
		}

		return arrayBuilder.Build();
	}

	private void OnFuelRewardFeatureFlagChanged(LdValue value)
	{
		fuelReward = value.AsFloat;
	}

	private void OnGravityFeatureFlagChanged(LdValue value)
	{
		flagGravityValue = value.AsFloat;
		hasNewFlagGravityValue = true;
	}

	private void OnFailedLandingFlavorTextFeatureFlagChanged(LdValue value)
	{
		IReadOnlyList<string> strings = value.AsList(LdValue.Convert.String);
		flavorText = new string[strings.Count];
		for(int i = 0; i < strings.Count; i++)
		{
			flavorText[i] = strings[i];	
		}
	}
	// END - Feature flag support

	private bool hasReleasedThrottle = false;
	private bool isLevelComplete = false;
	private bool isTimerActive = true;
	private int totalPoints = 0;
	private float time = 0;
}
