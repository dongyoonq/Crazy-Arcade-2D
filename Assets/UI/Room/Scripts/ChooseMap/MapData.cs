using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI.ChooseMap
{
	[CreateAssetMenu(fileName = "MapData", menuName = "Data/Map")]
	public class MapData : ScriptableObject
	{
		public int Id;

		public string Group;
		public string Title;
		public int MaxPlayer;
		public int Level;
		public int Popularity;
		public int Rank;
		public string info;

		public bool Favorites;

		public Sprite MapImg;
		public Sprite MapIcon;
		public string MapFileName;
	}
}