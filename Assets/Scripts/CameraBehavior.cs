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

    void Start()
    {
        cam = GetComponent<Camera>();
        initialCameraSize = cam.orthographicSize;
        Vector3 position = transform.position;
        position.y = cam.orthographicSize;
        transform.position = position;
        initialPosition = position;
    }

    public Vector2 outerBoundsBuffer = new Vector2(0.0f, 0.0f);
    public float floor = 100.0f;
    public float ceiling = 50.0f;
    public void DrawRect(Rect rect)
    {
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), Color.green);
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), Color.red);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), Color.green);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), Color.red);
    }
    void Update()
    {
        if (lander.HasCrashed || lander.HasLanded)
        {
            return;
        }

        float normalizedAltitude = Mathf.Clamp(lander.Altitude, floor, ceiling) / (ceiling - floor);
        normalizedAltitude = Mathf.Clamp(normalizedAltitude, 0.0f, 1.0f);
        float orthographicSize = initialCameraSize * normalizedAltitude;
        Camera mainCamera = Camera.main;
        mainCamera.orthographicSize = orthographicSize;

        Vector3 newCameraPosition = transform.position;
        // Rect bounds = new Rect(
        //     transform.position.x - Screen.width * 0.5f,
        //     transform.position.y - Screen.height * 0.5f,
        //     Screen.width,
        //     Screen.height);
        float worldSpaceWidth = mainCamera.orthographicSize * 2 * Screen.width / Screen.height;
        Rect bounds = new Rect(
            transform.position.x - worldSpaceWidth * 0.5f,
            transform.position.y - orthographicSize,
            worldSpaceWidth,
            orthographicSize * 2.0f);

        DrawRect(bounds);

        Vector2 landerPosition = lander.transform.position;
        bool isLanderAtLeftOuterBoundary = landerPosition.x < (bounds.x + outerBoundsBuffer.x);
        bool isLanderAtRightOuterBoundary = landerPosition.x > (bounds.xMax - outerBoundsBuffer.x);
        if (isLanderAtLeftOuterBoundary)
        {
            newCameraPosition.x = landerPosition.x + (bounds.width * 0.5f) - outerBoundsBuffer.x;
        }
        else if (isLanderAtRightOuterBoundary)
        {
            newCameraPosition.x = landerPosition.x - (bounds.width * 0.5f) + outerBoundsBuffer.x;
        }

        // float zoom = zoomSpeed * Time.deltaTime;
        // if (lander.Altitude < zoomInAltitudeThreshold)
        // {
        // 	if (zoomedLowerBound > lowerBoundActual)
        // 	{
        // 		lowerBoundActual = Mathf.Min(lowerBoundActual + zoom, zoomedLowerBound);
        // 	}
        // 	if (zoomedSideBound > sideBoundActual)
        // 	{
        // 		sideBoundActual = Mathf.Min(sideBoundActual + zoom, zoomedSideBound);
        // 	}
        // 	if (minCameraSize < cam.orthographicSize)
        // 	{
        // 		cam.orthographicSize = Mathf.Max(cam.orthographicSize - zoom, minCameraSize);
        // 	}
        // }
        // else if (lander.Altitude > zoomInAltitudeThreshold)
        // {
        // 	lowerBoundActual = lowerBound;
        // 	sideBoundActual = sideBound;
        // 	if (initialCameraSize > cam.orthographicSize)
        // 	{
        // 		cam.orthographicSize = Mathf.Min(cam.orthographicSize + zoom, initialCameraSize);
        // 	}
        // }

        // bool isOnRightSideOfCamera = lander.transform.position.x > transform.position.x;
        // float cameraOffsetX = isOnRightSideOfCamera ?
        // 	lander.transform.position.x - transform.position.x :
        // 	transform.position.x - lander.transform.position.x;
        // float boarderDistanceX = ((100.0f - sideBoundActual) * 0.01f) * cam.orthographicSize;
        // if (cameraOffsetX > boarderDistanceX)
        // {
        // 	newCameraPosition.x = isOnRightSideOfCamera ? lander.transform.position.x - boarderDistanceX : lander.transform.position.x + boarderDistanceX;
        // }

        // float cameraOffsetY = lander.transform.position.y - transform.position.y;
        // float upperBoarderDistanceY = (upperBound * 0.01f) * cam.orthographicSize;
        // float lowerBoarderDistanceY = ((100.0f - lowerBoundActual) * 0.01f) * cam.orthographicSize;
        // if (cameraOffsetY > 0.0f && cameraOffsetY > upperBoarderDistanceY)
        // {
        // 	newCameraPosition.y = lander.transform.position.y - upperBoarderDistanceY;
        // }
        // else if (cameraOffsetY < 0.0f && cameraOffsetY < -lowerBoarderDistanceY)
        // {
        // 	newCameraPosition.y = lander.transform.position.y + lowerBoarderDistanceY;
        // }

        // newCameraPosition.y = Mathf.Clamp(newCameraPosition.y, cam.orthographicSize, cam.orthographicSize * 2.0f);

        transform.position = newCameraPosition;
    }

    public void ResetCamera()
    {
        cam.orthographicSize = initialCameraSize;
        transform.position = initialPosition;
    }

    private Camera cam;
    private Vector3 initialPosition;
    private float initialCameraSize;
    public float lowerBoundActual;
    public float sideBoundActual;
}
