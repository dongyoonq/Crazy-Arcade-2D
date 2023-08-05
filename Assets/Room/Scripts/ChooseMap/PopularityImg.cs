using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomUI.ChooseMap
{
	public class PopularityImg : MonoBehaviour
	{
		private const int MAX_CNT = 5;
		private const string UI_PATH = "ChooseMap/StarPoint";

		[SerializeField]
		private List<StarPoint> starPoints;
	
		private void Awake()
		{
			starPoints = new List<StarPoint>();

			for(int i=0; i < MAX_CNT; i++)
			{
				StarPoint star = Instantiate<StarPoint>(Resources.Load<StarPoint>("UI_PATH"), transform);
				starPoints.Add(star);
			}
		}

		public void SetPopularity(int score)
		{
			for (int i = 0; i < MAX_CNT; i++)
				starPoints[i].SetStar(i < score);
		}
	}
}

