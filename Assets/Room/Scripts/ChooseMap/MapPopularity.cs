using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomUI.ChooseMap
{
	public class MapPopularity : MonoBehaviour
	{
		private const int MAX_CNT = 5;
		private const string UI_PATH = "ChooseMap/StarPoint";

		private List<MapPopularityStarPoint> starPoints;

		private void Awake()
		{
			starPoints = new List<MapPopularityStarPoint>();

			for (int i = 0; i < MAX_CNT; i++)
			{
				MapPopularityStarPoint starPoint = Instantiate<MapPopularityStarPoint>(Resources.Load<MapPopularityStarPoint>("UI_PATH"), transform);
				starPoints.Add(starPoint);
			}
		}

		public void SetPopularity(int score)
		{
			for (int i = 0; i < MAX_CNT; i++)
				starPoints[i].SetStar(i < score);
		}
	}
}