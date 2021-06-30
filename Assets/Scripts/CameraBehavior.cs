using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
	public Camera landerCam;
	public LanderBehaviour lander;
	public Vector2 outerBoundsBuffer = new Vector2(0.0f, 0.0f);
	public float zoomHighAltitudeThreshold = 500.0f;
	public float zoomLowAltitudeThreshold = 100.0f;
	public float zoomedOrthographicSize = 100.0f;

	void Start()
	{
		Vector3 position = transform.position;
		landscapeOrthographicSize = landerCam.orthographicSize;
		position.y = landscapeOrthographicSize;
		transform.position = position;
		initialPosition = position;
	}

	void Update()
	{
		if (lander.HasCrashed || lander.HasLanded)
		{
			return;
		}

		zoomBias = 1.0f - Mathf.Clamp01((lander.Altitude - zoomLowAltitudeThreshold) / (zoomHighAltitudeThreshold - zoomLowAltitudeThreshold));
		DrawAltitudeRange();

		Vector2 adjustedBuffer = outerBoundsBuffer * 0.5f;
		Vector3 landscapeCameraPosition = transform.position;
		Vector3 zoomedCameraPosition = lander.transform.position;
		if (zoomedCameraPosition.y - zoomedOrthographicSize < 0)
		{
			zoomedCameraPosition.y = zoomedOrthographicSize;
		}
		zoomedCameraPosition.z = transform.position.z;

		landerCam.orthographicSize = Mathf.Lerp(landscapeOrthographicSize, zoomedOrthographicSize, zoomBias);

		float landscapeWorldSpaceWidth = landscapeOrthographicSize * 2 * Screen.width / Screen.height;
		Rect landscapeBounds = new Rect(
			landscapeCameraPosition.x - landscapeWorldSpaceWidth * 0.5f,
			landscapeCameraPosition.y - landscapeOrthographicSize,
			landscapeWorldSpaceWidth,
			landscapeOrthographicSize * 2.0f);
		DrawRect(landscapeBounds);

		float boundsLeft = Mathf.Min(landscapeBounds.x + adjustedBuffer.x, landscapeBounds.xMax - adjustedBuffer.x);
		float boundsRight = Mathf.Max(landscapeBounds.x + adjustedBuffer.x, landscapeBounds.xMax - adjustedBuffer.x);
		bool isLanderAtLeftOuterBoundary = zoomedCameraPosition.x < boundsLeft;
		bool isLanderAtRightOuterBoundary = zoomedCameraPosition.x > boundsRight;
		if (isLanderAtLeftOuterBoundary)
		{
			landscapeCameraPosition.x = zoomedCameraPosition.x + ((boundsRight - boundsLeft) * 0.5f);
		}
		else if (isLanderAtRightOuterBoundary)
		{
			landscapeCameraPosition.x = zoomedCameraPosition.x - ((boundsRight - boundsLeft) * 0.5f);
		}

		float zoomedSpaceWidth = zoomedOrthographicSize * 2 * Screen.width / Screen.height;
		Rect zoomedBounds = new Rect(
			zoomedCameraPosition.x - zoomedSpaceWidth * 0.5f,
			zoomedCameraPosition.y - zoomedOrthographicSize,
			zoomedSpaceWidth,
			zoomedOrthographicSize * 2.0f);
		DrawRect(zoomedBounds);

		transform.position = landscapeCameraPosition;
		Vector3 cameraRigPosition = landscapeCameraPosition + (zoomBias * (zoomedCameraPosition - landscapeCameraPosition));
		Debug.DrawLine(landscapeCameraPosition, cameraRigPosition, Color.cyan);
		landerCam.transform.SetPositionAndRotation(cameraRigPosition, landerCam.transform.rotation);
	}

	public void ResetCamera()
	{
		transform.position = initialPosition;
	}

	private Camera cam;

	private Vector3 initialPosition;
	private float landscapeOrthographicSize;
	private float zoomBias = 0.0f;

	private void DrawAltitudeRange()
	{
		Debug.DrawLine(
			new Vector3(lander.transform.position.x - 10.0f, lander.transform.position.y - zoomLowAltitudeThreshold),
			new Vector3(lander.transform.position.x + 10.0f, lander.transform.position.y - zoomLowAltitudeThreshold),
			Color.red);
		Debug.DrawLine(
			new Vector3(lander.transform.position.x - 10.0f, lander.transform.position.y - zoomHighAltitudeThreshold),
			new Vector3(lander.transform.position.x + 10.0f, lander.transform.position.y - zoomHighAltitudeThreshold),
			Color.green);
		Debug.DrawLine(
			new Vector3(lander.transform.position.x - 10.0f, lander.transform.position.y - zoomLowAltitudeThreshold - (zoomBias * (zoomHighAltitudeThreshold - zoomLowAltitudeThreshold))),
			new Vector3(lander.transform.position.x + 10.0f, lander.transform.position.y - zoomLowAltitudeThreshold - (zoomBias * (zoomHighAltitudeThreshold - zoomLowAltitudeThreshold))),
			Color.magenta);
		Debug.DrawLine(
			new Vector3(lander.transform.position.x, lander.transform.position.y - zoomHighAltitudeThreshold),
			new Vector3(lander.transform.position.x, lander.transform.position.y - zoomLowAltitudeThreshold),
			Color.magenta);
	}

	private void DrawRect(Rect rect)
	{
		Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), Color.green);
		Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), Color.red);
		Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), Color.green);
		Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), Color.red);
	}

}
