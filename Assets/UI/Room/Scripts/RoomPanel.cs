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
		private PlayerCharacterSetter characterSetter;

		[SerializeField]
		private PickedTeam pickedTeam;

		[SerializeField]
		private PickedMap pickedMap;

		[SerializeField]
		private GameStartController gameStartController;

		[SerializeField]
		private RoomNotify roomNotifyPopup;
		
		private Dictionary<int, WaitingPlayer> playerDictionary;

		private bool isPassableStarting;

		private void Awake()
		{
			playerDictionary = new Dictionary<int, WaitingPlayer>();

			gameStartController.BtnGameReady.ReadyBtn.onClick.AddListener(() => StartGame());

			int num = 0;
			var slots = playerContent.GetComponentsInChildren<WaitingPlayer>();
			foreach (var waitSlot in slots)
				waitSlot.SlotNumber = num++;
		}

		private void OnEnable()
		{
			SetInPlayer();
			CheckPlayerReadyState();

			NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Warning, $"{PhotonNetwork.NickName}님이 참가하셨습니다.");		
		}

		private void OnDisable()
		{
			foreach (int actorNumber in playerDictionary.Keys)
			{
				 playerDictionary[actorNumber].InitWaitState();
			}
			playerDictionary.Clear();
			pickedMap.ResetChooseID();
		}

		private bool CheckPlayingRoom()
		{
			PhotonHashtable property = PhotonNetwork.CurrentRoom.CustomProperties;

			if (property.ContainsKey(RoomProp.ROOM_PLAYING) && (bool)property[RoomProp.ROOM_PLAYING])
			{
				return true;
            }

			return false;
		}


		public void EntryPlayer(Player player)
		{
			InstantiatePlayer(player);
			CheckPlayerReadyState();
		}

		public void LeavePlayer(Player leavePlayer)
		{
			playerDictionary[leavePlayer.ActorNumber].InitWaitState();
			playerDictionary.Remove(leavePlayer.ActorNumber);

			//int maxID = playerContent.GetComponentsInChildren<WaitingPlayer>().Max(x => x.SlotNumber);
			//WaitingPlayer newslot = Instantiate<WaitingPlayer>(Resources.Load<WaitingPlayer>("WaitingPlayer"), playerContent);
			//newslot.SlotNumber = maxID + 1;

			CheckPlayerReadyState();

			if(PhotonNetwork.IsMasterClient)
				NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Warning, $"{leavePlayer.NickName}님이 퇴장하셨습니다,");

		}

		private void AddPlayer()
		{
			int max = 8 - playerContent.childCount;

			for(int i=max; i>=0; i--)
			{
				Instantiate<WaitingPlayer>(Resources.Load<WaitingPlayer>("WaitingPlayer"), playerContent);
			}	
		}

		private void SetInPlayer()
		{
			var contents = playerContent.GetComponentInChildren<WaitingPlayer>();
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
				pickedTeam.InitTeam(player);
				InitCharacter(player);
			}
			PlayerPropertiesUpdate(player, player.CustomProperties);
		}

        public void PlayerPropertiesUpdate(Player player, PhotonHashtable changedProps)
		{
			WaitingPlayer updatedPlayer = playerDictionary.ContainsKey(player.ActorNumber) ? playerDictionary[player.ActorNumber] : GetPalyerEntry(player);

			if (updatedPlayer == null) 
				return;

			if (changedProps.ContainsKey(PlayerProp.READY))
			{
				updatedPlayer.UpdateReadyInfo();

				if (PhotonNetwork.IsMasterClient)
					CheckPlayerReadyState();
			}

			if (changedProps.ContainsKey(PlayerProp.TEAMCOLOR))
			{
				Color teamColor;
				string hexColor = player.CustomProperties[PlayerProp.TEAMCOLOR].ToString();
				UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out teamColor);

				UpdateOtherPlayerTeam(updatedPlayer, teamColor, player.CustomProperties[PlayerProp.TEAM].ToString());
			}

			if (changedProps.ContainsKey(PlayerProp.CHARACTER))
			{
				UpdateOtherPlayerCharacter(updatedPlayer, player.CustomProperties[PlayerProp.CHARACTER].ToString());
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

				PhotonHashtable property = new PhotonHashtable();

				property[RoomProp.ROOM_MAP] = data.Title;
				property[RoomProp.ROOM_MAP_GROUP] = data.Group;
				property[RoomProp.ROOM_MAP_FILE] = data.MapFileName;
				PhotonNetwork.CurrentRoom.SetCustomProperties(property);

				if (data != null)
				{
					pickedMap.gameMap.OnChoosedMap?.Invoke(data);
				}
			}

			if(changedProps.ContainsKey(RoomProp.SLOT_STATE))
			{
				UpdateOtherPlayerSlot(changedProps[RoomProp.SLOT_STATE].ToString());
			}

			roomInfo.SetChangedRoomInfo(changedProps);
		}

		private void UpdateOtherPlayerSlot(string state)
		{
			var changedSlots = playerContent.GetComponentsInChildren<WaitingPlayer>();

			char[] states = state.ToCharArray();

			int index = -1;
			if (changedSlots != null)
			{
				foreach(var slot in changedSlots) 
				{
					++index;

					if (slot.CurrentSlotState == SlotState.Use)
						continue;

					if(states[index++] == '0' && slot.CurrentSlotState != SlotState.Close) 
					{
						slot.UpdateSlotState(SlotState.Close);
					}
					else if (states[index++] == '1' && slot.CurrentSlotState != SlotState.Open) 
					{
						slot.UpdateSlotState(SlotState.Open);
					}
				}

				PhotonHashtable property = new PhotonHashtable();
				property[RoomProp.ROOM_MAX] = playerContent.GetComponentsInChildren<WaitingPlayer>().Count(x => x.CurrentSlotState != SlotState.Close);
				PhotonNetwork.CurrentRoom.SetCustomProperties(property);
			}
		}

		private CharacterData GetCharacterData(CharacterEnum characterEnum)
		{
			return characterSetter.CharacterDatas?.Where(x => x.Name == characterEnum.ToString()).FirstOrDefault();
		}

		public void UpdatePlayerState(WaitingPlayer player, bool isReady)
		{
			player?.UpdateReadyInfo();

			if (PhotonNetwork.IsMasterClient)
				CheckPlayerReadyState();
		}

		private void UpdateOtherPlayerCharacter(WaitingPlayer player, string characterKey)
		{
			CharacterEnum character = (CharacterEnum)Enum.Parse(typeof(CharacterEnum), characterKey);

			CharacterData data = GetCharacterData((CharacterEnum)character);
			if (data != null)
			{
				player.PlayerSet.PlayerImg.sprite = data.Character;
			}
		}

		private void UpdateOtherPlayerTeam(WaitingPlayer player, Color color, string teamName)
		{
			player.PlayerSet.TeamColor.color = color;
			player.PlayerSet.Team = teamName;
		}

		private void UpdateOtherPlayerState(WaitingPlayer player, bool isReady)
		{
			player.WaitState.UpdateReadyInfo(isReady);
		}

		private void UpdateMasterPlayerState(WaitingPlayer player)
		{
			player.WaitState.UpdateMasterInfo();
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
				int totalPlayer = playerDictionary.Count(x => x.Value.CurrentSlotState == SlotState.Use);
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
				var userList = playerDictionary.Where(x => x.Value.CurrentSlotState == SlotState.Use);

				int totalPlayer = userList.Count();
				var teams = userList.GroupBy(x => x.Value.PlayerSet.Team)
									.Select(x => new
									{
										Team = x.Key,
										Count = x.Count()
									});

				int CntByTeam = teams.Count() / totalPlayer;

				if(teams.Any(x => x.Count != CntByTeam))
				{
					//roomNotifyPopup.OnNotifyPopup("팀 구성이 맞지 않아 게임을 시작할 수 없습니다.");
					NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Critical, "팀 구성이 맞지 않아 게임을 시작할 수 없습니다."); 
					return false;
				}
			}
			return true;
		}

		public void SwitchedMasterPlayer(Player newMaster)
		{
			roomInfo.SetMasterRoomInfo();
			playerDictionary[newMaster.ActorNumber].UpdateMasterInfo();
			gameStartController.BtnGameReady.SetReadyBtnImg();
			UpdateMasterPlayerState(playerDictionary[newMaster.ActorNumber]);
			pickedMap.AddOpenChooseMap();
		}

		public void StartGame()
		{
			if (PhotonNetwork.IsMasterClient && !isPassableStarting)
			{
                NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Critical, "모든 플레이어가 레디하지 않았습니다.");
            }

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

		private void InitCharacter(Player player)
		{
			if(player.CustomProperties.ContainsKey(PlayerProp.CHARACTER) ==false)
			{
				PhotonHashtable property = new PhotonHashtable();
				property[PlayerProp.CHARACTER] = CharacterEnum.Dao;
				PhotonNetwork.LocalPlayer.SetCustomProperties(property);
			}
        }

		private void LoadMapScene()
		{
			PhotonHashtable property = new PhotonHashtable();
			property[RoomProp.ROOM_PLAYING] = true;
			PhotonNetwork.CurrentRoom.SetCustomProperties(property);

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