using CustomProperty;
using KDY;
using Photon.Pun;
using Photon.Realtime;
using RoomUI.ChangedRoomInfo;
using RoomUI.Chat;
using RoomUI.ChooseTeam;
using RoomUI.PlayerSetting;
using RoomUI.SetGameReady;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace Gangbie
{
    public class RoomPanel : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private RoomChangedInfo roomInfo;

        [SerializeField]
        private RectTransform playerContent;

        [SerializeField]
        private PickedTeam pickedTeam;

        [SerializeField]
        private GameStartController gameStartController;

        [SerializeField]
        private List<CharacterData> characterDatas;

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

        public void PlayerPropertiesUpdate(Player player, PhotonHashtable changedProps)
        {
            if (changedProps.ContainsKey(PlayerProp.READY))
            {
                GetPalyerEntry(player)?.UpdateReadyInfo();

                if (PhotonNetwork.IsMasterClient)
                    CheckPlayerReadyState();
            }

            else if (changedProps.ContainsKey(PlayerProp.TEAM))
            {
                Color teamColor;
                string hexColor = player.CustomProperties[PlayerProp.TEAM].ToString();
                UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out teamColor);

                UpdateOtherPlayerTeam(player.ActorNumber, teamColor);
            }

            else if (changedProps.ContainsKey(PlayerProp.CHARACTER))
            {
                UpdateOtherPlayerCharacter(player.ActorNumber, player.CustomProperties[PlayerProp.CHARACTER].ToString());
            }
        }

        private CharacterData GetCharacterData(CharacterEnum characterEnum)
        {
            return characterDatas.Where(x => x.Name == characterEnum.ToString()).FirstOrDefault();
        }

        public void UpdatePlayerState(Player player, bool isReady)
        {
            GetPalyerEntry(player)?.UpdateReadyInfo();

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
            playerDictionary.Add(player.ActorNumber, waitingPlayer);

            if (player.IsLocal)
            {
                pickedTeam.InitTeam();
                InitCharacter();
            }
            else
            {
                if (player.CustomProperties.ContainsKey(PlayerProp.CHARACTER))
                    UpdateOtherPlayerCharacter(player.ActorNumber, player.CustomProperties[PlayerProp.CHARACTER].ToString());
            }
        }

        private void UpdateOtherPlayerCharacter(int actorNumber, string characterKey)
        {
            CharacterEnum character = (CharacterEnum)Enum.Parse(typeof(CharacterEnum), characterKey);

            CharacterData data = GetCharacterData((CharacterEnum)character);
            if (data != null)
            {
                playerDictionary[actorNumber].PlayerSet.PlayerImg.sprite = data.Character;
            }
        }

        private void UpdateOtherPlayerTeam(int actorNumber, Color color)
        {
            playerDictionary[actorNumber].PlayerSet.TeamColor.color = color;
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

            UpdateMasterPlayerState(newMaster.ActorNumber);
        }

        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient && isPassableStarting)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;

                LoadMapScene();
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void InitCharacter()
        {
            PhotonHashtable property = new PhotonHashtable();
            property[PlayerProp.CHARACTER] = CharacterEnum.Dao;
            PhotonNetwork.LocalPlayer.SetCustomProperties(property);
        }

        private void LoadMapScene()
        {
            // ROOM_MAP is MapData.Title
            switch ((string)PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.ROOM_MAP])
            {
                // Todo Scene Load
                default:
                    PhotonNetwork.LoadLevel("GameScene");
                    break;
            }
        }
    }
}
