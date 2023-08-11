using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI.ChooseMap
{
	public class MapEntry : MonoBehaviour
	{
		public ChooseMapView chooseMap;

		//public MapList maplist;

		public TMP_Text title;
		public TMP_Text maxPlayer;
		public TMP_Text level;
		public Toggle favoritesCheck;

		private MapData map;

		private void Awake()
		{
			chooseMap = GetComponentInParent<ChooseMapView>();
			//maplist = GetComponentInParent<MapList>();
		}

		public void SetMapInfo(MapData mapInfo)
		{
			map = mapInfo;
			title.text = mapInfo.Title;
			maxPlayer.text = mapInfo.MaxPlayer.ToString();
			level.text = mapInfo.Level.ToString();
		}

		public void OnChooseMapClicked()
		{
			chooseMap.OnChoosedMap?.Invoke(this.map);
		}
	}
}