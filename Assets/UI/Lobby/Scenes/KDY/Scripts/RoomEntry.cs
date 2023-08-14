using CustomProperty;
using Photon.Pun;
using Photon.Realtime;
using RoomUI.ChooseMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Extension;
using static MapDropDown;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace KDY
{
    public class RoomEntry : MonoBehaviourPunCallbacks
    {
        private const string MAP_PATH = "ChooseMap/Data";

		[SerializeField]
        private RectTransform roomType;
        [SerializeField]
        private TMP_Text roomName;
        [SerializeField]
        private TMP_Text roomNumber;
        [SerializeField]
        private Image roomState;
        [SerializeField]
        private TMP_Text currentPlayer;
        [SerializeField]
        private Button joinRoomButton;
        [SerializeField]
        private Button infoButton;
        [SerializeField]
        private Image roomImg;

        private Canvas popUpCanvas;
        private PasswordRoomPanel passwordPanel;

        private string roomPassword;
        private bool isPasswordRoom;

        public RoomInfo RoomInfo { get; private set; }
        public int RoomNumber { get; private set; }

        private void Start()
        {
            infoButton.onClick.AddListener(ShowRoomPlayers);
            popUpCanvas = GameObject.Find("PopUp").GetComponent<Canvas>();
        }

        public void Initialized(RoomInfo info, int number, PasswordRoomPanel passwordRoomPanel)
        {
            RoomNumber = number;
            passwordPanel = passwordRoomPanel;
            RoomInfo = info;

            roomName.text = info.CustomProperties[RoomProp.ROOM_NAME].ToString();
            currentPlayer.text = string.Format("{0} / {1}", info.PlayerCount, info.MaxPlayers);
            joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;

            if (info.PlayerCount < info.MaxPlayers)
            {
                Sprite[] all = Resources.LoadAll<Sprite>("대기방");

                foreach (Sprite s in all)
                    if (s.name == "대기방_5")
                        roomState.sprite = s;

                info.CustomProperties[RoomProp.ROOM_STATE] = "Waiting";
            }
            else
            {
                roomState.sprite = Resources.Load<Sprite>("Full");
                info.CustomProperties[RoomProp.ROOM_STATE] = "Full";
            }

            roomNumber.text = string.Format("{0:D3}", number);
            info.CustomProperties[RoomProp.ROOM_ID] = number;

            if (info.CustomProperties.ContainsKey(RoomProp.ROOM_PASSWORD))
            {
                roomPassword = info.CustomProperties[RoomProp.ROOM_PASSWORD].ToString().Trim();
                isPasswordRoom = !(roomPassword == "");
            }

            if (info.CustomProperties.ContainsKey(RoomProp.ROOM_MAP_FILE))
            {
				string path = $"{MAP_PATH}/{info.CustomProperties[RoomProp.ROOM_MAP_FILE]}";
                MapData data = Resources.Load<MapData>(path);
				if (data != null)
					roomImg.sprite = data.MapIcon;
			}
		}

        public void OnJoinButtonClicked()
        {
            if (passwordPanel == null)
                return;

            if (isPasswordRoom)
            {
                passwordPanel.gameObject.SetActive(true);
                passwordPanel.confirmBtn.onClick.RemoveAllListeners();
                passwordPanel.confirmBtn.onClick.AddListener(PasswordRoomJoin);
                return;
            }

            PhotonNetwork.JoinRoom(RoomNumber.ToString());
        }

        private void PasswordRoomJoin()
        {
            if (passwordPanel.passwordInput.text == roomPassword)
            {
                PhotonNetwork.JoinRoom(RoomNumber.ToString());
                passwordPanel.gameObject.SetActive(false);
            }
            else
            {
                // Todo Fail Match Password
                Debug.Log("방 비밀번호가 다릅니다");
            }
        }

        private void ShowRoomPlayers()
        {
            PlayerListPanel playerListPanel = Instantiate(Resources.Load<PlayerListPanel>("Prefabs/PlayerListPanel"));
            playerListPanel.transform.SetParent(popUpCanvas.transform, false);

            if (RoomInfo.CustomProperties.ContainsKey(RoomProp.PLAYER_LIST))
            {
                string players = RoomInfo.CustomProperties[RoomProp.PLAYER_LIST].ToString();
                playerListPanel.ShowPlayers(players.Split(";"));
            }
        }

        public void SetChangedRoomInfo(string changedType, string value)
        {
            switch (changedType)
            {
                case RoomProp.ROOM_NAME:
                    roomName.text = value;
                    break;
                case RoomProp.ROOM_PASSWORD:
                    isPasswordRoom = !(value == "");
                    roomPassword = value;
                    break;
			}
        }
    }
}