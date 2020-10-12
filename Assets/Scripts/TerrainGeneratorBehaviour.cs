using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class TerrainGeneratorBehaviour : MonoBehaviour
{
	public Vector2 offset;

	public float lineThickness = 0.01f;
	public float heightScaleA = 0.0f;
	public float heightScaleB = 0.0f;
	public float primaryBufferScale = 0.0f;
	public float secondaryBufferScale = 0.0f;
	public float perlinOffset = 0.0f;

	void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		edgeCollider2D = GetComponent<EdgeCollider2D>();

		lineRenderer.startWidth = lineThickness;
		lineRenderer.endWidth = lineThickness;
		lineRenderer.positionCount = pointCount;

		linePositionList = new Vector3[pointCount];
		colliderPositionList = new Vector2[pointCount];

		GenerateTerrain();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (xOffset != offset.x)
		{
			xOffset = offset.x;
			GenerateTerrain();
		}
	}

	protected void GenerateTerrain()
	{
		const float z = 0.0f;

		float screenRatio = ((float)Screen.width / (float)Screen.height);
		float screenHeight = Camera.main.orthographicSize * 2;
		float screenWidth = screenHeight * screenRatio;

		float leftScreenEdge = screenWidth * -0.5f;

		float y0 = screenHeight * -0.5f;
		for (int i = 0; i < pointCount; i++)
		{
			float x = (xOffset + leftScreenEdge) + (i * (screenWidth / (float)pointCount));
			float y1 = y0 + (Mathf.PerlinNoise(perlinOffset, 5000.0f + x * primaryBufferScale) * heightScaleA);
			float y2 = y0 + (Mathf.PerlinNoise(perlinOffset, 5000.0f + x * secondaryBufferScale) * heightScaleB);

			colliderPositionList[i].x = linePositionList[i].x = x;
			colliderPositionList[i].y = linePositionList[i].y = (y1 + y2) / 2.0f;
			linePositionList[i].z = z;
		}

		lineRenderer.SetPositions(linePositionList);
		edgeCollider2D.points = colliderPositionList;
	}

	private LineRenderer lineRenderer;
	private EdgeCollider2D edgeCollider2D;

	private int pointCount = 768;
	private float xOffset = 0.0f;
	private Vector3[] linePositionList;
	private Vector2[] colliderPositionList;
}
