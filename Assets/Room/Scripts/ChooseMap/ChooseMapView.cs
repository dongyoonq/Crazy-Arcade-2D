using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

		public UnityAction<MapData> OnClosedMapView;
		public UnityAction OnCancelMapView;


		protected override void Awake()
		{
			base.Awake();

			btnOk.onClick.AddListener(() => OnOkButtonClicked());
			btnCancel.onClick.AddListener(() => OnCancelButtonClicked());
		}

		public void SetMapInfo(MapList mapList)
		{
			foreach (var maps in mapList.Maps)
			{
				MapEntry entry = Instantiate(mapEntryPrefab, mapContent);
				entry.SetMapInfo(maps);
			}

			OnMapChoosed(mapList.Maps[0]);
		}

		protected override void OnMapChoosed(MapData data)
		{
			base.OnMapChoosed(data);

			rank.text = data.Rank.ToString();
			mapInfo.text = data.info;
		}

		public void OnOkButtonClicked()
		{
			ClosedView();
			OnClosedMapView.Invoke(curChoosedMap);
		}

		public void OnCancelButtonClicked()
		{
			ClosedView();
			OnCancelMapView?.Invoke();
		}

		private void ClosedView()
		{
			var mapContents = mapContent.GetComponentsInChildren<MapEntry>();

			for (int i = 0; i < mapContents.Length; i++)
			{
				Destroy(mapContents[i].gameObject);
			}
			gameObject.SetActive(false);
		}
	}
}