using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField]
    private RoomEntry roomEntryPrefab;
    [SerializeField]
    private RectTransform roomContent;
    [SerializeField]
    private GameObject createRoomPanel;

    [SerializeField]
    private TMP_InputField roomNameInputField;
    [SerializeField]
    private TMP_InputField maxPlayerInputField;

    private Dictionary<string, RoomInfo> roomDictionary;

    private void Awake()
    {
        roomDictionary = new Dictionary<string, RoomInfo>();
    }

    private void OnDisable()
    {
        roomDictionary.Clear();
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
            entry.Initialized(room, ++cnt);
        }
    }

    public void OnLeaveLobbyButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void OnCreateRoomButtonClicked()
    {
        createRoomPanel.SetActive(true);
    }

    public void OnCreateRoomCancelButtonClicked()
    {
        createRoomPanel.SetActive(false);
    }

    public void OnCreateRoomConfirmButtonClicked()
    {
        CreateRoom();
    }

    public void CreateRoom()
    {
        createRoomPanel.SetActive(false);

        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
            roomName = $"Room {Random.Range(0, 1000)}";

        int maxPlayer = (string.IsNullOrEmpty(maxPlayerInputField.text)) ? 8 : int.Parse(maxPlayerInputField.text);
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayer;

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }
}
