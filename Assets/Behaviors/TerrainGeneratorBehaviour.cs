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

    public float xOffset = 0.0f;
    private float xOffsetActual = 0.0f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();

        lineThicknessActual = lineThickness;
        lineRenderer.startWidth = lineThicknessActual;
        lineRenderer.endWidth = lineThicknessActual;
        lineRenderer.positionCount = pointCount;

        linePositionList = new Vector3[pointCount];
        colliderPositionList = new Vector2[pointCount];

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

        if(buffer != bufferActual || scale != scaleActual || perlin != perlinActual || xOffset != xOffsetActual)
		{
            xOffsetActual = xOffset;
            bufferActual = buffer;
            perlinActual = perlin;
            scaleActual = scale;
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
        for(int i = 0; i < pointCount; i++)
		{
            float x = leftScreenEdge + (i * (screenWidth / (float)pointCount));
            float y1 = y0 + Mathf.PerlinNoise(perlinActual, xOffset + (i * bufferActual)) * scaleActual;

            linePositionList[i].x = x;
            linePositionList[i].y = y1;
            linePositionList[i].z = z;
            colliderPositionList[i].x = x;
            colliderPositionList[i].y = y1;
		}

        lineRenderer.SetPositions(linePositionList);
		edgeCollider2D.points = colliderPositionList;
	}

    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider2D;
    private float lineThicknessActual;

    private int pointCount = 512;
    private Vector3[] linePositionList;
    private Vector2[] colliderPositionList;
}
