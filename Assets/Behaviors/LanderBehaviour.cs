using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class LanderBehaviour : MonoBehaviour
{
    private const string rotationAxisName = "Rotate";
    private const string thrustAxisName = "ApplyThrust";
    private Rigidbody rigidbodyComponent;
    private float angle = 0;

    public float thrust = 1.0f;
    public ForceMode forceMode = ForceMode.Acceleration;

    public float rotationSensitivity = 1.0f;
    public Vector3 initialVelocity = new Vector3();


    void Start()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        rigidbodyComponent.velocity = initialVelocity;

        angle = rigidbodyComponent.rotation.eulerAngles.z;
    }


    void FixedUpdate()
    {
        angle -= Input.GetAxis(rotationAxisName) * Time.fixedDeltaTime * rotationSensitivity;
        rigidbodyComponent.MoveRotation(Quaternion.AngleAxis(angle, Vector3.forward));

        float actualThrust = Input.GetAxis(thrustAxisName) * thrust;
        rigidbodyComponent.AddRelativeForce(0.0f, actualThrust, 0.0f, ForceMode.Acceleration);
    }
}
