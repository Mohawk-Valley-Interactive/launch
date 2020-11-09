using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LanderCameraBehavior : MonoBehaviour
{
    public LanderBehaviour lander;

	private void Start()
	{
        Camera camera = GetComponent<Camera>();
        Vector3 position = this.transform.position;
        position.y = camera.orthographicSize;
        this.transform.position = position;
	}

	void Update()
    {
        if (lander.HasCrashed)
            return;

        Vector3 position = this.transform.position;
        position.x = lander.transform.position.x;
        this.transform.position = position;
    }
}
