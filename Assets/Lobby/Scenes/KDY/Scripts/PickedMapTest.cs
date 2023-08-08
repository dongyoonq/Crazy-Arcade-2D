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
			Debug.Log("[OpenChooseMapUI] Ŭ��");

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
			//todo.���õ� �̹����� ����

			if (map.title.Contains("ķ��"))
			{
				SetRoomProperty("Map", "Camp");
			}
            else if (map.title.Contains("��ũ ĳ��"))
			{
                SetRoomProperty("Map", "DarkCastle");
            }
			else if (map.title.Contains("���丮"))
			{
                SetRoomProperty("Map", "Factory");
            }
            else if (map.title.Contains("����"))
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
