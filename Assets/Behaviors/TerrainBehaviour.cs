using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class TerrainBehaviour : MonoBehaviour
{
	public float terrainLineThickness = 1.0f;
	public float terrainLineNeutralHeight = 0.5f;
	public int subdivisionCount = 1;

	private float[] heightMap;
	private Mesh mesh;
	private float terrainLineThicknessActual = 1.0f;
	private float terrainLineNeutralHeightActual = 0.5f;
	private int subdivisionCountActual = 1;

	void Start()
	{
		subdivisionCount = (int)Mathf.Clamp(subdivisionCount, 0.0f, 16.0f);
		terrainLineThicknessActual = terrainLineThickness;
		terrainLineNeutralHeightActual = terrainLineNeutralHeight;
		subdivisionCountActual = subdivisionCount;

		GenerateMesh();
	}

	void Update()
	{
		if (terrainLineThicknessActual != terrainLineThickness || terrainLineNeutralHeightActual != terrainLineNeutralHeight || subdivisionCountActual != subdivisionCount)
		{

			subdivisionCount = (int)Mathf.Clamp(subdivisionCount, 0.0f, 16.0f);
			terrainLineThicknessActual = terrainLineThickness;
			terrainLineNeutralHeightActual = terrainLineNeutralHeight;
			subdivisionCountActual = subdivisionCount;

			Debug.Log("Generating Mesh!" + subdivisionCount);

			GenerateMesh();
		}
	}


	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("!!!");
	}

	private void GenerateMesh()
	{
		int segmentCount = (int)Mathf.Pow(2.0f, Mathf.Max(0.0f, (float)(subdivisionCount)));
		int heightMapSize = segmentCount + 1;
		heightMap = new float[heightMapSize];
		for (int i = 0; i < heightMapSize; i++)
		{
			heightMap[i] = (float)i / (float)heightMapSize;
			//heightMap[i] = 0.5f + (Mathf.Sin(i * 0.001f) * 0.1f);
			//heightMap[i] = 0.5f + (Mathf.Cos(i * 0.001f) * 0.1f);
			//heightMap[i] = 0.5f + (Mathf.Sin(i * 0.001f) * 0.1f) + (Mathf.Cos(i * 0.01f) * 0.2f);
		}

		float screenRatio = ((float)Screen.width / (float)Screen.height);
		float screenHeight = Camera.main.orthographicSize * 2;
		float screenWidth = screenHeight * screenRatio;

		float lineThickness = (screenHeight * terrainLineThickness) * 0.5f;
		float lineNeutralTop = (screenHeight * -0.5f) + (screenHeight * terrainLineNeutralHeight) + lineThickness;
		float lineNeutralBottom = (screenHeight * -0.5f) + (screenHeight * terrainLineNeutralHeight) - lineThickness;

		float leftScreenEdge = screenWidth * -0.5f;
		float rightScreenEdge = screenWidth;
		float near = -0.5f;
		float far = 0.5f;

		float incrementLength = rightScreenEdge;
		Vector3 incrementVector = new Vector3(incrementLength, 0.0f, 0.0f);

		Vector3 leftBottomNear = new Vector3(leftScreenEdge, lineNeutralBottom + (screenHeight * heightMap[0]), near);
		Vector3 leftTopNear = new Vector3(leftScreenEdge, lineNeutralTop + (screenHeight * heightMap[0]), near);
		Vector3 leftTopFar = new Vector3(leftScreenEdge, lineNeutralTop + (screenHeight * heightMap[0]), far);
		Vector3 leftBottomFar = new Vector3(leftScreenEdge, lineNeutralBottom + (screenHeight * heightMap[0]), far);
		Vector3 rightBottomNear = leftBottomNear + (incrementVector / segmentCount);
		Vector3 rightTopNear = leftTopNear + (incrementVector / segmentCount);
		Vector3 rightBottomFar = leftBottomFar + (incrementVector / segmentCount);
		Vector3 rightTopFar = leftTopFar + (incrementVector / segmentCount);
		rightBottomNear.y = lineNeutralBottom + (screenHeight * heightMap[1]);
		rightTopNear.y = lineNeutralBottom + (screenHeight * heightMap[1]);
		rightBottomFar.y = lineNeutralBottom + (screenHeight * heightMap[1]);
		rightTopFar.y = lineNeutralBottom + (screenHeight * heightMap[1]);
		int leftBottomNearIndex = 0;
		int leftTopNearIndex = 1;
		int leftTopFarIndex = 2;
		int leftBottomFarIndex = 3;
		int rightBottomNearIndex = leftBottomNearIndex + 4;
		int rightTopNearIndex = leftTopNearIndex + 4;
		int rightTopFarIndex = leftTopFarIndex + 4;
		int rightBottomFarIndex = leftBottomFarIndex + 4;

		mesh = new Mesh();
		List<Vector3> vertexList = new List<Vector3>(8);
		List<int> indexList = new List<int>(12);

		// Left-side cap
		vertexList.Add(leftBottomNear);
		vertexList.Add(leftTopNear);
		vertexList.Add(leftTopFar);
		vertexList.Add(leftBottomFar);
		indexList.Add(leftBottomNearIndex);
		indexList.Add(leftBottomFarIndex);
		indexList.Add(leftTopNearIndex);
		indexList.Add(leftBottomFarIndex);
		indexList.Add(leftTopFarIndex);
		indexList.Add(leftTopNearIndex);

		for (int i = 1; i < segmentCount + 1; i++)
		{

			vertexList.Add(rightBottomNear);
			vertexList.Add(rightTopNear);
			vertexList.Add(rightTopFar);
			vertexList.Add(rightBottomFar);

			// Top
			indexList.Add(leftTopNearIndex);
			indexList.Add(leftTopFarIndex);
			indexList.Add(rightTopNearIndex);
			indexList.Add(leftTopFarIndex);
			indexList.Add(rightTopFarIndex);
			indexList.Add(rightTopNearIndex);

			// Bottom
			indexList.Add(leftBottomFarIndex);
			indexList.Add(leftBottomNearIndex);
			indexList.Add(rightBottomNearIndex);
			indexList.Add(leftBottomFarIndex);
			indexList.Add(rightBottomNearIndex);
			indexList.Add(rightBottomFarIndex);

			// Near
			indexList.Add(leftBottomNearIndex);
			indexList.Add(leftTopNearIndex);
			indexList.Add(rightBottomNearIndex);
			indexList.Add(leftTopNearIndex);
			indexList.Add(rightTopNearIndex);
			indexList.Add(rightBottomNearIndex);

			// Far
			indexList.Add(leftTopFarIndex);
			indexList.Add(leftBottomFarIndex);
			indexList.Add(rightBottomFarIndex);
			indexList.Add(leftTopFarIndex);
			indexList.Add(rightBottomFarIndex);
			indexList.Add(rightTopFarIndex);

			leftBottomNear = rightBottomNear;
			leftTopNear = rightTopNear;
			leftBottomFar = rightBottomFar;
			leftTopFar = rightTopFar;

			leftBottomNearIndex = rightBottomNearIndex;
			leftTopNearIndex = rightTopNearIndex;
			leftTopFarIndex = rightTopFarIndex;
			leftBottomFarIndex = rightBottomFarIndex;

			rightBottomNear = leftBottomNear + (incrementVector / segmentCount);
			rightTopNear = leftTopNear + (incrementVector / segmentCount);
			rightBottomFar = leftBottomFar + (incrementVector / segmentCount);
			rightTopFar = leftTopFar + (incrementVector / segmentCount);

			rightBottomNear.y = lineNeutralBottom + (screenHeight * heightMap[i]);
			rightTopNear.y = lineNeutralTop + (screenHeight * heightMap[i]);
			rightBottomFar.y = lineNeutralBottom + (screenHeight * heightMap[i]);
			rightTopFar.y = lineNeutralTop + (screenHeight * heightMap[i]);

			rightBottomNearIndex = leftBottomNearIndex + 4;
			rightTopNearIndex = leftTopNearIndex + 4;
			rightTopFarIndex = leftTopFarIndex + 4;
			rightBottomFarIndex = leftBottomFarIndex + 4;
		}

		// Right-side cap
		indexList.Add(leftBottomNearIndex);
		indexList.Add(leftBottomFarIndex);
		indexList.Add(leftTopNearIndex);
		indexList.Add(leftBottomFarIndex);
		indexList.Add(leftTopFarIndex);
		indexList.Add(leftTopNearIndex);

		mesh.vertices = vertexList.ToArray();
		mesh.triangles = indexList.ToArray();
		mesh.RecalculateNormals();

		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}
}
