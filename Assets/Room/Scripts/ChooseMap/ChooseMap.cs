using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoomUI.ChooseMap
{
	public class ChooseMap : PopUpUI
	{
		private MapList mapList;

		[SerializeField] RectTransform mapContent;

		[SerializeField] MapEntry mapEntryPrefab;

		[SerializeField] TMP_Text mapTitle;
		[SerializeField] TMP_Text maxPlayer;
		[SerializeField] TMP_Text rank;
		[SerializeField] TMP_Text mapInfo;

		[SerializeField] Image mapImg;
		[SerializeField] Image levelImg;

		[SerializeField]
		private MapPopularity popularity;

		[SerializeField]
		private Button btnOk;

		[SerializeField]
		private Button btnCancel;

		public MapData prevMap;
		public MapData curChoosedMap;
		public MapData curMap;

		public UnityAction<MapData> OnClosedMapView;


		private void Awake()
		{
			btnOk.onClick.AddListener(() => OnOkButtonClicked());
			btnCancel.onClick.AddListener(() => OnCancelButtonClicked());
		}

		private void OnEnable()
		{
			prevMap = curMap;
			Debug.Log("curMap = {0}", curMap);
		}

		public void SetMapInfo(MapList mapList)
		{
			foreach (var maps in mapList.Maps)
			{
				MapEntry entry = Instantiate(mapEntryPrefab, mapContent);
				entry.SetMapInfo(maps);
			}

			curChoosedMap = mapList.Maps[0];
			OnMapChoosed();
		}

		public void OnMapChoosed()
		{
			mapTitle.text = curChoosedMap.Title;
			maxPlayer.text = curChoosedMap.MaxPlayer.ToString();
			rank.text = curChoosedMap.Rank.ToString();
			mapInfo.text = curChoosedMap.info;
			mapImg.sprite = curChoosedMap.MapImg;

			popularity.SetPopularity(curChoosedMap.Popularity);
		}

		public void OnOkButtonClicked()
		{
			//Debug.Log($"[OnOkButtonClicked] : {curChoosedMap.Title}");

			curMap = curChoosedMap;
			OnClosedMapView.Invoke(curMap);

			GameManager.UI.ClosePopUpUI();
		}


		public void OnCancelButtonClicked()
		{
			curMap = prevMap;
			//OnClosedMapView.Invoke(curMap);

			GameManager.UI.ClosePopUpUI();
		}
	}
}