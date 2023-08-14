using CustomProperty;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace RoomUI.ChooseMap
{
	public class ChooseMapView : ChooseMap
	{
		[SerializeField] RectTransform mapContent;

		[SerializeField] MapEntry mapEntryPrefab;

		[SerializeField]
		private TMP_Text rank;

		[SerializeField]
		private TMP_Text mapInfo;

		[SerializeField]
		private Button btnOk;

		[SerializeField]
		private Button btnCancel;

		public UnityAction<int> OnClosedMapView;


		protected override void Awake()
		{
			base.Awake();

			btnOk.onClick.AddListener(() => OnOkButtonClicked());
			btnCancel.onClick.AddListener(() => OnCancelButtonClicked());
		}

		public void SetMapInfo(MapList mapList, int choosedMapId)
		{
			foreach (var maps in mapList.Maps)
			{
				MapEntry entry = Instantiate(mapEntryPrefab, mapContent);
				entry.SetMapInfo(maps);
			}

			OnMapChoosed(mapList.Maps.Where(x => x.Id == choosedMapId).FirstOrDefault()) ;
		}

		protected override void OnMapChoosed(MapData data)
		{
			base.OnMapChoosed(data);

			rank.text = data.Rank.ToString();
			mapInfo.text = data.info;
		}

		public void OnOkButtonClicked()
		{
			PhotonHashtable property = new PhotonHashtable();
			property[RoomProp.ROOM_MAP_ID] = curChoosedMap.Id;
			PhotonNetwork.CurrentRoom.SetCustomProperties(property);

			ClosedView();	
		}

		public void OnCancelButtonClicked()
		{
			ClosedView();
		}

		private void ClosedView()
		{
			var mapContents = mapContent.GetComponentsInChildren<MapEntry>();

			for (int i = 0; i < mapContents.Length; i++)
			{
				Destroy(mapContents[i].gameObject);
			}
			gameObject.SetActive(false);

			OnClosedMapView?.Invoke(curChoosedMap.Id);
		}
	}
}