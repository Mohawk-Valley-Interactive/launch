using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class ProceeduralTerrainGeneratorBehaviour : MonoBehaviour
{
	[Range(1.0f, 1000.0f)]
	public float terrainLength = 100.0f; // Length in world space
	public float terrainDensity = 1.0f; // Terrain points per unit in world space
	public float terrainMaxHeight = 1.0f; // Maximum height of terrain in world space
	private float terrainLengthActual = 100.0f; // Length in world space
	private float terrainDensityActual = 1.0f; // Terrain points per unit in world space
	private float terrainMaxHeightActual = 1.0f; // Maximum height of terrain in world space
	[Range(-50.0f, 50.0f)]
	public float floorHeightAdjustment = 0.0f;
	private float floorHeightAdjustmentActual = 0.0f;

	public int smoothA = 1;
	private int smoothAActual = 1;
	public int smoothB = 1;
	private int smoothBActual = 1;
	public int smoothC = 1;
	private int smoothCActual = 1;

	public float lineThickness = 0.01f;

	void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		edgeCollider2D = GetComponent<EdgeCollider2D>();

		lineRenderer.startWidth = lineThickness;
		lineRenderer.endWidth = lineThickness;

		int pointCount = Mathf.FloorToInt(terrainLength * terrainDensity);
		linePositionList = new Vector3[pointCount];
		colliderPositionList = new Vector2[pointCount];
		heightMap = new float[pointCount];

		GenerateTerrain();
	}

	// Update is called once per frame
	void Update()
	{
		if (
			terrainLength != terrainLengthActual || terrainDensity != terrainDensityActual || terrainMaxHeight != terrainMaxHeightActual ||
			smoothA != smoothAActual || smoothB != smoothBActual|| smoothC != smoothCActual || floorHeightAdjustmentActual != floorHeightAdjustment
			)
		{

			terrainLengthActual = terrainLength;
			terrainDensityActual = terrainDensity;
			terrainMaxHeightActual = terrainMaxHeight;
			floorHeightAdjustmentActual = floorHeightAdjustment;

			smoothAActual = smoothA;
			smoothBActual = smoothB;
			smoothCActual = smoothC;

			GenerateTerrain();
		}
	}

	protected void GenerateTerrain()
	{
		Random.InitState(0);
		const float z = 0.0f;

		float screenRatio = (float)Screen.width / (float)Screen.height;
		float screenHeight = Camera.main.orthographicSize * 2;
		float screenWidth = screenHeight * screenRatio;

		float terrainFloorHeight = (screenHeight * -0.5f) + floorHeightAdjustment; // Lowest height of terrain in world space

		int pointCount = heightMap.Length;
		for (int i = 0; i < pointCount; ++i)
		{
			heightMap[i] = Random.value;
		}
		SmoothHeightMap(smoothA);
		SmoothHeightMap(smoothB);
		SmoothHeightMap(smoothC);

		NormalizeMap();

		float horizontalPosition = terrainLength * -0.5f;
		float horizontalIncrement = 1.0f / terrainDensity;
		for (int i = 0; i < pointCount; ++i)
		{
			colliderPositionList[i].x = linePositionList[i].x = horizontalPosition;
			colliderPositionList[i].y = linePositionList[i].y = (heightMap[i] * terrainMaxHeight) + terrainFloorHeight;
			linePositionList[i].z = z;

			horizontalPosition += horizontalIncrement;
		}

		lineRenderer.positionCount = pointCount;
		lineRenderer.SetPositions(linePositionList);
		edgeCollider2D.points = colliderPositionList;

		return;
	}

	protected void NormalizeMap()
	{
		float min = Mathf.Min(heightMap);
		float max = Mathf.Max(heightMap);
		float adjustment = (1.0f / (max - min));
		for(int i = 0; i < heightMap.Length; i++)
		{
			heightMap[i] = (heightMap[i] - min) * adjustment;
		}
	}

	protected void SmoothHeightMap(int SCAN_RADIUS)
	{
		for (int i = 0; i < heightMap.Length; i++)
		{
			float heightSum = 0;
			int heightCount = 0;

			for (int n = i - SCAN_RADIUS;
					 n < i + SCAN_RADIUS + 1;
					 n++)
			{
				if (n >= 0 &&
					n < heightMap.Length)
				{
					heightSum += heightMap[n];
					heightCount++;
				}
			}

			float heightAverage = heightSum / heightCount;
			heightMap[i] = heightAverage;
		}
	}

	private LineRenderer lineRenderer;
	private EdgeCollider2D edgeCollider2D;

	private Vector3[] linePositionList;
	private Vector2[] colliderPositionList;
	float[] heightMap;
}
