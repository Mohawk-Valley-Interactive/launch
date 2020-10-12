using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody2D))]
public class LanderBehaviour : MonoBehaviour
{
	[Header("Support GameObjects")]
	public Transform spawnPoint = null;
	[Space(10)]

	[Header("Handling")]
	public float thrust = 1.0f;
	public float rotationSensitivity = 1.0f;
	[Space(10)]

	[Header("World Settings")]
	public Vector2 initialVelocity = new Vector2();
	[Space(10)]

	[Header("Landing Requirements")]
	public int angleOfApproachThreshold = 0;
	public int horizontalSpeedThreshold = 1;
	public int verticalSpeedThreshold = 1;
	[Space(10)]

	[Header("Audio & Visuals")]
	public GameObject landerAvatar;
	[Space(5)]
	public AudioSource successfulLandingSound;
	[Space(5)]
	public AudioSource explosionSound;
	public ParticleSystem explosionParticleSystem;
	[Space(5)]
	public float thrustSoundRamp = 0.1f;
	public AudioSource thrustSound;
	public ParticleSystem thrusterParticleSystem;

	[Header("UI Gauges")]
	public Text altitudeGuageText;
	public Text horizontalSpeedGuageText;
	public Text verticalSpeedGuageText;
	public Text angleOfApproachGuageText;

	void Start()
	{
		if (spawnPoint)
		{
			RepositionToSpawnPoint();
		}

		rigidbodyComponent = GetComponent<Rigidbody2D>();
		rigidbodyComponent.velocity = initialVelocity;

		if (thrustSound)
		{
			thrustSound.volume = 0;
			thrustSound.Play();
		}
	}

	void Update()
	{
		bool isThrusting = Input.GetAxis(thrustAxisName) != 0.0f;
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

		if (hasCrashed)
		{
			return;
		}

		CalculateStats();

		float actualThrust = Input.GetAxis(thrustAxisName) * thrust;
		rigidbodyComponent.AddRelativeForce(new Vector2(0.0f, actualThrust));

		rigidbodyComponent.angularVelocity = rigidbodyComponent.angularVelocity + Input.GetAxis(rotationAxisName) * -rotationSensitivity;
		rigidbodyComponent.rotation = Mathf.Clamp(rigidbodyComponent.rotation, -90.0f, 90.0f);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (hasCrashed)
		{
			return;
		}

		bool isBadLanding = isDangerousAngleOfApproach || isDangerousHorizontalVelocity || isDangerousVerticalVelocity;

		if (isBadLanding)
		{
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
		}
	}

	private void CalculateStats()
	{
		velocity = Mathf.Abs((rigidbodyComponent.velocity.magnitude * 100.0f));
		horizontalSpeed = Mathf.Abs((rigidbodyComponent.velocity.x * 100.0f));
		verticalSpeed = Mathf.Abs((rigidbodyComponent.velocity.y * 100.0f));
		angleOfApproach = Mathf.Abs(Vector3.Angle(transform.up, Vector3.up));

		Vector2 direction = new Vector2(0.0f, -1.0f);
		List<RaycastHit2D> results = new List<RaycastHit2D>(2);
		Physics2D.Raycast(transform.position, direction, new ContactFilter2D(), results);
		if (results.Count > 1)
		{
			float distance = results[1].point.y - results[0].point.y;
			altitude = Mathf.Abs(distance * 10.0f) - 1.0f;
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
		const string floatingPointPrecision = "F1";
		altitudeGuageText.text = altitude.ToString(floatingPointPrecision);
		angleOfApproachGuageText.text = angleOfApproach.ToString(floatingPointPrecision);
		horizontalSpeedGuageText.text = horizontalSpeed.ToString(floatingPointPrecision);
		verticalSpeedGuageText.text = verticalSpeed.ToString(floatingPointPrecision);

		horizontalSpeedGuageText.color = isDangerousHorizontalVelocity ? Color.red : Color.white;
		verticalSpeedGuageText.color = isDangerousVerticalVelocity ? Color.red : Color.white;
		angleOfApproachGuageText.color = isDangerousAngleOfApproach ? Color.red : Color.white;
	}

	private bool hasCrashed = false;
	private bool isDangerousHorizontalVelocity = false;
	private bool isDangerousVerticalVelocity = false;
	private bool isDangerousAngleOfApproach = false;

	private const string rotationAxisName = "Rotate";
	private const string thrustAxisName = "ApplyThrust";

	private Rigidbody2D rigidbodyComponent;

	private float altitude;
	private float angleOfApproach;
	private float horizontalSpeed;
	private float verticalSpeed;
	private float velocity;
}
