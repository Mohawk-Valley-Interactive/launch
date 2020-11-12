using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFieldBehavior : MonoBehaviour
{
	public GameObject[] starPrefabs = new GameObject[2];
	// Start is called before the first frame update
	public void GenerateStars(LineRenderer landscapeRenderer)
	{
		Vector3[] positions = new Vector3[landscapeRenderer.positionCount];
		landscapeRenderer.GetPositions(positions);
		for (int i = 0; i < positions.Length; i++)
		{
			if (Random.value < 0.1f)
			{
				Vector3 line = positions[i];
				float starY = Random.value * 720.0f;
				if (starY > line.y)
				{
					GameObject star;
					if (Random.value > 0.1f)
					{
						star = Instantiate(starPrefabs[0], transform);
					}
					else
					{
						star = Instantiate(starPrefabs[1], transform);
					}
					star.transform.position = new Vector3(line.x, starY, line.z);
				}
			}
		}
	}
}
