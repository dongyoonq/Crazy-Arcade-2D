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

		public MapList mapList;

		public PickedGameMap gameMap;

		[SerializeField]
		private ChooseMapView chooseMapUI;

		private bool isActiveUI;
		private int chooseMapID;

		private void Awake()
		{
			if (PhotonNetwork.IsMasterClient)
				btnPickedMap.onClick.AddListener(() => OpenChooseMapUI());

			isActiveUI = false;

			for(int i=0; i<mapList.Maps.Count; i++)
				mapList.Maps[i].Id = i;

			chooseMapID = 0;
		}

		private void Start()
		{
			if (mapList != null && mapList.Maps.Count > 0)
				gameMap.InitGameMap(mapList.Maps[0]);
		}

		private void OpenChooseMapUI()
		{
			GameManager.Sound.Onclick();

			if (isActiveUI == false)
			{
				chooseMapUI.gameObject.SetActive(true);
				chooseMapUI.SetMapInfo(mapList, chooseMapID);
				chooseMapUI.OnClosedMapView += ClosedMapView;
				isActiveUI = true;
			}
		}

		private void ClosedMapView(int choosedId)
		{
			chooseMapID = choosedId;
			chooseMapUI.OnClosedMapView -= ClosedMapView;
			isActiveUI = false;
		}

		public void ResetChooseID()
		{
			chooseMapID = 0;
		}

		public void AddOpenChooseMap()
		{
			btnPickedMap.onClick.AddListener(() => OpenChooseMapUI());
		}
	}

}
