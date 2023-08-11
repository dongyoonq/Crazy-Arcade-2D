using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace SYJ
{

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

        public Panel curPanel;
        public Panel prevPanel;

        private void Start()
        {
            if (PhotonNetwork.IsConnected)
                OnConnectedToMaster();
            else if (PhotonNetwork.InRoom)
                OnJoinedRoom();
            else if (PhotonNetwork.InLobby)
                OnJoinedLobby();
            else
                OnDisconnected(DisconnectCause.None);

            //SetActivePanel(Panel.Login);
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

            PhotonNetwork.LocalPlayer.SetReady(false);
            PhotonNetwork.LocalPlayer.SetLoad(false);

            PhotonNetwork.AutomaticallySyncScene = true;
            roomPanel.UpdatePlayerList();
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
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            roomPanel.UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            roomPanel.UpdatePlayerList();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            roomPanel.UpdatePlayerList();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
        {
            roomPanel.UpdatePlayerList();
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
        }
    }
}
