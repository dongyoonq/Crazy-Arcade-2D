using MySql.Data.MySqlClient;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private RoomEntry roomEntryPrefab;
    [SerializeField] private RectTransform roomContent;
    [SerializeField] private Canvas popUpCanvas;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerLevel;
    [SerializeField] private TMP_Text playerExp;

    [SerializeField] RectTransform playerContent;
    [SerializeField] LobbyPlayer playerPrefab;

    private MySqlDataReader reader;
    private CreateRoomPanel createRoomPanel;
    private Dictionary<string, RoomInfo> roomDictionary;

    private void Update()
    {
        playerName.text = PhotonNetwork.NickName;
        ReadSqlData();
    }

    private void Awake()
    {
        roomDictionary = new Dictionary<string, RoomInfo>();
    }

    private void OnDisable()
    {
        roomDictionary.Clear();
    }

    public void ReadSqlData()
    {
        string sqlCommand = string.Format("SELECT Level,Exp FROM user_info WHERE ID ='{0}'", PhotonNetwork.NickName);
        MySqlCommand cmd = new MySqlCommand(sqlCommand, LoginPanel.connection);
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

    public void UpdatePlayerList()
    {
        // Clear Player List
        for (int i = 0; i < playerContent.childCount; i++)
            Destroy(playerContent.GetChild(i).gameObject);

        Debug.Log(string.Format("{0}", PhotonNetwork.PlayerList.Length));

        // Update Player List
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            LobbyPlayer entry = Instantiate(playerPrefab, playerContent);
            entry.Initialized(player);
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
            // ���� ����� �����̸� or ���� ������� �Ǿ����� or ���� ��������
            if (room.RemovedFromList || !room.IsVisible || !room.IsOpen)
            {
                if (roomDictionary.ContainsKey(room.Name))
                {
                    roomDictionary.Remove(room.Name);
                }

                continue;
            }

            // ���� �ڷᱸ���� �־����� (�׳� ������ �̸��� �־��� ���̸� �ֽ�����)
            if (roomDictionary.ContainsKey(room.Name))
                roomDictionary[room.Name] = room;
            else
                roomDictionary.Add(room.Name, room);
        }

        int cnt = 0;

        foreach (RoomInfo room in roomDictionary.Values)
        {
            RoomEntry entry = Instantiate(roomEntryPrefab, roomContent);
            entry.Initialized(room, ++cnt);
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
            roomName = $"Room {Random.Range(0, 1000)}";

        int maxPlayer = 8;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayer;

        roomOptions.CustomRoomProperties = new PhotonHashtable() { { "RoomName", roomName } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "RoomName" };

        if (createRoomPanel.passwordToggle.isOn)
        {
            roomOptions.CustomRoomProperties = new PhotonHashtable() { { "Password", createRoomPanel.passwordInput.text } };
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "Password" };
        }

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);

        Destroy(createRoomPanel.gameObject);
    }
}