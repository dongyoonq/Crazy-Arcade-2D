using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
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
    private Image roomState;
    [SerializeField]
    private TMP_Text currentPlayer;
    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private Button infoButton;

    private int roomId;
    private RoomInfo info;

    public void Initialized(RoomInfo roomInfo, int number)
    {
        info = roomInfo;
        roomName.text = info.Name;
        currentPlayer.text = string.Format("{0} / {1}", info.PlayerCount, info.MaxPlayers);
        joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;
        roomState.sprite = (info.PlayerCount < info.MaxPlayers) ? Resources.Load<Sprite>("Waiting") : Resources.Load<Sprite>("Full");
        roomId = number;
        roomNumber.text = number.ToString();
    }

    public void OnJoinButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
