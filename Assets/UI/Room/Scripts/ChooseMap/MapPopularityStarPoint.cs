using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI.ChooseMap
{ 
	public class MapPopularityStarPoint : MonoBehaviour
	{
		[SerializeField]
		private Image StarImg;
	
		[SerializeField]
		private Sprite StarON;
	
		[SerializeField]
		private Sprite StarOFF;
	
		public void SetStar(bool isOn)
		{
			StarImg.sprite = isOn ? StarON : StarOFF;
		}
	} 
}