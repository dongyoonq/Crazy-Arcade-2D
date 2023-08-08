using Photon.Pun;
using Photon.Realtime;
using RoomUI.ChooseMap;
using SYJ;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace KDY
{
    public class RoomEntry : MonoBehaviour
    {
        
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
        PasswordRoomPanel passwordPanel;

        public Dictionary<int, Player> roomPlayers;
        private string roomPassword;
        private bool isPasswordRoom;
        public RoomInfo info;

        private void Start()
        {
            passwordPanel = transform.parent.parent.parent.GetChild(2).GetComponent<PasswordRoomPanel>();
            infoButton.onClick.AddListener(ShowRoomPlayers);
            popUpCanvas = GameObject.Find("PopUp").GetComponent<Canvas>();
        }

        public void Initialized(RoomInfo roomInfo, int number)
        {
            info = roomInfo;
            roomName.text = info.CustomProperties["RoomName"].ToString();
            currentPlayer.text = string.Format("{0} / {1}", info.PlayerCount, info.MaxPlayers);
            joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;
            
            if (info.PlayerCount < info.MaxPlayers)
            {
                Sprite[] all = Resources.LoadAll<Sprite>("대기방");

                foreach (Sprite s in all)
                    if (s.name == "대기방_5")
                        roomState.sprite = s;

                info.CustomProperties["RoomState"] = "Waiting";
            }
            else
            {
                roomState.sprite = Resources.Load<Sprite>("Full");
                info.CustomProperties["RoomState"] = "Full";
            }

            roomNumber.text = string.Format("{0:D3}", number);
            info.CustomProperties["RoomId"] = number;

            if (info.CustomProperties.ContainsKey("Password"))
            {
                isPasswordRoom = true;
                roomPassword = (string)info.CustomProperties["Password"];
            }

            if ((string)info.CustomProperties["Map"] == "Camp")
            {
                roomImg.sprite = Resources.Load<Sprite>("Map/CampMap");
            }
            else if ((string)info.CustomProperties["Map"] == "DarkCastle")
            {
                roomImg.sprite = Resources.Load<Sprite>("Map/DarkCastleMap");
            }
            else if ((string)info.CustomProperties["Map"] == "Factory")
            {
                roomImg.sprite = Resources.Load<Sprite>("Map/FactoryMap");
            }
            else if ((string)info.CustomProperties["Map"] == "Random")
            {
                roomImg.sprite = Resources.Load<Sprite>("Map/AllRandomMap");
            }
        }

        public void OnJoinButtonClicked()
        {
            //PhotonNetwork.LeaveLobby();
            if (isPasswordRoom)
            {
                passwordPanel.gameObject.SetActive(true);
                passwordPanel.confirmBtn.onClick.RemoveAllListeners();
                passwordPanel.confirmBtn.onClick.AddListener(PasswordRoomJoin);
                return;
            }

            PhotonNetwork.JoinRoom(roomName.text);
        }

        private void PasswordRoomJoin()
        {
            if (passwordPanel.passwordInput.text == roomPassword)
            {
                PhotonNetwork.JoinRoom(roomName.text);
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
            //playerListPanel.ShowPlayers(roomPlayers);
        }
    }
}