using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBehavior : MonoBehaviour
{
	public LanderBehaviour lander;
	public float zoomInAltitudeThreshold = 500.0f;
	public float minCameraSize = 10.0f;
	public float sideBound = 50.0f;
	public float upperBound = 50.0f;
	public float lowerBound = 20.0f;
	public float zoomedSideBound = 100.0f;
	public float zoomedLowerBound = 100.0f;
	public float zoomSpeed = 1.0f;

	private void Start()
	{
		cam = GetComponent<Camera>();
		initialCameraSize = cam.orthographicSize;
		Vector3 position = transform.position;
		position.y = cam.orthographicSize;
		transform.position = position;
	}

	void Update()
	{
		if (lander.HasCrashed || lander.HasLanded)
			return;

		Vector3 newCameraPosition = transform.position;
		float zoom = zoomSpeed * Time.deltaTime;
		if (lander.Altitude < zoomInAltitudeThreshold)
		{
			if (zoomedLowerBound > lowerBoundActual)
			{
				lowerBoundActual = Mathf.Min(lowerBoundActual + zoom, zoomedLowerBound);
			}
			if (zoomedSideBound > sideBoundActual)
			{
				sideBoundActual = Mathf.Min(sideBoundActual + zoom, zoomedSideBound);
			}
			if (minCameraSize < cam.orthographicSize)
			{
				cam.orthographicSize = Mathf.Max(cam.orthographicSize - zoom, minCameraSize);
			}
		}
		else if (lander.Altitude > zoomInAltitudeThreshold)
		{
			lowerBoundActual = lowerBound;
			sideBoundActual = sideBound;
			if (initialCameraSize > cam.orthographicSize)
			{
				cam.orthographicSize = Mathf.Min(cam.orthographicSize + zoom, initialCameraSize);
			}
		}

		bool isOnRightSideOfCamera = lander.transform.position.x > transform.position.x;
		float cameraOffsetX = isOnRightSideOfCamera ?
			lander.transform.position.x - transform.position.x :
			transform.position.x - lander.transform.position.x;
		float boarderDistanceX = ((100.0f - sideBoundActual) * 0.01f) * cam.orthographicSize;
		if (cameraOffsetX > boarderDistanceX)
		{
			newCameraPosition.x = isOnRightSideOfCamera ? lander.transform.position.x - boarderDistanceX : lander.transform.position.x + boarderDistanceX;
		}

		float cameraOffsetY = lander.transform.position.y - transform.position.y;
		float upperBoarderDistanceY = (upperBound * 0.01f) * cam.orthographicSize;
		float lowerBoarderDistanceY = ((100.0f - lowerBoundActual) * 0.01f) * cam.orthographicSize;
		if (cameraOffsetY > 0.0f && cameraOffsetY > upperBoarderDistanceY)
		{
			newCameraPosition.y = lander.transform.position.y - upperBoarderDistanceY;
		}
		else if (cameraOffsetY < 0.0f && cameraOffsetY < -lowerBoarderDistanceY)
		{
			newCameraPosition.y = lander.transform.position.y + lowerBoarderDistanceY;
		}

		transform.position = newCameraPosition;
	}

	private Camera cam;
	private float initialCameraSize;
	public float lowerBoundActual;
	public float sideBoundActual;
}
