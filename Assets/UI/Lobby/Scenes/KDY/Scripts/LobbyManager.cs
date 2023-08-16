using CustomProperty;
using CustomProperty.Utils;
using KDY;
using Photon.Pun;
using Photon.Realtime;
using RoomUI;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Lobby, Room, Shop }

    [SerializeField]
    private LoginPanel loginPanel;
    [SerializeField]
    private RoomPanel roomPanel;
    [SerializeField]
    private LobbyPanel lobbyPanel;
    [SerializeField]
    private ShopPanel shopPanel;

    public List<RoomInfo> Rooms { get; private set; }

    public Panel curPanel;
    public Panel prevPanel;

    private void Start()
    {
        SetActivePanel(Panel.Login);

        if (PhotonNetwork.InRoom)
            OnJoinedRoom();
        else if (PhotonNetwork.InLobby)
            OnJoinedLobby();
        else if (PhotonNetwork.IsConnected)
            OnConnectedToMaster();
        else
            OnDisconnected(DisconnectCause.None);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(Panel.Login);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Lobby);
        Debug.Log(string.Format("Create room failed with error({0}) : {1}", returnCode, message));
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(Panel.Room);

		PhotonHashtable playerProperty = new PhotonHashtable();
        playerProperty[PlayerProp.READY] = PhotonNetwork.IsMasterClient;
        playerProperty[PlayerProp.LOAD] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);

		PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Lobby);
        Debug.Log(string.Format("Join room failed with error({0}) : {1}", returnCode, message));
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(string.Format("Join random room failed with error({0}) : {1}", returnCode, message));
        Debug.Log("Create room instead");

        string roomName = string.Format("Room {0}", Random.Range(0, 1000));
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 8 });
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        SetActivePanel(Panel.Lobby);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
        Rooms = roomList;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.EntryPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPanel.LeavePlayer(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsMasterClient)
            roomPanel.SwitchedMasterPlayer(newMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        roomPanel.PlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        lobbyPanel.RoomPropertiesUpdate(propertiesThatChanged);
        roomPanel.RoomPropertiesUpdate(propertiesThatChanged);
    }

    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Login);
    }

    public void SetActivePanel(Panel panel)
    {
        prevPanel = curPanel;
        curPanel = panel;

        loginPanel.gameObject?.SetActive(panel == Panel.Login);
        lobbyPanel.gameObject?.SetActive(panel == Panel.Lobby);
        roomPanel.gameObject?.SetActive(panel == Panel.Room);
        shopPanel.gameObject?.SetActive(panel == Panel.Shop);

        if (panel == Panel.Login)
        {
            GameManager.Sound.BgmStop(GameManager.Sound.lobbySource);
            GameManager.Sound.BgmPlay(GameManager.Sound.loginSource);
        }
        else
        {
            GameManager.Sound.BgmStop(GameManager.Sound.loginSource);
            GameManager.Sound.BgmPlay(GameManager.Sound.lobbySource);
        }
    }
}
