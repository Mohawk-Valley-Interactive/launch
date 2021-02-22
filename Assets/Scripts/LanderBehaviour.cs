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
	public float rotationSensitivity = 1.0f;
	[Space(10)]

	[Header("World Settings")]
	public Vector2 initialVelocity = new Vector2();
	public float initialFuel = 3500.0f;
	[Space(10)]

	[Header("Landing Requirements")]
	public int angleOfApproachThreshold = 0;
	public int horizontalSpeedThreshold = 1;
	public int verticalSpeedThreshold = 1;
	public int lowFuelThreshold = 250;
	public int fuelNearEmptyThreshold = 100;
	public int fuelEmergencyThreshold = 50;
	[Space(10)]

	[Header("Audio & Visuals")]
	public GameObject landerAvatar;
	[Space(5)]
	public AudioSource successfulLandingSound;
	public float fuelNearEmptyDelay = 0.5f;
	public float fuelEmergencyDelay = 0.25f;
	public float lowFuelChimeDelay = 1.0f;
	public AudioSource lowFuelSound;
	public AudioSource explosionSound;
	public ParticleSystem explosionParticleSystem;
	[Space(5)]
	public float thrustSoundRamp = 0.1f;
	public AudioSource thrustSound;
	public ParticleSystem thrusterParticleSystem;
	[Space(10)]

	[Header("UI")]
	public Text fuelGuageText;
	public Text altitudeGuageText;
	public Text horizontalSpeedGuageText;
	public Text verticalSpeedGuageText;
	public Text angleOfApproachGuageText;

	public string RotationAxisName { get => rotationAxisName; }
	public string ThrustAxisName { get => thrustAxisName; }
	public float Altitude { get => altitude; }
	public float Fuel { get => fuel;  }
	public bool HasCrashed { get => hasCrashed; }
	public bool HasLanded { get => hasLanded; }
	public int LandingMultiplier { get => landingMultiplier; }

	public void ModifyFuel(float fuel)
	{
		this.fuel += fuel;
	}

	public void ResetLander(bool isGameOver)
	{
		RepositionToSpawnPoint();
		hasLanded = false;
		hasCrashed = false;
		rigidbodyComponent.simulated = true;
		rigidbodyComponent.velocity = initialVelocity;
		rigidbodyComponent.angularVelocity = 0.0f;
		landerAvatar.SetActive(true);

		if(isGameOver)
		{
			fuel = initialFuel;
		}
	}

	void Start()
	{
		if (spawnPoint)
		{
			RepositionToSpawnPoint();
		}

		rigidbodyComponent = GetComponent<Rigidbody2D>();
		rigidbodyComponent.velocity = initialVelocity;

		fuel = initialFuel;

		if (thrustSound)
		{
			thrustSound.volume = 0;
			thrustSound.Play();
		}

	}

	void Update()
	{
		if (hasCrashed || hasLanded)
		{
			return;
		}

		bool isThrusting = Input.GetAxis(thrustAxisName) != 0.0f && fuel > 0.0f;
		if (thrustSound)
		{
			if (isThrusting && thrustSound.volume < 1.0f)
			{
				thrustSound.volume = Mathf.Min(thrustSound.volume + (thrustSoundRamp * Time.deltaTime), 1.0f);
			}
			else if (!isThrusting && thrustSound.volume > 0.0f)
			{
				thrustSound.volume = Mathf.Max(thrustSound.volume - (thrustSoundRamp * Time.deltaTime), 0.0f);
			}
		}

		if (fuel < lowFuelThreshold)
		{
			if (timeSinceLastFuelChime < 0)
			{
				lowFuelSound.Play();
				if (fuel < fuelEmergencyThreshold)
				{
					timeSinceLastFuelChime = fuelEmergencyDelay;
				}
				else if (fuel < fuelNearEmptyThreshold)
				{
					timeSinceLastFuelChime = fuelNearEmptyDelay;
				}
				else
				{
					timeSinceLastFuelChime = lowFuelChimeDelay;
				}
			}
			else
			{
				timeSinceLastFuelChime -= Time.deltaTime;
			}
		}

		if (thrusterParticleSystem)
		{
			if (isThrusting && !thrusterParticleSystem.isEmitting)
			{
				thrusterParticleSystem.Play();
			}
			if (!isThrusting && thrusterParticleSystem.isEmitting)
			{
				thrusterParticleSystem.Stop();
			}
		}

		UpdateUiElements();
	}

	void FixedUpdate()
	{
		if (hasCrashed || hasLanded)
		{
			return;
		}

		CalculateStats();

		float actualThrust = fuel > 0.0f ? Input.GetAxis(thrustAxisName) * thrust * Time.fixedDeltaTime : 0.0f;
		fuel -= actualThrust;
		rigidbodyComponent.AddRelativeForce(new Vector2(0.0f, actualThrust * 100.0f));

		rigidbodyComponent.angularVelocity = rigidbodyComponent.angularVelocity + Input.GetAxis(rotationAxisName) * -rotationSensitivity;
		rigidbodyComponent.rotation = rigidbodyComponent.rotation > 360.0f ? rigidbodyComponent.rotation - 360.0f :
			rigidbodyComponent.rotation < 0.0f ? rigidbodyComponent.rotation + 360.0f : rigidbodyComponent.rotation;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (hasCrashed || hasLanded)
		{
			return;
		}

		bool isBadLanding = landingMultiplier == LandingZoneBehavior.DEFAULT_MULTIPLIER || isDangerousAngleOfApproach || isDangerousHorizontalVelocity || isDangerousVerticalVelocity;

		if (isBadLanding)
		{
			rigidbodyComponent.simulated = false;
			if (explosionParticleSystem)
			{
				explosionParticleSystem.Play();
			}

			if (explosionSound)
			{
				explosionSound.Play();
			}

			if (landerAvatar)
			{
				landerAvatar.SetActive(false);
			}

			if (thrusterParticleSystem && thrusterParticleSystem.isEmitting)
			{
				thrusterParticleSystem.Stop();
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
		landingMultiplier = LandingZoneBehavior.DEFAULT_MULTIPLIER;
	}

	private void CalculateStats()
	{
		horizontalSpeed = Mathf.Abs((rigidbodyComponent.velocity.x));
		verticalSpeed = Mathf.Abs((rigidbodyComponent.velocity.y));
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

		isDangerousHorizontalVelocity = horizontalSpeed >= horizontalSpeedThreshold;
		isDangerousVerticalVelocity = verticalSpeed >= verticalSpeedThreshold;
		isDangerousAngleOfApproach = angleOfApproach >= angleOfApproachThreshold;
	}

	private void RepositionToSpawnPoint()
	{
		this.transform.position = spawnPoint.position;
		this.transform.rotation = spawnPoint.rotation;
	}

	private void UpdateUiElements()
	{
		const string fuelPrecision = "D4";
		const string floatingPointPrecision = "F1";

		fuelGuageText.text = ((int)fuel).ToString(fuelPrecision);

		altitudeGuageText.text = altitude.ToString(floatingPointPrecision);
		angleOfApproachGuageText.text = angleOfApproach.ToString(floatingPointPrecision);
		horizontalSpeedGuageText.text = horizontalSpeed.ToString(floatingPointPrecision);
		verticalSpeedGuageText.text = verticalSpeed.ToString(floatingPointPrecision);

		fuelGuageText.color = fuel < lowFuelThreshold ? Color.red : Color.white;
		horizontalSpeedGuageText.color = isDangerousHorizontalVelocity ? Color.red : Color.white;
		verticalSpeedGuageText.color = isDangerousVerticalVelocity ? Color.red : Color.white;
		angleOfApproachGuageText.color = isDangerousAngleOfApproach ? Color.red : Color.white;
	}

	private bool hasCrashed = false;
	private bool hasLanded = false;
	private bool isDangerousHorizontalVelocity = false;
	private bool isDangerousVerticalVelocity = false;
	private bool isDangerousAngleOfApproach = false;

	private Rigidbody2D rigidbodyComponent;

	private const string rotationAxisName = "Rotate";
	private const string thrustAxisName = "ApplyThrust";
	private int landingMultiplier = LandingZoneBehavior.DEFAULT_MULTIPLIER;
	private float timeSinceLastFuelChime;
	private float fuel = 0;
	private float altitude;
	private float angleOfApproach;
	private float horizontalSpeed;
	private float verticalSpeed;
}
