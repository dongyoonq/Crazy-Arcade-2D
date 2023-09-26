using CustomProperty;
using MySql.Data.MySqlClient;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebSocketSharp;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace KDY
{
    public class LobbyPanel : MonoBehaviour
    {
        private const string currentChannelName = "Channel 001";

        [SerializeField] private RoomEntry roomEntryPrefab;
        [SerializeField] private RectTransform roomContent;
        [SerializeField] private Canvas popUpCanvas;
        [SerializeField] private PasswordRoomPanel PasswordPanel;
        [SerializeField] private QuickStartPopup quickStartPopup;

        [SerializeField] GameObject chatingView;
        [SerializeField] GameObject chatingHide;
        [SerializeField] GameObject speakerPopUp;

        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerLevel;
        [SerializeField] private TMP_Text playerExp;
        [SerializeField] private Slider expSlider;

        [SerializeField] RectTransform playerContent;
        [SerializeField] LobbyPlayer playerPrefab;

        [SerializeField]
        private Button btnFastStart;

        private bool isActiveQuickStarView = false;

        public ChatClient chatClient;


		private CreateRoomPanel createRoomPanel;
        public Dictionary<int, RoomInfo> roomDictionary { get; private set; }

        private void Awake()
        {
            roomDictionary = new Dictionary<int, RoomInfo>();
            btnFastStart.onClick.AddListener(() => ActiveFastStart());
        }

		private void OnEnable()
		{
            ReadSqlData();

            if (chatClient.TryGetChannel(currentChannelName, out ChatChannel channel))
            {
                UpdatePlayerList(channel);
            }

            isActiveQuickStarView = false;
		}

		private void Update()
        {
        }

        private void OnDisable()
        {
            for (int i = 0; i < playerContent.childCount; i++)
                Destroy(playerContent.GetChild(i).gameObject);

            roomDictionary.Clear();
        }

        public void ReadSqlData()
        {
            if (!GameManager.Data.reader.IsClosed)
                GameManager.Data.reader.Close();

            string sqlCommand = string.Format("SELECT ID, Level, Exp, ExpMax FROM user_info WHERE ID ='{0}'", PhotonNetwork.NickName);
            MySqlCommand cmd = new MySqlCommand(sqlCommand, GameManager.Data.Connection);
            GameManager.Data.reader = cmd.ExecuteReader();

            if (GameManager.Data.reader.HasRows)
            {
                while (GameManager.Data.reader.Read())
                {
                    playerName.text = GameManager.Data.reader["ID"].ToString();
                    playerLevel.text = string.Format("Lv.  {0}", GameManager.Data.reader["Level"].ToString());
                    float exp = float.Parse(GameManager.Data.reader["Exp"].ToString());
                    float expMax = float.Parse(GameManager.Data.reader["ExpMax"].ToString());
                    expSlider.maxValue = expMax;
                    expSlider.value = exp;

                    playerExp.text = $"{(Mathf.Round((exp / expMax) * 100) * 0.01f) * 100.0f}%";
                }

                if (!GameManager.Data.reader.IsClosed)
                    GameManager.Data.reader.Close();

                return;
            }
        }

        public void UpdatePlayerList(ChatChannel chatChannel)
        {
            // Clear Player List
            for (int i = 0; i < playerContent.childCount; i++)
                Destroy(playerContent.GetChild(i).gameObject);

            // Update Player List
            foreach (string users in chatChannel.Subscribers)
            {
                LobbyPlayer entry = Instantiate(playerPrefab, playerContent);
                entry.Initialized(users);
            }
        }

        public void UpdateRoomList(List<RoomInfo> roomList)
        {
            for (int i = 0; i < roomContent.childCount; i++)
            {
                Destroy(roomContent.GetChild(i).gameObject);
            }

            foreach (RoomInfo room in roomList)
            {
                try
                {
                    int roomNum = int.Parse(room.Name);

                    // 방이 사라질 예정이면 or 방이 비공개가 되었으면 or 방이 닫혔으면
                    if (room.RemovedFromList || !room.IsOpen)
                    {
                        if (roomDictionary.ContainsKey(roomNum))
                            roomDictionary.Remove(roomNum);
                        continue;
                    }

                    // 방이 자료구조에 있었으면 (그냥 무조건 이름이 있었던 방이면 최신으로)
                    if (roomDictionary.ContainsKey(roomNum))
                        roomDictionary[roomNum] = room;

                    else
                        roomDictionary.Add(roomNum, room);
                }
                catch (FormatException e)
                {
                    continue;
                }
            }

            foreach (var data in roomDictionary)
            {
                if (data.Value.IsVisible) //방이 공개 상태일 때만
                {
                    if (data.Value.MaxPlayers > 8) //빠른 시작을 위한 방
                        continue;

                    RoomEntry entry = Instantiate(roomEntryPrefab, roomContent);
                    entry.Initialized(data.Value, data.Key, PasswordPanel);
                }
            }
        }

        public void OnLeaveLobbyButtonClicked()
        {
            GameManager.Sound.Onclick();
            PhotonNetwork.LeaveLobby();
        }

        public void OnCreateRoomButtonClicked()
        {
            GameManager.Sound.Onclick();
            createRoomPanel = Instantiate(Resources.Load<CreateRoomPanel>("Prefabs/CreateRoom"));
            createRoomPanel.transform.SetParent(popUpCanvas.transform, false);
            createRoomPanel.okBtn.onClick.AddListener(OnCreateRoomConfirmButtonClicked);
            createRoomPanel.cancelBtn.onClick.AddListener(OnCreateRoomCancelButtonClicked);
        }

        public void OnCreateRoomCancelButtonClicked()
        {
            GameManager.Sound.Onclick();
            Destroy(createRoomPanel.gameObject);
        }

        public void OnCreateRoomConfirmButtonClicked()
        {
            GameManager.Sound.Onclick();
            CreateRoom();
        }

        public void CreateRoom()
        {
            string roomName = createRoomPanel.roomNameInput.text;
            if (string.IsNullOrEmpty(roomName))
                roomName = $"Room {UnityEngine.Random.Range(0, 1000)}";

            RoomMode mode = createRoomPanel.roomMode.GetSeletedRoom();

            int maxPlayer = 8;
            int roomNumber = PhotonNetwork.CountOfRooms + 1;//GetRoomNumber();
            
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayer;

            roomOptions.CustomRoomProperties = new PhotonHashtable() {
                { RoomProp.ROOM_NAME, roomName },
                { RoomProp.ROOM_PASSWORD, createRoomPanel.passwordToggle.isOn ? createRoomPanel.passwordInput.text : "" },
                { RoomProp.ROOM_ID, roomNumber },
                { RoomProp.ROOM_STATE, "Waiting" },
                { RoomProp.ROOM_MODE, mode },
                { RoomProp.ROOM_MAX, maxPlayer },
                { RoomProp.ROOM_MAP_GROUP, "Random" },
                { RoomProp.ROOM_MAP_FILE, "RandomData" },
                { RoomProp.ROOM_PLAYING, false },
				{ RoomProp.SLOT_STATE, (byte)(Math.Pow(2, 8) - 1) },
			};

			roomOptions.CustomRoomPropertiesForLobby = new string[] { 
              RoomProp.ROOM_NAME, RoomProp.ROOM_PASSWORD, RoomProp.ROOM_ID,
              RoomProp.ROOM_STATE, RoomProp.ROOM_MODE, RoomProp.ROOM_MAX,
              RoomProp.ROOM_MAP_GROUP, RoomProp.ROOM_MAP_FILE,
              RoomProp.ROOM_PLAYING, RoomProp.SLOT_STATE };   
              
            PhotonNetwork.CreateRoom(roomNumber.ToString(), roomOptions, null);

            Destroy(createRoomPanel.gameObject);
        }


        private int GetRoomNumber()
        {
            return roomDictionary.Count() + 1;
        }

        public void RoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
        {
            int roomNumber = int.Parse(PhotonNetwork.CurrentRoom.Name);
            RoomEntry changedRoom = GetRoomEntry(roomNumber);

            if (changedRoom != null)
            {
                if (propertiesThatChanged.ContainsKey(RoomProp.ROOM_NAME))
                {
                    changedRoom.SetChangedRoomInfo(RoomProp.ROOM_PASSWORD, propertiesThatChanged[RoomProp.ROOM_NAME].ToString().Trim());
                }

                if (propertiesThatChanged.ContainsKey(RoomProp.ROOM_PASSWORD))
                {
                    changedRoom.SetChangedRoomInfo(RoomProp.ROOM_PASSWORD, propertiesThatChanged[RoomProp.ROOM_PASSWORD].ToString().Trim());
                }

                if (propertiesThatChanged.ContainsKey(RoomProp.ROOM_MODE))
                {
                    RoomMode mode = (RoomMode)Enum.Parse(typeof(RoomMode), propertiesThatChanged[RoomProp.ROOM_MODE].ToString().Trim());
                    createRoomPanel.roomMode.ChooseRoomMode(mode);
                }
            }
        }

        private RoomEntry GetRoomEntry(int roomNumber)
        {
            return roomContent.GetComponentsInChildren<RoomEntry>().Where(x => x.RoomNumber == roomNumber).FirstOrDefault();
        }

        private void ActiveFastStart()
        {
			quickStartPopup.gameObject.SetActive(true);
			quickStartPopup.InitView();
		}

        public void OnClicked()
        {
            GameManager.Sound.Onclick();
        }

        private bool chatingHideButtonCheck = true;

        public void OnUpButtonClicked()
        {
            GameManager.Sound.Onclick();
            if (chatingHideButtonCheck == false)
            {
                chatingHide.transform.Translate(Vector3.up * 330);
                chatingView.SetActive(true);
            }
            chatingHideButtonCheck = true;
        }

        public void OnDownButtonClicked()
        {
            GameManager.Sound.Onclick();
            if (chatingHideButtonCheck == true)
            {
                chatingHide.transform.Translate(Vector3.down * 330);
                chatingView.SetActive(false);
            }
            chatingHideButtonCheck = false;
        }

        public void OnSpeakerButtonClicked()
        {
            GameManager.Sound.Onclick();
            if (speakerPopUp.active == false)
            {
                speakerPopUp.SetActive(true);
            }
        }
    }
}