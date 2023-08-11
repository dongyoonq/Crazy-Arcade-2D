using CustomProperty;
using MySql.Data.MySqlClient;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using SYJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace KDY
{
    public class LobbyPanel : MonoBehaviour
    {
		[SerializeField] private RoomEntry roomEntryPrefab;
        [SerializeField] private RectTransform roomContent;
        [SerializeField] private Canvas popUpCanvas;
        [SerializeField] private PasswordRoomPanel PasswordPanel;

		[SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerLevel;
        [SerializeField] private TMP_Text playerExp;

        [SerializeField] RectTransform playerContent;
        [SerializeField] LobbyPlayer playerPrefab;

        private MySqlConnection connection;
        private MySqlDataReader reader;

        private CreateRoomPanel createRoomPanel;
        public Dictionary<int, RoomInfo> roomDictionary { get ; private set; }

		private void Awake()
        {
            roomDictionary = new Dictionary<int, RoomInfo>();
		}

        private void Start()
        {
            //ConnectDataBase();
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
                try
                {
					int roomNum = int.Parse(room.Name);

					// ���� ����� �����̸� or ���� ������� �Ǿ����� or ���� ��������
					if (room.RemovedFromList || !room.IsOpen)
					{
						if (roomDictionary.ContainsKey(roomNum))
							roomDictionary.Remove(roomNum);
						continue;
					}

					// ���� �ڷᱸ���� �־����� (�׳� ������ �̸��� �־��� ���̸� �ֽ�����)
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
                if(data.Value.IsVisible) //���� ���� ������ ����
				{
					RoomEntry entry = Instantiate(roomEntryPrefab, roomContent);
					entry.Initialized(data.Value, data.Key, PasswordPanel);
				}
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
            int roomNumber = GetRoomNumber();

			RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayer;

			roomOptions.CustomRoomProperties = new PhotonHashtable() {
					{ RoomProp.ROOM_NAME, roomName },
					{ RoomProp.ROOM_MODE, RoomMode.Free },
					{ RoomProp.ROOM_PASSWORD, createRoomPanel.passwordToggle.isOn ? createRoomPanel.passwordInput.text : "" },
					{ RoomProp.ROOM_ID, roomNumber },
					{ RoomProp.ROOM_STATE, "Waiting" },
				};

			roomOptions.CustomRoomPropertiesForLobby = new string[]
			{ RoomProp.ROOM_NAME, RoomProp.ROOM_PASSWORD, RoomProp.ROOM_ID, RoomProp.ROOM_STATE, RoomProp.ROOM_MAP_GROUP, RoomProp.ROOM_MODE };

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

				else if (propertiesThatChanged.ContainsKey(RoomProp.ROOM_PASSWORD))
				{
                    changedRoom.SetChangedRoomInfo(RoomProp.ROOM_PASSWORD, propertiesThatChanged[RoomProp.ROOM_PASSWORD].ToString().Trim());
				}

                else if (propertiesThatChanged.ContainsKey(RoomProp.ROOM_MODE))
				{
					RoomMode mode = (RoomMode)Enum.Parse(typeof(RoomMode), propertiesThatChanged[RoomProp.ROOM_MODE].ToString().Trim());
                    changedRoom.SetChangedRoomInfo(mode);
				}
			}
		}

		private RoomEntry GetRoomEntry(int roomNumber)
		{
			return roomContent.GetComponentsInChildren<RoomEntry>().Where(x => x.RoomNumber == roomNumber).FirstOrDefault();
		}
	}
}