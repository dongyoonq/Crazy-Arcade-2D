using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace RoomUI.ChooseMap
{
	public class PickedMapTest : MonoBehaviourPun
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

			if (map.title.Contains("캠프"))
			{
				SetRoomProperty("Map", "Camp");
			}
            else if (map.title.Contains("다크 캐슬"))
			{
                SetRoomProperty("Map", "DarkCastle");
            }
			else if (map.title.Contains("팩토리"))
			{
                SetRoomProperty("Map", "Factory");
            }
            else if (map.title.Contains("랜덤"))
            {
                SetRoomProperty("Map", "Random");
            }

            chooseMapUI = null;
		}

        private void SetRoomProperty(string propertyKey, string value)
        {
            PhotonHashtable property = PhotonNetwork.CurrentRoom.CustomProperties;

            property[propertyKey] = value;
            PhotonNetwork.CurrentRoom.SetCustomProperties(property);
        }
    }

}
