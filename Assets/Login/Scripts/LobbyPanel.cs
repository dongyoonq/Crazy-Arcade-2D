using KDY;
using MySql.Data.MySqlClient;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace Gangbie
{
    public class LobbyPanel : MonoBehaviour
    {
        private const string ROOM_NAME = "RoomName";
        private const string PASSWORD = "Password";
        private const string ROOM_ID = "RoomId";

        [SerializeField] private RoomEntry roomEntryPrefab;
        [SerializeField] private RectTransform roomContent;
        [SerializeField] private Canvas popUpCanvas;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerLevel;
        [SerializeField] private TMP_Text playerExp;

        [SerializeField] RectTransform playerContent;
        [SerializeField] LobbyPlayer playerPrefab;

        private MySqlConnection connection;
        private MySqlDataReader reader;

        private CreateRoomPanel createRoomPanel;
        private Dictionary<string, RoomInfo> roomDictionary;

        private void Awake()
        {
            roomDictionary = new Dictionary<string, RoomInfo>();
        }

        private void Start()
        {
            ConnectDataBase();
        }

        private void ConnectDataBase()
        {
            try
            {
                string serverInfo = "Server=127.0.0.1; Database=userdata; Uid=root; Pwd=1234; Port=3306; CharSet=utf8;";
                connection = new MySqlConnection(serverInfo);
                connection.Open();

                Debug.Log("DataBase Connect Success");
            }
            catch (InvalidCastException e)
            {
                Debug.Log(e.Message);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void Update()
        {
            playerName.text = PhotonNetwork.NickName;
            //ReadSqlData();
        }

        private void OnDisable()
        {
            roomDictionary.Clear();
        }

        public void ReadSqlData()
        {
            string sqlCommand = string.Format("SELECT Level,Exp FROM user_info WHERE ID ='{0}'", PhotonNetwork.NickName);
            MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    playerLevel.text = string.Format("Lv.  {0}", reader["Level"].ToString());
                    playerExp.text = reader["Exp"].ToString();
                }

                if (!reader.IsClosed)
                    reader.Close();

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
                // 방이 사라질 예정이면 or 방이 비공개가 되었으면 or 방이 닫혔으면
                if (room.RemovedFromList || !room.IsVisible || !room.IsOpen)
                {
                    if (roomDictionary.ContainsKey(room.Name))
                    {
                        roomDictionary.Remove(room.Name);
                    }

                    continue;
                }

                // 방이 자료구조에 있었으면 (그냥 무조건 이름이 있었던 방이면 최신으로)
                if (roomDictionary.ContainsKey(room.Name))
                    roomDictionary[room.Name] = room;
                else
                    roomDictionary.Add(room.Name, room);
            }

            int cnt = 0;

            foreach (RoomInfo room in roomDictionary.Values)
            {
                RoomEntry entry = Instantiate(roomEntryPrefab, roomContent);
                entry.Initialized(room, cnt++);
            }
        }

        public void OnLeaveLobbyButtonClicked()
        {
            PhotonNetwork.LeaveLobby();
        }

        public void OnCreateRoomButtonClicked()
        {
            createRoomPanel = Instantiate(Resources.Load<CreateRoomPanel>("Prefabs/CreateRoom"));
            createRoomPanel.transform.SetParent(popUpCanvas.transform, false);
            createRoomPanel.okBtn.onClick.AddListener(OnCreateRoomConfirmButtonClicked);
            createRoomPanel.cancelBtn.onClick.AddListener(OnCreateRoomCancelButtonClicked);
        }

        public void OnCreateRoomCancelButtonClicked()
        {
            Destroy(createRoomPanel.gameObject);
        }

        public void OnCreateRoomConfirmButtonClicked()
        {
            CreateRoom();
        }

        public void CreateRoom()
        {
            string roomName = createRoomPanel.roomNameInput.text;
            if (string.IsNullOrEmpty(roomName))
                roomName = $"Room {UnityEngine.Random.Range(0, 1000)}";

            int maxPlayer = 8;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)maxPlayer;

            roomOptions.CustomRoomProperties = new PhotonHashtable() { { "RoomName", roomName }, { "RoomId", GetRoomNumber() }, { "RoomState", "Waiting" } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "RoomName", "RoomId", "RoomState", "Map", "Mode" };

            if (createRoomPanel.passwordToggle.isOn)
            {
                roomOptions.CustomRoomProperties = new PhotonHashtable() {
                    { "RoomName", roomName },
                    { "Password", createRoomPanel.passwordInput.text },
                    { "RoomId", GetRoomNumber() },
                    { "RoomState", "Waiting" },
                };

                roomOptions.CustomRoomPropertiesForLobby = new string[] { "RoomName", "Password", "RoomId", "RoomState", "Map", "Mode" };
            }

            PhotonNetwork.CreateRoom(roomName, roomOptions, null);

            Destroy(createRoomPanel.gameObject);
        }

        private int GetRoomNumber()
        {
            int cnt = 0;

            foreach (RoomInfo room in roomDictionary.Values)
            {
                cnt++;
            }

            return cnt;
        }

        public void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(ROOM_ID))
            {
                string key = propertiesThatChanged[ROOM_ID].ToString();

                if (propertiesThatChanged.ContainsKey(ROOM_NAME))
                {
                    //현재 방 이름과 다른 경우 변경
                }

                if (propertiesThatChanged.ContainsKey(PASSWORD) && propertiesThatChanged[PASSWORD].ToString().Trim() != "")
                {
                    //암호방 설정하기
                }
            }
        }
    }
}
