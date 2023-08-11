using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using CustomProperty.Utils;
using RoomUI.PlayerSetting;
using RoomUI.ChangedRoomInfo;
using RoomUI.SetGameReady;
using RoomUI.Chat;
using RoomUI.ChooseTeam;
using RoomUI.ChooseMap;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using CustomProperty;
using static Extension;
using UnityEngine.Networking.Types;
using UnityEngine.Events;

namespace RoomUI
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
		private PickedMap pickedMap;

		[SerializeField]
		private GameStartController gameStartController;

		[SerializeField]
		private RoomNotify roomNotifyPopup;

		[SerializeField]
		private List<CharacterData> characterDatas;

		private Dictionary<int, WaitingPlayer> playerDictionary;

		private bool isPassableStarting;

		private void Awake()
		{
			playerDictionary = new Dictionary<int, WaitingPlayer>();

			gameStartController.BtnGameReady.ReadyBtn.onClick.AddListener(() => StartGame());

			int num = 0;
			var slots = playerContent.GetComponentsInChildren<WaitingPlayer>();
			foreach (var waitSlot in slots)
				waitSlot.Slot.SlotNumber = num++;
		}

		private void OnEnable()
		{
			SetInPlayer();
			CheckPlayerReadyState();

			NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Warning, $"{PhotonNetwork.NickName}님이 참가하셨습니다.");

			SetPlayerList();

			
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
			SetPlayerList();
		}

		public void LeavePlayer(Player leavePlayer)
		{
			Destroy(playerDictionary[leavePlayer.ActorNumber].gameObject);
			playerDictionary.Remove(leavePlayer.ActorNumber);

			int maxID = playerContent.GetComponentsInChildren<WaitingPlayer>().Max(x => x.Slot.SlotNumber);
			WaitingPlayer newslot = Instantiate<WaitingPlayer>(Resources.Load<WaitingPlayer>("WaitingPlayer"), playerContent);
			newslot.Slot.SlotNumber = maxID + 1;

			CheckPlayerReadyState();

			if(PhotonNetwork.IsMasterClient)
				NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Warning, $"{leavePlayer.NickName}님이 퇴장하셨습니다,");

			SetPlayerList();
		}

		private void AddPlayer()
		{
			if (playerContent.childCount < 8)
			{
				Instantiate<WaitingPlayer>(Resources.Load<WaitingPlayer>("WaitingPlayer"), playerContent);
			}	
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

		private void SetPlayerList()
		{
			/*
			if (PhotonNetwork.IsMasterClient == false)
				return;

			var list = playerDictionary.Select(x => x.Value.player.NickName).ToList();
			string players = string.Join(";", list);
			PhotonNetwork.CurrentRoom.SetRoomProperty(RoomProp.PLAYER_LIST, players);
			//*/
		}


		public void PlayerPropertiesUpdate(Player player, PhotonHashtable changedProps)
		{
			if (changedProps.ContainsKey(PlayerProp.READY))
			{
				GetPalyerEntry(player)?.UpdateReadyInfo();

				if (PhotonNetwork.IsMasterClient)
					CheckPlayerReadyState();
			}

			else if (changedProps.ContainsKey(PlayerProp.TEAMCOLOR))
			{
				Color teamColor;
				string hexColor = player.CustomProperties[PlayerProp.TEAMCOLOR].ToString();
				UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out teamColor);

				UpdateOtherPlayerTeam(player.ActorNumber, teamColor, player.CustomProperties[PlayerProp.TEAM].ToString());
			}

			else if (changedProps.ContainsKey(PlayerProp.CHARACTER))
			{
				UpdateOtherPlayerCharacter(player.ActorNumber, player.CustomProperties[PlayerProp.CHARACTER].ToString());
			}
		}

		public void RoomPropertiesUpdate(PhotonHashtable changedProps)
		{
			if (changedProps.ContainsKey(RoomProp.PLAYER_LIST))
				return;

			if (changedProps.ContainsKey(RoomProp.ROOM_MAP_ID))
			{
				int mapId = int.Parse(changedProps[RoomProp.ROOM_MAP_ID].ToString());
				var data = pickedMap.mapList.Maps.Where(x => x.Id == mapId).Select(x => x).FirstOrDefault();

				PhotonHashtable mapTitle = new PhotonHashtable();

				PhotonNetwork.CurrentRoom.SetRoomProperty(RoomProp.ROOM_MAP, data.Title);
				PhotonNetwork.CurrentRoom.SetRoomProperty(RoomProp.ROOM_MAP_GROUP, data.Group);

				if (data != null)
				{
					pickedMap.gameMap.OnChoosedMap?.Invoke(data);
				}
			}

			if(changedProps.ContainsKey(RoomProp.SLOT_NUMBER))
			{
				int number = int.Parse(changedProps[RoomProp.SLOT_NUMBER].ToString());
				SlotState state = (SlotState)int.Parse(changedProps[RoomProp.SLOT_STATE].ToString());

				UpdateOtherPlayerSlot(number, state);
			}

			roomInfo.SetChangedRoomInfo(changedProps);
		}

		private void UpdateOtherPlayerSlot(int number, SlotState state)
		{
			var changedSlot = playerContent.GetComponentsInChildren<WaitingPlayer>().Where(x => x.Slot.SlotNumber == number).FirstOrDefault();

			if(changedSlot != null)
			{
				changedSlot.Slot.SetSlot(state);
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

		private void UpdateOtherPlayerCharacter(int actorNumber, string characterKey)
		{
			CharacterEnum character = (CharacterEnum)Enum.Parse(typeof(CharacterEnum), characterKey);

			CharacterData data = GetCharacterData((CharacterEnum)character);
			if (data != null)
			{
				playerDictionary[actorNumber].PlayerSet.PlayerImg.sprite = data.Character;
			}
		}

		private void UpdateOtherPlayerTeam(int actorNumber, Color color, string teamName)
		{
			playerDictionary[actorNumber].PlayerSet.TeamColor.color = color;
			playerDictionary[actorNumber].PlayerSet.Team = teamName;
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
				int totalPlayer = playerDictionary.Count(x => x.Value.Slot.SlotCurState == SlotState.Use);
				int readyCount = PhotonNetwork.PlayerList.Count(x => x.GetReady());

				if (readyCount > 1)
				{
					isPassableStarting = (readyCount == totalPlayer);
				}
			}
		}

		private bool CheckPlayerTeamBalance()
		{
			RoomMode mode = (RoomMode)Enum.Parse(typeof(RoomMode), PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.ROOM_MODE].ToString());

			if(mode == RoomMode.Manner)
			{
				int totalPlayer = playerDictionary.Count(x => x.Value.Slot.SlotCurState == SlotState.Use);
				var teams = playerDictionary.Where(x => x.Value.Slot.SlotCurState == SlotState.Use)
											.GroupBy(x => x.Value.PlayerSet.Team)
											.Select(x => new
											{
												Team = x.Key,
												Count = x.Count()
											});

				int CntByTeam = teams.Count() / totalPlayer;

				if(teams.Any(x => x.Count != CntByTeam))
				{
					roomNotifyPopup.OnNotifyPopup("팀 구성이 맞지 않아 게임을 시작할 수 없습니다.");
					NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Critical, "팀 구성이 맞지 않아 게임을 시작할 수 없습니다."); 
					return false;
				}
			}
			return true;
		}

		public void SwitchedMasterPlayer(Player newMaster)
		{
			roomInfo.SetMasterRoomInfo();
			playerDictionary[newMaster.ActorNumber].WaitState.UpdateMasterInfo();
			playerDictionary[newMaster.ActorNumber].UpdateMasterInfo();
			gameStartController.BtnGameReady.SetReadyBtnImg();
			newMaster.SetReady(true);
			UpdateMasterPlayerState(newMaster.ActorNumber);
		}

		public void StartGame()
		{
			if (PhotonNetwork.IsMasterClient && isPassableStarting)
			{
				if(CheckPlayerTeamBalance() == false)
				{
					return;
				}

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