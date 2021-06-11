using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeRollerBehavior : MonoBehaviour
{
    public GameObject leftLandmass;
    public GameObject rightLandmass;
    public float cameraBoundsBuffer;

    void Start()
    {
        bufferBoundDistance = (Camera.main.orthographicSize * 2.0f) + cameraBoundsBuffer;
    }

    void Update()
    {
        float leftBoundary = Camera.main.transform.position.x - bufferBoundDistance;
        float rightBoundary = Camera.main.transform.position.x + bufferBoundDistance;
        float leftLandmassPosition = leftLandmass.transform.position.x;
        float rightLandmassPosition = rightLandmass.transform.position.x;
        MeshFilter leftMesh = leftLandmass.GetComponent<MeshFilter>();
        leftSize = leftMesh.mesh.bounds.size.x * leftLandmass.transform.localScale.x;
        MeshFilter rightMesh = rightLandmass.GetComponent<MeshFilter>();
        rightSize = rightMesh.mesh.bounds.size.x * rightLandmass.transform.localScale.x;

        if(leftBoundary < leftLandmassPosition) {
            Debug.Log("0 " + leftBoundary + " < " + leftLandmassPosition + " & " + rightSize);

            Vector3 newPosition = leftLandmass.transform.position;
            newPosition.x = newPosition.x - rightSize;
            rightLandmass.transform.position = newPosition;

            GameObject tempLandmass = rightLandmass;
            rightLandmass = leftLandmass;
            leftLandmass = tempLandmass;
        } else if (rightLandmassPosition + rightSize < rightBoundary) {
            Debug.Log("1 " + (rightLandmassPosition + rightSize) + " < " + rightBoundary + " & " + rightSize);
            Vector3 newPosition = rightLandmass.transform.position;
            newPosition.x = newPosition.x + rightSize;
            leftLandmass.transform.position = newPosition;

            GameObject tempLandmass = leftLandmass;
            leftLandmass = rightLandmass;
            rightLandmass = tempLandmass;
        }
    }

    private float bufferBoundDistance = 0.0f;
    private float leftSize = 0.0f;
    private float rightSize = 0.0f;
}