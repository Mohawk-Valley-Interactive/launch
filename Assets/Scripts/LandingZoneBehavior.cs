﻿using UnityEngine;
using UnityEngine.UI;

public class LandingZoneBehavior : MonoBehaviour
{
	public const int DEFAULT_MULTIPLIER = 1;
	public int Multiplier = DEFAULT_MULTIPLIER;

	public Text text;

	void Awake() {
	    text.text = Multiplier.ToString() + "x";
	}
}
