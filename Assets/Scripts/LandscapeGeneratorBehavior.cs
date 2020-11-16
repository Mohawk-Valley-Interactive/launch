using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using static LandingZoneGeneratorBehavior;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(LandingZoneGeneratorBehavior))]
[RequireComponent(typeof(LineRenderer))]
public class LandscapeGeneratorBehavior : MonoBehaviour
{
	public TextAsset terrainFile;
	public int rangeTotal = 1;
	public float terrainLineThickness;
	public float bottomBufferSpace = 100.0f;
	public float minimumXDistance = 25.0f;
	public StarFieldBehavior starFieldBehavior;

	public List<LandingZone> LandingZones
	{
		get => landingZones;
	}

	// Start is called before the first frame update
	void Start()
	{
		edgeCollider = GetComponent<EdgeCollider2D>();
		lineRenderer = GetComponent<LineRenderer>();
		LandingZoneGeneratorBehavior landscapeBehavior = GetComponent<LandingZoneGeneratorBehavior>();

		lineRenderer.startWidth = terrainLineThickness;
		lineRenderer.endWidth = terrainLineThickness;

		InitData();

		landscapeBehavior.GenerateLandingZones(landingZones);
		starFieldBehavior.GenerateStars(lineRenderer);
	}

	// Update is called once per frame
	private LineRenderer lineRenderer;
	private EdgeCollider2D edgeCollider;
	private List<LandingZone> referenceLandingZones = new List<LandingZone>();
	private List<LandingZone> landingZones = new List<LandingZone>();

	private void InitData()
	{
		string[] lines = terrainFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
		List<Vector3> points = new List<Vector3>(lines.Length);
		List<Vector3> finalPoints = new List<Vector3>((lines.Length * rangeTotal * 2) + 1);

		float previousX = 0.0f;
		float xAdjustment = 0.0f;
		bool isLandingZone = false;
		for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
		{
			string[] lineData = lines[lineIndex].Split(' ');
			float x = float.Parse(lineData[0]) + xAdjustment;
			float y = float.Parse(lineData[1]);
			Vector3 newPoint = new Vector3(x, y, 0.0f);
			if (lineData.Length > 2)
			{
				referenceLandingZones.Add(new LandingZone(lineIndex, int.Parse(lineData[2])));
				isLandingZone = true;
			}
			else if (isLandingZone)
			{
				isLandingZone = false;

				float providedDistance = newPoint.x - previousX;
				if (providedDistance < minimumXDistance)
				{
					newPoint.x = previousX + minimumXDistance;
					xAdjustment += minimumXDistance - providedDistance;
				}
			}

			previousX = newPoint.x;
			points.Add(newPoint);
		}

		List<float> xs = new List<float>(points.Count);
		points.ForEach(p => xs.Add(p.x));
		float rangeX = Mathf.Max(xs.ToArray());

		int finalPointIndex = 0;
		for (int rangeIndex = -rangeTotal; rangeIndex <= rangeTotal; rangeIndex++)
		{
			for (int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
			{
				Vector3 newVec3 = new Vector3((points[pointIndex].x) + (rangeIndex * rangeX), points[pointIndex].y + bottomBufferSpace, 0.0f);
				finalPoints.Add(newVec3);

				int referenceIndex = finalPointIndex > points.Count ? finalPointIndex % points.Count : finalPointIndex;
				for (int zoneIndex = 0; zoneIndex < referenceLandingZones.Count; ++zoneIndex)
				{
					if (referenceIndex == referenceLandingZones[zoneIndex].StartIndex)
					{
						landingZones.Add(new LandingZone(finalPointIndex, referenceLandingZones[zoneIndex].Multiplier, zoneIndex));
					}
				}

				++finalPointIndex;
			}
		}

		foreach (LandingZone landingZone in landingZones)
		{
			landingZone.StartPoint = finalPoints[landingZone.StartIndex];
			landingZone.EndPoint = finalPoints[landingZone.StartIndex + 1];
		}

		lineRenderer.positionCount = finalPoints.Count;
		lineRenderer.SetPositions(finalPoints.ToArray());

		List<Vector2> points2d = new List<Vector2>(finalPoints.Count);
		finalPoints.ForEach(p => points2d.Add(p));
		edgeCollider.points = points2d.ToArray();
	}

}
