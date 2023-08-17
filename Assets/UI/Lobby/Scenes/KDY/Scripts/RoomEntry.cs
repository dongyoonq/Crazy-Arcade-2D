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

namespace KDY
{
    public class RoomEntry : MonoBehaviourPunCallbacks
    {
        private const string MAP_PATH = "ChooseMap/Data";

        [SerializeField]
        private List<Sprite> modeSprites;
        [SerializeField]
        private List<Sprite> stateSprites;

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
        [SerializeField]
        private Image passwordImg;
        [SerializeField]
        private Image roomMode;

        private Canvas popUpCanvas;
        private PasswordRoomPanel passwordPanel;

        private string roomPassword;
        private bool isPasswordRoom;

        public int MaxPlayerNum { get; private set; } = 8;

        public RoomInfo RoomInfo { get; private set; }
        public int RoomNumber { get; private set; }

        private void Start()
        {
            infoButton.onClick.AddListener(ShowRoomPlayers);
            popUpCanvas = GameObject.Find("PopUp").GetComponent<Canvas>();
            GetComponent<Button>().onClick.AddListener(() => GameManager.Sound.Onclick());
        }

        public void Initialized(RoomInfo info, int number, PasswordRoomPanel passwordRoomPanel)
        {
			RoomNumber = number;
            passwordPanel = passwordRoomPanel;
            RoomInfo = info;
            MaxPlayerNum = 8;
            
            roomName.text = info.CustomProperties[RoomProp.ROOM_NAME].ToString();
            currentPlayer.text = string.Format("{0} / {1}", info.PlayerCount, info.MaxPlayers);
            joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;

            // 방 모드 체크
            if ((RoomMode)info.CustomProperties[RoomProp.ROOM_MODE] == RoomMode.Manner)
            {
                Debug.Log(modeSprites[0].name);
                roomMode.sprite = modeSprites[0];
            }
            else if ((RoomMode)info.CustomProperties[RoomProp.ROOM_MODE] == RoomMode.Free)
            {
                Debug.Log(modeSprites[1].name);
                roomMode.sprite = modeSprites[1];
            }
            else if ((RoomMode)info.CustomProperties[RoomProp.ROOM_MODE] == RoomMode.Random)
            {
                Debug.Log(modeSprites[2].name);
                roomMode.sprite = modeSprites[2];
            }
            
            // 방 번호 체크
            roomNumber.text = string.Format("{0:D3}", number);
            info.CustomProperties[RoomProp.ROOM_ID] = number;

            // 비번방 체크
            if (info.CustomProperties.ContainsKey(RoomProp.ROOM_PASSWORD))
            {
                roomPassword = info.CustomProperties[RoomProp.ROOM_PASSWORD].ToString().Trim();
                isPasswordRoom = !(roomPassword == "");
            }

            if (isPasswordRoom)
                passwordImg.gameObject.SetActive(true);
            else
                passwordImg.gameObject.SetActive(false);

            // 방 맵 체크
            if (info.CustomProperties.ContainsKey(RoomProp.ROOM_MAP_FILE))
            {
                string path = $"{MAP_PATH}/{info.CustomProperties[RoomProp.ROOM_MAP_FILE]}";
                MapData data = Resources.Load<MapData>(path);
			        	if (data != null)
				        	roomImg.sprite = data.MapIcon;
			      }

            // 방 상태 체크
            if ((bool)info.CustomProperties[RoomProp.ROOM_PLAYING])
            {
                // roomState.sprite = 플레이이미지
                info.CustomProperties[RoomProp.ROOM_STATE] = "Playing";
            }
            else 
            {
                if(info.CustomProperties.ContainsKey(RoomProp.ROOM_MAX))
                {
				            MaxPlayerNum = (int)info.CustomProperties[RoomProp.ROOM_MAX];
				            currentPlayer.text = string.Format("{0} / {1}", info.PlayerCount, MaxPlayerNum);
		    	      }
                SetParticipatedPlayer(info, MaxPlayerNum, info.PlayerCount);
            }       
		    }

        private void SetParticipatedPlayer(RoomInfo info, int maxPlayers, int playerCnt)
        {
			      joinRoomButton.interactable = playerCnt < maxPlayers;

			      if (joinRoomButton.interactable)
			      {
                roomState.sprite = stateSprites[0];
                info.CustomProperties[RoomProp.ROOM_STATE] = "Waiting";
		      	}
			      else
			      {
				        roomState.sprite = stateSprites[1];
				        info.CustomProperties[RoomProp.ROOM_STATE] = "Full";
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