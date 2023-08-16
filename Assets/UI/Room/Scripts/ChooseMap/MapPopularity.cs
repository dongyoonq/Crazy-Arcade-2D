using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomUI.ChooseMap
{
	public class MapPopularity : MonoBehaviour
	{
		private const int MAX_CNT = 5;
		private const string UI_PATH = "ChooseMap/MapPopularityStarPoint";

		[SerializeField]
		private Transform Popularity;

		private List<MapPopularityStarPoint> starPoints;

		private void Awake()
		{
			starPoints = new List<MapPopularityStarPoint>();
			SetInitStartPoints();
		}

		public void SetInitStartPoints()
		{
			for (int i = 0; i < MAX_CNT; i++)
			{
				MapPopularityStarPoint starPoint = Instantiate(Resources.Load<MapPopularityStarPoint>(UI_PATH), Popularity);
				starPoints.Add(starPoint);
			}
		}

		public void SetPopularity(int score)
		{
			if (starPoints.Count >= MAX_CNT)
			{
				for (int i = 0; i < MAX_CNT; i++)
					starPoints[i].SetStar(i < score);
			}
		}
	}
}