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
		private Map map;

		[SerializeField]
		private Button btnPickedMap;

		[SerializeField]
		private MapList mapList;

		[SerializeField]
		private PickedGameMap gameMap;

		[SerializeField]
		private ChooseMapView chooseMapUI;

		private bool isActiveUI;

		private void Awake()
		{
			if (PhotonNetwork.IsMasterClient)
				btnPickedMap.onClick.AddListener(() => OpenChooseMapUI());

			isActiveUI = false;

			for(int i=0; i<mapList.Maps.Count; i++)
				mapList.Maps[i].Id = i;
		}

		private void Start()
		{
			if (mapList != null && mapList.Maps.Count > 0)
				gameMap.InitGameMap(mapList.Maps[0]);
		}

		private void OpenChooseMapUI()
		{
			if (isActiveUI == false)
			{
				chooseMapUI.gameObject.SetActive(true);
				chooseMapUI.SetMapInfo(mapList);
				chooseMapUI.OnClosedMapView += ClosedMapView;
				chooseMapUI.OnCancelMapView += CancelMapView;
				isActiveUI = true;
			}
		}

		private void ClosedMapView(MapData map)
		{
			gameMap.OnChoosedMap?.Invoke(map);
			DisableMapView();
		}

		private void CancelMapView()
		{
			DisableMapView();
		}

		private void DisableMapView()
		{
			chooseMapUI.OnClosedMapView -= ClosedMapView;
			chooseMapUI.OnCancelMapView -= CancelMapView;

			isActiveUI = false;
		}
	}

}
