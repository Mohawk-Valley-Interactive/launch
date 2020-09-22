using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class TerrainGeneratorBehaviour : MonoBehaviour
{
    public float lineThickness = 0.01f;
    public float scale = 0.0f;
    private float scaleActual = 0.0f;
    public float buffer = 0.0f;
    private float bufferActual = 0.0f;
    public float perlin = 0.0f;
    private float perlinActual = 0.0f;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();

        lineThicknessActual = lineThickness;
        lineRenderer.startWidth = lineThicknessActual;
        lineRenderer.endWidth = lineThicknessActual;

        GenerateTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        lineThickness = Mathf.Max(lineThickness, 0.005f);
        if(lineThickness != lineThicknessActual)
		{
            lineThicknessActual = lineThickness;
            lineRenderer.startWidth = lineThicknessActual;
            lineRenderer.endWidth = lineThicknessActual;
		}

        if(buffer != bufferActual || scale != scaleActual || perlin != perlinActual)
		{
            bufferActual = buffer;
            perlinActual = perlin;
            scaleActual = scale;
            GenerateTerrain();
		}
    }

    protected void GenerateTerrain()
	{

		float screenRatio = ((float)Screen.width / (float)Screen.height);
		float screenHeight = Camera.main.orthographicSize * 2;
		float screenWidth = screenHeight * screenRatio;

		float leftScreenEdge = screenWidth * -0.5f;
		float rightScreenEdge = screenWidth * 0.5f;

        int pointCount = 512;
        Vector3[] linePositionList = new Vector3[pointCount];
        Vector2[] colliderPositionList = new Vector2[pointCount];
        float x = leftScreenEdge;
        float y0 = screenHeight * -0.5f;
        float z = 0.0f;
        for(int i = 0; i < pointCount; i++)
		{
            x = leftScreenEdge + (i * (screenWidth / (float)pointCount));
            float y1 = y0 + Mathf.PerlinNoise(perlinActual, i * bufferActual) * scaleActual;
            linePositionList[i] = new Vector3(x, y1, z);
            colliderPositionList[i] = new Vector2(x, y1);
		}

        lineRenderer.positionCount = pointCount;
        lineRenderer.SetPositions(linePositionList);
		edgeCollider2D.points = colliderPositionList;
	}

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider2D;
    private float lineThicknessActual;
}
