using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingZoneGeneratorBehavior : MonoBehaviour
{
	public class LandingZone
	{
		public LandingZone(int startIndex, int multiplier)
		{
			this.startIndex = startIndex;
			this.multiplier = multiplier;
		}

		public LandingZone(int startIndex, int multiplier, int id)
		{
			this.startIndex = startIndex;
			this.multiplier = multiplier;
			this.id = id;
		}

		public Vector2 EndPoint
		{
			get => endPoint;
			set => endPoint = value;
		}
		public int Id { get => id; }
		public int Multiplier { get => multiplier; }
		public int StartIndex { get => startIndex; }
		public Vector2 StartPoint
		{
			get => startPoint;
			set => startPoint = value;
		}

		private Vector2 endPoint;
		private int id = -1;
		private int multiplier;
		private int startIndex;
		private Vector2 startPoint;
	}

	public float colliderHeight = 10.0f;

	public GameObject canvas;
	public Font font;
	public Material lzMaterial;
	public int fontSize;
	public float yOffset;
	public float lzLineThickness;

	public void GenerateLandingZones(List<LandingZone> landingZones, int levelId = 0)
	{
		for (int landingZoneIndex = 0; landingZoneIndex < landingZones.Count; ++landingZoneIndex)
		{
			LandingZone landingZone = landingZones[landingZoneIndex];
			for (int configIndex = 0; configIndex < landingZoneConfigs.GetLength(1); ++configIndex)
			{
				if(landingZone.Id != landingZoneConfigs[levelId, configIndex])
				{
					continue;
				}

				GameObject lz = new GameObject("LZ" + landingZone.StartIndex.ToString());
				landingZoneColliders.Add(lz);
				lz.transform.position = new Vector3(
					landingZone.StartPoint.x + ((landingZone.EndPoint.x - landingZone.StartPoint.x) * 0.5f),
					landingZone.StartPoint.y + ((landingZone.EndPoint.y - landingZone.StartPoint.y) + (colliderHeight * 0.5f)),
					-1.0f
				);

				BoxCollider2D collider = lz.AddComponent<BoxCollider2D>();
				collider.isTrigger = true;
				collider.size = new Vector2(
					(landingZone.EndPoint.x - landingZone.StartPoint.x),
					colliderHeight
				);

				LandingZoneBehavior lzb = lz.AddComponent<LandingZoneBehavior>();
				lzb.Multiplier = landingZone.Multiplier;

				LineRenderer lr = lz.AddComponent<LineRenderer>();
				lr.SetPositions(new Vector3[2] { landingZone.StartPoint, landingZone.EndPoint });
				lr.material = lzMaterial;
				lr.startWidth = lzLineThickness;
				lr.endWidth = lzLineThickness;

				// Multiplier text 
				GameObject go = new GameObject(lz.name + "_text");
				go.transform.position = new Vector3(lz.transform.position.x, lz.transform.position.y + yOffset, lz.transform.position.z);
				go.transform.SetParent(canvas.transform);
				Text text = go.AddComponent<Text>();
				text.text = landingZone.Multiplier.ToString() + "x";
				text.font = font;
				text.fontSize = fontSize;
				text.alignment = TextAnchor.MiddleCenter;
				text.fontStyle = FontStyle.Bold;
			}
		}
	}

	private List<GameObject> landingZoneColliders = new List<GameObject>(4);
	private int[,] landingZoneConfigs = new int[7, 4] {
		{2, 3, 7, 9},
		{7, 8, 9, 10},
		{2, 3, 7, 9},
		{1, 4, 7, 9},
		{0, 5, 7, 9},
		{6, 7, 8, 9},
		{1, 4, 7, 9}
	};
}
