using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LanderCameraBehavior : MonoBehaviour
{
    public LanderBehaviour lander;
    public float zoomInAltitudeThreshold = 500.0f;
    public float minCameraSize = 10.0f;

	private void Start()
	{
        camera = GetComponent<Camera>();
        initialCameraSize = camera.orthographicSize;
        Vector3 position = transform.position;
        position.y = camera.orthographicSize;
        transform.position = position;
	}

	void Update()
    {
        if (lander.HasCrashed || lander.HasLanded)
            return;

        Vector3 position = this.transform.position;
        position.x = lander.transform.position.x;
        if(lander.Altitude < zoomInAltitudeThreshold)
		{
            position.y = lander.transform.position.y;
            camera.orthographicSize = minCameraSize;
		}
        else if(lander.transform.position.y > (initialCameraSize * 2.0f))
		{
            position.y = lander.transform.position.y - initialCameraSize;
            camera.orthographicSize = initialCameraSize;
		}
        else
		{
            position.y = initialCameraSize;
            camera.orthographicSize = initialCameraSize;
		}

        this.transform.position = position;
    }

    private Camera camera;
    private float initialCameraSize;

    private void ZoomIn()
	{

	}
}
