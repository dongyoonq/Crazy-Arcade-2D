using CustomProperty;
using Photon.Pun;
using RoomUI.ChooseMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LobbyUI.QuickStart
{
	public class QuickChooseMap : ChooseMap
	{
		//[SerializeField]
		//private MapDropDown mapContent;

		[SerializeField] 
		private RectTransform mapContent;

		[SerializeField] 
		private QuickMapEntry mapEntryPrefab;

		[SerializeField]
		private TMP_Text rank;

		[SerializeField]
		private TMP_Text mapInfo;

		[SerializeField]
		private Button btnOk;

		[SerializeField]
		private Button btnCancel;

		[SerializeField]
		private MapList mapList;

		private int choosedMapId;

		public UnityAction<int, MapData> OnClosedMapView;

		protected override void Awake()
		{
			base.Awake();
			btnOk.onClick.AddListener(() => OnOkButtonClicked());

			int index = 0;
			foreach (var maps in mapList.Maps)
			{
				QuickMapEntry entry = Instantiate(mapEntryPrefab, mapContent);
				entry.SetMapEntry(maps);
				maps.Id = index++;
			}
		}

		public MapData GetInitMapData()
		{
			return mapList.Maps[0];
		}

		public void SetMapInfo(int mapId)
		{
			choosedMapId = mapId;
			OnMapChoosed(mapList.Maps.Where(x => x.Id == mapId).FirstOrDefault());
		}

		protected override void OnMapChoosed(MapData data)
		{
			base.OnMapChoosed(data);

			rank.text = data.Rank.ToString();
			mapInfo.text = data.info;

			choosedMapId = data.Id;
		}

		public void OnOkButtonClicked()
		{
			gameObject.SetActive(false);
			OnClosedMapView?.Invoke(choosedMapId, curChoosedMap);
		}
	}
}
