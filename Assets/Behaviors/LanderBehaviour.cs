using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class LanderBehaviour : MonoBehaviour
{
	public float thrust = 1.0f;
	public float rotationSensitivity = 1.0f;
	public Vector2 initialVelocity = new Vector2();
	public float gravity = 1.62f;
	public float thrustSoundRamp = 0.1f;

	public AudioSource thrustSound;
	public ParticleSystem thrusterParticleSystem;


	void Start()
	{
		rigidbodyComponent = GetComponent<Rigidbody2D>();
		rigidbodyComponent.velocity = initialVelocity;

		gravityActual = gravity;
		Physics2D.gravity = new Vector2(0.0f, -gravityActual);
		if (thrustSound)
		{
			thrustSound.volume = 0;
			thrustSound.Play();
		}

		if (thrusterParticleSystem)
		{
			thrusterParticleSystem.Stop();
		}
	}

	void Update()
	{
		if (gravityActual != gravity)
		{
			Physics2D.gravity = new Vector2(0.0f, -gravityActual);
		}


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
	}

	void FixedUpdate()
	{
		float actualThrust = Input.GetAxis(thrustAxisName) * thrust;
		rigidbodyComponent.AddRelativeForce(new Vector2(0.0f, actualThrust));

		rigidbodyComponent.angularVelocity = rigidbodyComponent.angularVelocity + Input.GetAxis(rotationAxisName) * -rotationSensitivity;
		rigidbodyComponent.rotation = Mathf.Clamp(rigidbodyComponent.rotation, -90.0f, 90.0f);
	}

	private const string rotationAxisName = "Rotate";
	private const string thrustAxisName = "ApplyThrust";
	private float gravityActual = 0.0f;
	private Rigidbody2D rigidbodyComponent;
}
