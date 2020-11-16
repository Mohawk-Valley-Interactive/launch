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

		//      if(lander.Altitude < zoomInAltitudeThreshold)
		//{
		//          newCameraPosition.y = lander.transform.position.y;
		//          cam.orthographicSize = minCameraSize;
		//}
		//      else if(lander.transform.position.y > (initialCameraSize * 2.0f))
		//{
		//          newCameraPosition.y = lander.transform.position.y - initialCameraSize;
		//          cam.orthographicSize = initialCameraSize;
		//}
		//      else
		//{
		//          newCameraPosition.y = initialCameraSize;
		//          cam.orthographicSize = initialCameraSize;
		//}

		float cameraOffsetX = lander.transform.position.x - transform.position.x;
		float cameraDistanceX = Mathf.Abs(cameraOffsetX);
		float boarderDistanceX = (sideBound * 0.01f) * cam.orthographicSize;
		if (cameraDistanceX > boarderDistanceX)
		{
			if (cameraOffsetX < 0.0f)
			{
				newCameraPosition.x = lander.transform.position.x + boarderDistanceX;
			}
			else
			{
				newCameraPosition.x = lander.transform.position.x - boarderDistanceX;
			}
		}

		float cameraOffsetY = lander.transform.position.y - transform.position.y;
		float upperBoarderDistanceY = (upperBound * 0.01f) * cam.orthographicSize;
		float lowerBoarderDistanceY = ((100.0f - lowerBound) * 0.01f) * cam.orthographicSize;
		Debug.Log(cameraOffsetY + " | " + upperBoarderDistanceY + " | " + lowerBoarderDistanceY);
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
}
