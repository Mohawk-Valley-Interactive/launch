using UnityEngine;

public class StarBehavior : MonoBehaviour
{
	public float range = 90.0f;

	void Start()
	{
		rotation = (Random.value * range) - (range * 0.5f);
	}

	void Update()
    {
		transform.Rotate(Vector3.forward, rotation);    
    }

	private float rotation;
}
