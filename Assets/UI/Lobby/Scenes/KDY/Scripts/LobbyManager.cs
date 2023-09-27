using CustomProperty;
using CustomProperty.Utils;
using KDY;
using LobbyUI.QuickStart;
using Photon.Pun;
using Photon.Realtime;
using RoomUI;
using RoomUI.ChooseTeam;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public const string QUICK_MATCH_ROOM_NAME = "9999";

    public enum Panel { Login, Lobby, Room, Shop, Matching }

    [SerializeField]
    private LoginPanel loginPanel;
    [SerializeField]
    private RoomPanel roomPanel;
    [SerializeField]
    private LobbyPanel lobbyPanel;
    [SerializeField]
    private ShopPanel shopPanel;
	[SerializeField]
	private QuickMatching quickMatchPanel;

	public List<RoomInfo> Rooms { get; private set; }

    public Panel curPanel;
    public Panel prevPanel;

    private string defaultTeamColor = $"#{Color.red.ToHexString()}";

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
		PhotonNetwork.AutomaticallySyncScene = true;

		if (PhotonNetwork.CurrentRoom.MaxPlayers > 8)
            SetActivePanel(Panel.Matching);
        else
        {
			SetActivePanel(Panel.Room);

			PhotonHashtable playerProperty = new PhotonHashtable();
			playerProperty[PlayerProp.READY] = PhotonNetwork.IsMasterClient;
			playerProperty[PlayerProp.LOAD] = false;

            if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(RoomProp.ROOM_PLAYING) == false || (bool)PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.ROOM_PLAYING] == false)
            {
				playerProperty[PlayerProp.CHARACTER] = CharacterEnum.Dao.ToString();
				playerProperty[PlayerProp.TEAM] = "Red";
				playerProperty[PlayerProp.TEAMCOLOR] = defaultTeamColor;
			}
			PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
		}
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

		PhotonHashtable playerProperty = new PhotonHashtable();
        playerProperty[PlayerProp.SLOT_NUMBER] = -1;
		PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
	}

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
        Rooms = roomList;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.MaxPlayers > 8)
			quickMatchPanel.EntryPlayer(newPlayer);
        else
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
        lobbyPanel.gameObject?.SetActive(panel == Panel.Lobby || panel == Panel.Matching);
        roomPanel.gameObject?.SetActive(panel == Panel.Room);
        shopPanel.gameObject?.SetActive(panel == Panel.Shop);
        quickMatchPanel.gameObject?.SetActive(panel == Panel.Matching);
        
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
