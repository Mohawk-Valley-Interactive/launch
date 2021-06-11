using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBehavior : MonoBehaviour
{
    public LanderBehaviour lander;
    public Vector2 outerBoundsBuffer = new Vector2(0.0f, 0.0f);

    void Start()
    {
        cam = GetComponent<Camera>();
        Vector3 position = transform.position;
        position.y = cam.orthographicSize;
        transform.position = position;
        initialPosition = position;
    }

    void Update()
    {
        if (lander.HasCrashed || lander.HasLanded)
        {
            return;
        }

        float worldSpaceWidth = cam.orthographicSize * 2 * Screen.width / Screen.height;
        Rect bounds = new Rect(
            transform.position.x - worldSpaceWidth * 0.5f,
            transform.position.y - cam.orthographicSize,
            worldSpaceWidth,
            cam.orthographicSize * 2.0f);

        DrawRect(bounds);

        Vector3 newCameraPosition = transform.position;
        Vector3 landerPosition = lander.transform.position;
        Vector2 adjustedBuffer = outerBoundsBuffer * 0.5f;

        float boundsLeft = Mathf.Min(bounds.x + adjustedBuffer.x, bounds.xMax - adjustedBuffer.x);
        float boundsRight = Mathf.Max(bounds.x + adjustedBuffer.x, bounds.xMax - adjustedBuffer.x);
        float boundsBottom = Mathf.Min(bounds.y + adjustedBuffer.y, bounds.yMax - adjustedBuffer.y);
        float boundsTop = Mathf.Max(bounds.y + adjustedBuffer.y, bounds.yMax - adjustedBuffer.y);
        boundsLeft = Mathf.Abs(boundsLeft - boundsRight) < 25.0f ? boundsLeft - 25.0f : boundsLeft;
        boundsRight = Mathf.Abs(boundsLeft - boundsRight) < 25.0f ? boundsRight + 25.0f : boundsRight;
        boundsBottom = Mathf.Abs(boundsTop - boundsBottom) < 25.0f ? boundsBottom - 25.0f : boundsBottom;
        boundsTop = Mathf.Abs(boundsTop - boundsBottom) < 25.0f ? boundsTop - 25.0f : boundsTop;

        bool isLanderAtLeftOuterBoundary = landerPosition.x < boundsLeft;
        bool isLanderAtRightOuterBoundary = landerPosition.x > boundsRight;
        bool isLanderAtTopOuterBoundary = landerPosition.y > boundsTop;
        bool isLanderAtBottomOuterBoundary = landerPosition.y < boundsBottom;
        if (isLanderAtLeftOuterBoundary)
        {
            newCameraPosition.x = landerPosition.x + ((boundsRight - boundsLeft) * 0.5f);
        }
        else if (isLanderAtRightOuterBoundary)
        {
            newCameraPosition.x = landerPosition.x - ((boundsRight - boundsLeft) * 0.5f);
        }

        if (isLanderAtTopOuterBoundary)
        {
            // newCameraPosition.y = boundsTop;
        }
        else if (isLanderAtBottomOuterBoundary)
        {
            // newCameraPosition.y = boundsBottom;
        }

        transform.position = newCameraPosition;
    }

    public void ResetCamera()
    {
        transform.position = initialPosition;
    }

    private Camera cam;

    private Vector3 initialPosition;

    private void DrawRect(Rect rect)
    {
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), Color.green);
        Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), Color.red);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), Color.green);
        Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), Color.red);
    }

}
