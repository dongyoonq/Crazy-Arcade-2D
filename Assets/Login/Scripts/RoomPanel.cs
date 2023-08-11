using KDY;
using Photon.Pun;
using Photon.Realtime;
using RoomUI.ChangedRoomInfo;
using RoomUI.Chat;
using RoomUI.PlayerSetting;
using RoomUI.SetGameReady;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gangbie
{
    public class RoomPanel : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private RoomChangedInfo roomInfo;

        [SerializeField]
        private RectTransform playerContent;

        [SerializeField]
        private GameStartController gameStartController;

        private Dictionary<int, WaitingPlayer> playerDictionary;

        private bool isPassableStarting;

        private void Awake()
        {
            playerDictionary = new Dictionary<int, WaitingPlayer>();

            gameStartController.BtnGameReady.ReadyBtn.onClick.AddListener(() => StartGame());
        }

        private void OnEnable()
        {
            SetInPlayer();
            CheckPlayerReadyState();

            NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Warning, $"{PhotonNetwork.NickName}¥‘¿Ã ¬¸∞°«œºÃΩ¿¥œ¥Ÿ.");
        }

        private void OnDisable()
        {
            foreach (int actorNumber in playerDictionary.Keys)
            {
                Destroy(playerDictionary[actorNumber].gameObject);
            }
            playerDictionary.Clear();
        }

        public void EntryPlayer(Player player)
        {
            InstantiatePlayer(player);
            CheckPlayerReadyState();
        }

        public void LeavePlayer(Player leavePlayer)
        {
            Destroy(playerDictionary[leavePlayer.ActorNumber].gameObject);
            playerDictionary.Remove(leavePlayer.ActorNumber);

            Instantiate<WaitingPlayer>(Resources.Load<WaitingPlayer>("WaitingPlayer"), playerContent);

            CheckPlayerReadyState();

            if (PhotonNetwork.IsMasterClient)
                NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Warning, $"{leavePlayer.NickName}¥‘¿Ã ≈¿Â«œºÃΩ¿¥œ¥Ÿ,");
        }

        private void AddPlayer()
        {
            if (playerContent.childCount < 8)
                Instantiate<WaitingPlayer>(Resources.Load<WaitingPlayer>("WaitingPlayer"), playerContent);
        }

        public void UpdatePlayerState(Player player)
        {
            GetPalyerEntry(player)?.UpdateReadyInfo();

            if (PhotonNetwork.IsMasterClient)
                CheckPlayerReadyState();
        }

        public void UpdatePlayerState(Player player, bool isReady)
        {
            GetPalyerEntry(player)?.UpdateReadyInfo(isReady);

            if (PhotonNetwork.IsMasterClient)
                CheckPlayerReadyState();
        }

        private void SetInPlayer()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                InstantiatePlayer(player);
            }
        }

        private void InstantiatePlayer(Player player)
        {
            int index = playerDictionary.Count();

            if (index == 8)
                return;

            WaitingPlayer waitingPlayer = playerContent.GetComponentsInChildren<WaitingPlayer>()[index];
            waitingPlayer.SetPlayer(player);
            waitingPlayer.OnChangedOtherPlayerCharacter += UpdateOtherPlayerCharacter;
            waitingPlayer.OnChangedOtherPlayerState += UpdateOtherPlayerState;
            waitingPlayer.OnChangedMasterPlayerState += UpdateMasterPlayerState;
            playerDictionary.Add(player.ActorNumber, waitingPlayer);

            if (player.IsLocal)
                gameStartController.OnChangeReadyState += UpdatePlayerState;
        }

        private void UpdateOtherPlayerCharacter(int actorNumber, CharacterData data)
        {
            playerDictionary[actorNumber].playerImg.sprite = data.Character;
        }

        private void UpdateOtherPlayerState(int actorNumber, bool isReady)
        {
            playerDictionary[actorNumber].WaitState.UpdateReadyInfo(isReady);
        }

        private void UpdateMasterPlayerState(int actorNumber)
        {
            playerDictionary[actorNumber].WaitState.UpdateMasterInfo();
        }


        private WaitingPlayer GetPalyerEntry(Player chkPlayer)
        {
            return playerContent.GetComponentsInChildren<WaitingPlayer>().Where(x => x.player.ActorNumber == chkPlayer.ActorNumber).FirstOrDefault();
        }

        public void UpdateActiveStartButton(int actorNumber)
        {
            //startButton.gameObject.SetActive()
        }

        public void CheckPlayerReadyState()
        {
            isPassableStarting = false;

            if (PhotonNetwork.IsMasterClient)
            {
                int readyCount = PhotonNetwork.PlayerList.Count(x => x.GetReady());

                if (readyCount > 1)
                {
                    isPassableStarting = (readyCount == PhotonNetwork.PlayerList.Length);
                }
            }

            isPassableStarting = true;
        }

        public void SwitchedMasterPlayer(Player newMaster)
        {
            roomInfo.SetMasterRoomInfo();
            playerDictionary[newMaster.ActorNumber].WaitState.UpdateMasterInfo();
            playerDictionary[newMaster.ActorNumber].UpdateMasterInfo();
            gameStartController.BtnGameReady.SetReadyBtnImg();
            CheckPlayerReadyState();
        }

        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient && isPassableStarting)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;

                PhotonNetwork.LoadLevel("GameScene");
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        internal void UpdatePlayerList()
        {
            throw new NotImplementedException();
        }
    }
}
