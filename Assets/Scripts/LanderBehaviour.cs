using LaunchDarkly.Client;
using LaunchDarkly.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class LanderBehaviour : MonoBehaviour
{
	[Header("Support GameObjects")]
	public Transform spawnPoint = null;
	public GameBehavior gameBehavior = null;
	[Space(10)]

	[Header("Handling")]
	public float thrust = 1.0f;
	public float maxVelocity = 10.0f;
	public float rotationSensitivity = 1.0f;
	[Space(10)]

	[Header("World Settings")]
	public Vector2 initialVelocity = new Vector2();
	public float initialFuel = 3500.0f;
	public string startingFuelFlagName = "starting-fuel";
	[Space(10)]

	[Header("Landing Requirements")]
	public Color normalColor = Color.white;
	public Color dangerColor = Color.red;
	public int angleOfApproachThreshold = 0;
	public string angleOfApproachThresholdFlagName = "angle-of-approach-threshold";
	public int lowAltitudeThreshold = 250;
	public string lowAltitudeThresholdFlagName = "low-altitude-threshold";
	public int velocityThreshold = 1;
	public string velocityThresholdFlagName = "velocity-threshold";
	public int verticalSpeedThreshold = 1;
	public int lowFuelThreshold = 250;
	public string lowFuelThresholdFlagName = "low-fuel-threshold";
	public int fuelNearEmptyThreshold = 100;
	public int fuelEmergencyThreshold = 50;
	[Space(10)]

	[Header("Audio & Visuals")]
	public GameObject landerAvatar;
	[Space(5)]
	public AudioSource successfulLandingSound;
	public float fuelNearEmptyDelay = 0.5f;
	public float fuelEmergencyDelay = 0.25f;
	public AudioSource lowFuelSound;
	public AudioSource explosionSound;
	public ExplosionBehavior explosionBehavior;
	[Space(5)]
	public float thrustSoundRamp = 0.1f;
	public AudioSource thrustSound;
	public ThrusterBehavior thrusterBehavior;
	public string audioLevelsFlagName = "audio-levels";
	public float fxVolume = 1.0f;
	[Space(10)]

	[Header("UI")]
	public Text fuelGuageText;
	public Text altitudeGuageText;
	public Text velocityGuageText;
	public Text angleOfApproachGuageText;

	public MusicPlayerBehavior musicPlayer;

	public string RotationAxisName { get => rotationAxisName; }
	public string ThrustAxisName { get => thrustAxisName; }
	public float Altitude { get => altitude; }
	public float Fuel { get => fuel; }
	public bool HasCrashed { get => hasCrashed; }
	public bool HasLanded { get => hasLanded; }
	public int LandingMultiplier { get => landingMultiplier; }

	public float ceiling = 500.0f;

	public void ModifyFuel(float fuel)
	{
		this.fuel += fuel;
	}

	public bool HasRunFirstUpdate()
	{
		return hasRunFirstUpdate;
	}

	public void ResetLander(bool isGameOver)
	{
		RepositionToSpawnPoint();
		hasLanded = false;
		hasCrashed = false;
		timeSinceLastFuelChime = -1.0f;
		rigidbodyComponent.simulated = true;
		rigidbodyComponent.velocity = initialVelocity;
		rigidbodyComponent.angularVelocity = 0.0f;
		landerAvatar.SetActive(true);

		if (isGameOver)
		{
			fuel = LaunchDarklyClientBehavior.Instance.FloatVariation(startingFuelFlagName, initialFuel);
		}
	}

	void Awake()
	{
		LdValue defaultValue = LdValue.BuildObject()
			.Add("fx", 1.0f)
			.Build();
		LaunchDarklyClientBehavior.Instance.RegisterFeatureFlagChangedCallback(
			audioLevelsFlagName, defaultValue, OnAudioLevelsFlagChanged, true);
	}

	void Start()
	{
		if (spawnPoint)
		{
			RepositionToSpawnPoint();
		}

		rigidbodyComponent = GetComponent<Rigidbody2D>();
		rigidbodyComponent.velocity = initialVelocity;

		fuel = LaunchDarklyClientBehavior.Instance.FloatVariation(startingFuelFlagName, initialFuel);

		if (thrustSound)
		{
			thrustSound.volume = 0;
			thrustSound.Play();
		}

		LdValue defaultValue = LdValue.BuildObject()
			.Add("fx", 1.0f)
			.Build();
		LdValue audioLevels = LaunchDarklyClientBehavior.Instance.JsonVariation(audioLevelsFlagName, defaultValue);
		IReadOnlyDictionary<string, float> al = audioLevels.AsDictionary<float>(LdValue.Convert.Float);
		fxVolumeInternal = al.ContainsKey("fx") ? al["fx"] : 1.0f;
	}

	void Update()
	{
		if (hasCrashed || hasLanded)
		{
			return;
		}

		if (fxVolumeInternal != fxVolumeActual)
		{
			fxVolumeActual = fxVolumeInternal;
			thrustSound.volume = fxVolumeActual;
			explosionSound.volume = fxVolumeActual;
			lowFuelSound.volume = fxVolumeActual;
			explosionSound.volume = fxVolumeActual;
			successfulLandingSound.volume = fxVolumeActual;
		}

		bool isThrusting = Input.GetAxis(thrustAxisName) != 0.0f && fuel > 0.0f;
		if (thrustSound)
		{
			if (isThrusting && thrustSound.volume < fxVolumeActual)
			{
				thrustSound.volume = Mathf.Min(thrustSound.volume + (thrustSoundRamp * Time.deltaTime), fxVolumeActual);
			}
			else if (!isThrusting && thrustSound.volume > 0.0f)
			{
				thrustSound.volume = Mathf.Max(thrustSound.volume - (thrustSoundRamp * Time.deltaTime), 0.0f);
			}
		}

		float lowFuelThresholdActual = LaunchDarklyClientBehavior.Instance.FloatVariation(lowFuelThresholdFlagName, lowFuelThreshold);
		if (fuel < lowFuelThresholdActual)
		{
			if (timeSinceLastFuelChime < 0)
			{
				timeSinceLastFuelChime = 0.0f;
				lowFuelSound.Play();
				if (musicPlayer)
				{
					musicPlayer.OnLowFuel();
				}
			}
		}

		if (thrusterBehavior)
		{
			if (isThrusting && !thrusterBehavior.isEmitting)
			{
				thrusterBehavior.Play();
			}
			if (!isThrusting && thrusterBehavior.isEmitting)
			{
				thrusterBehavior.Stop();
			}
		}

		hasRunFirstUpdate = true;

		UpdateMusic();
		UpdateUiElements();
	}

	void FixedUpdate()
	{
		if (hasCrashed || hasLanded)
		{
			return;
		}

		if (transform.position.y > ceiling)
		{
			transform.position = new Vector2(transform.position.x, ceiling);
			rigidbodyComponent.velocity = new Vector2(rigidbodyComponent.velocity.x, 0.0f);
		}

		CalculateStats();

		float actualThrust = fuel > 0.0f ? Input.GetAxis(thrustAxisName) * thrust * Time.fixedDeltaTime : 0.0f;
		fuel -= actualThrust;
		rigidbodyComponent.AddRelativeForce(new Vector2(0.0f, actualThrust * 100.0f));

		rigidbodyComponent.angularVelocity = rigidbodyComponent.angularVelocity + Input.GetAxis(rotationAxisName) * -rotationSensitivity;
		rigidbodyComponent.rotation = rigidbodyComponent.rotation > 360.0f ? rigidbodyComponent.rotation - 360.0f :
			rigidbodyComponent.rotation < 0.0f ? rigidbodyComponent.rotation + 360.0f : rigidbodyComponent.rotation;
	}

	public void OnAudioLevelsFlagChanged(LdValue audioLevels)
	{
		IReadOnlyDictionary<string, float> al = audioLevels.AsDictionary<float>(LdValue.Convert.Float);
		fxVolumeInternal = al.ContainsKey("fx") ? al["fx"] : 1.0f;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (hasCrashed || hasLanded)
		{
			return;
		}

		bool isBadLanding = landingMultiplier < LandingZoneBehavior.DEFAULT_MULTIPLIER || isDangerousAngleOfApproach || isDangerousVelocity;

		if (isBadLanding)
		{
			rigidbodyComponent.simulated = false;
			if (explosionBehavior)
			{
				explosionBehavior.Play();
			}

			if (explosionSound)
			{
				explosionSound.Play();
			}

			if (landerAvatar)
			{
				landerAvatar.SetActive(false);
			}

			if (thrusterBehavior && thrusterBehavior.isEmitting)
			{
				thrusterBehavior.Stop();
			}

			if (thrustSound)
			{
				thrustSound.volume = 0.0f;
			}

			hasCrashed = true;
			gameBehavior.OnCrashLanding(this);
		}
		else
		{
			hasLanded = true;
			rigidbodyComponent.simulated = false;
			successfulLandingSound.Play();

			gameBehavior.OnSuccessfulLanding(this, landingMultiplier);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		LandingZoneBehavior lzb = collision.gameObject.GetComponent<LandingZoneBehavior>();
		landingMultiplier = lzb.Multiplier;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		landingMultiplier = LandingZoneBehavior.DEFAULT_MULTIPLIER - 1;
	}

	private void CalculateStats()
	{
		velocity = Mathf.Abs((rigidbodyComponent.velocity.magnitude));
		angleOfApproach = Mathf.Abs(Vector3.Angle(transform.up, Vector3.up));

		Vector2 direction = new Vector2(0.0f, -1.0f);
		List<RaycastHit2D> results = new List<RaycastHit2D>(2);
		Physics2D.Raycast(transform.position, direction, new ContactFilter2D(), results);
		if (results.Count > 1)
		{
			float distance = results[1].point.y - results[0].point.y;
			altitude = Mathf.Abs(distance) - 1.0f;
		}
		else
		{
			altitude = Mathf.Infinity;
		}

		float velocityThresholdActual = LaunchDarklyClientBehavior.Instance.FloatVariation(velocityThresholdFlagName, velocityThreshold);
		float angleOfApproachThresholdActual = LaunchDarklyClientBehavior.Instance.FloatVariation(angleOfApproachThresholdFlagName, angleOfApproachThreshold);
		float lowAltitudeThresholdActual = LaunchDarklyClientBehavior.Instance.FloatVariation(lowAltitudeThresholdFlagName, lowAltitudeThreshold);
		isDangerousVelocity = velocity >= velocityThresholdActual;
		isDangerousAngleOfApproach = angleOfApproach >= angleOfApproachThresholdActual;
		isLowAltitude = altitude < lowAltitudeThresholdActual;
	}

	private void RepositionToSpawnPoint()
	{
		transform.position = spawnPoint.position;
		transform.rotation = spawnPoint.rotation;
	}

	private void UpdateMusic()
	{
		if (musicPlayer == null)
		{
			return;
		}

		float lowFuelThresholdActual = LaunchDarklyClientBehavior.Instance.FloatVariation(lowFuelThresholdFlagName, lowFuelThreshold);
		if (fuel < lowFuelThresholdActual)
		{
			musicPlayer.OnLowFuel();
		}
		if (isLowAltitude)
		{
			musicPlayer.onLowAltitudeEntered();
			if (isDangerousAngleOfApproach || isDangerousVelocity)
			{
				musicPlayer.onBadAngleEntered();
				musicPlayer.onHighVelocityEntered();
			}
			else
			{
				musicPlayer.onBadAngleExited();
				musicPlayer.onHighVelocityExited();
			}
		}
		else
		{
			musicPlayer.onLowAltitudeExited();
			musicPlayer.onBadAngleExited();
			musicPlayer.onHighVelocityExited();
		}
	}

	private void UpdateUiElements()
	{
		const string fuelPrecision = "D4";
		const string floatingPointPrecision = "F1";

		fuelGuageText.text = ((int)fuel).ToString(fuelPrecision);

		altitudeGuageText.text = altitude.ToString(floatingPointPrecision);
		angleOfApproachGuageText.text = angleOfApproach.ToString(floatingPointPrecision);
		velocityGuageText.text = velocity.ToString(floatingPointPrecision);

		float lowFuelThresholdActual = LaunchDarklyClientBehavior.Instance.FloatVariation(lowFuelThresholdFlagName, lowFuelThreshold);
		fuelGuageText.color = fuel < lowFuelThresholdActual ? dangerColor : normalColor;
		velocityGuageText.color = isDangerousVelocity ? dangerColor : normalColor;
		angleOfApproachGuageText.color = isDangerousAngleOfApproach ? dangerColor : normalColor;
	}

	private bool hasCrashed = false;
	private bool hasLanded = false;
	private bool isDangerousVelocity = false;
	private bool isDangerousAngleOfApproach = false;
	private bool isLowAltitude = false;

	private Rigidbody2D rigidbodyComponent;

	private const string rotationAxisName = "Rotate";
	private const string thrustAxisName = "ApplyThrust";
	private int landingMultiplier = LandingZoneBehavior.DEFAULT_MULTIPLIER;
	private float timeSinceLastFuelChime = -1.0f;
	private float fuel = 0;
	private float altitude;
	private float angleOfApproach;
	private float velocity;
	private float verticalSpeed;

	private bool hasRunFirstUpdate = false;

	private float fxVolumeActual;
	private float fxVolumeInternal;
}
