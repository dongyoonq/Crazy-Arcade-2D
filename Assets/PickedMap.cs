using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoomUI.ChooseMap
{
	public class PickedMap : MonoBehaviourPun
	{
		private const string UI_PATH = "ChooseMap/ChooseMap";

		private ChooseMap chooseMapUI;

		private Map map;

		[SerializeField]
		private Button btnPickedMap;

		[SerializeField]
		private MapList mapList;

		[SerializeField]
		private TMP_Text txtMapName;

		private void Awake()
		{
			if(PhotonNetwork.IsMasterClient)
				btnPickedMap.onClick.AddListener(() => OpenChooseMapUI());
		}

		private void OpenChooseMapUI()
		{
			Debug.Log("[OpenChooseMapUI] 클릭");

			if(chooseMapUI == null)
			{
				chooseMapUI = GameManager.UI.ShowPopUpUI<ChooseMap>(UI_PATH);
				chooseMapUI.SetMapInfo(mapList);
				chooseMapUI.OnClosedMapView += ClosedMapView;
			}
		}

		private void ClosedMapView(Map map)
		{
			txtMapName.text = map.title;
			//todo.선택된 이미지로 세팅

			chooseMapUI = null;
		}
	}

}
