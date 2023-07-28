using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField]
    private RectTransform roomType;
    [SerializeField]
    private TMP_Text roomName;
    [SerializeField]
    private TMP_Text roomNumber;
    [SerializeField]
    private TMP_Text roomState;
    [SerializeField]
    private TMP_Text currentPlayer;
    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private Button infoButton;

    private RoomInfo info;

    private List<RoomInfo> currRoomList;

    public void Initialized(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomName.text = info.Name;
        currentPlayer.text = string.Format("{0} / {1}", info.PlayerCount, info.MaxPlayers);
        joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;
        roomState.text = (info.PlayerCount < info.MaxPlayers) ? "Waiting" : "Full";
        roomNumber.text = currRoomList.Count.ToString();
        currRoomList.Add(info);
    }

    public void OnJoinButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
