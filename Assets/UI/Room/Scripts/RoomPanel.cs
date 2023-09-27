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
using Unity.VisualScripting;

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
		private ExplainPlayer explainPlayer;

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

			if ((bool)PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.ROOM_PLAYING] == true)
			{
				PhotonHashtable property = new PhotonHashtable();
				property[RoomProp.ROOM_PLAYING] = false;
				PhotonNetwork.CurrentRoom.SetCustomProperties(property);
			}
		}

		private void OnDisable()
		{
			foreach (int actorNumber in playerDictionary.Keys)
			{
				 playerDictionary[actorNumber].InitWaitState();
			}
			InitRoom();

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
			InstantiatePlayer(player,  playerContent.GetComponentsInChildren<WaitingPlayer>());
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
			if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(RoomProp.SLOT_STATE))
			{
				UpdateOtherPlayerSlot((byte)PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.SLOT_STATE]);
			}

			var contents = playerContent.GetComponentsInChildren<WaitingPlayer>();
			foreach (Player player in PhotonNetwork.PlayerList)
			{
				InstantiatePlayer(player, contents);
			}
		}

		private void InstantiatePlayer(Player player, WaitingPlayer[] contents)
		{
			int slotNum = -1;
			WaitingPlayer waitingPlayer = null;
			if (player.CustomProperties.ContainsKey(PlayerProp.SLOT_NUMBER))
				slotNum = (int)player.CustomProperties[PlayerProp.SLOT_NUMBER];

			if (slotNum >= 0)
				waitingPlayer = contents.Where(x => x.SlotNumber == slotNum).FirstOrDefault();
			else
			{
				waitingPlayer = contents.Where(x => x.CurrentSlotState == SlotState.Open).FirstOrDefault();
				player.SetPlayerProperty(PlayerProp.SLOT_NUMBER, waitingPlayer.SlotNumber);
			}
		
			if (waitingPlayer == null)
				return;

			waitingPlayer.SetPlayer(player);
			playerDictionary.Add(player.ActorNumber, waitingPlayer);

			//if (player.IsLocal)
			//{
			//	if (PhotonNetwork.CurrentRoom.GetRoomProperty(RoomProp.ROOM_PLAYING, true) == false)
			//	{
			//		pickedTeam.InitTeam(player);
			//		InitCharacter(player);
			//	}
			//}

			if (player.CustomProperties.ContainsKey(PlayerProp.CHARACTER))
			{
				CharacterEnum character = (CharacterEnum)Enum.Parse(typeof(CharacterEnum), player.CustomProperties[PlayerProp.CHARACTER].ToString());
				Debug.Log($"[InstantiatePlayer] {character.ToString()} / {player.CustomProperties[PlayerProp.CHARACTER].ToString()}");

				UpdateOtherPlayerCharacter(waitingPlayer, player.CustomProperties[PlayerProp.CHARACTER].ToString());
			}
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
				CharacterEnum character = (CharacterEnum)Enum.Parse(typeof(CharacterEnum), player.CustomProperties[PlayerProp.CHARACTER].ToString());
				Debug.Log($"[PlayerPropertiesUpdate] {character.ToString()}");

				UpdateOtherPlayerCharacter(updatedPlayer, player.CustomProperties[PlayerProp.CHARACTER].ToString());
			}
		}

		private void InitRoom()
		{
			pickedTeam.InitTeam();
			characterSetter.InitCharacterSetter();
			explainPlayer.InitExplainPlayer();
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
				UpdateOtherPlayerSlot((byte)changedProps[RoomProp.SLOT_STATE]);
			}

			roomInfo.SetChangedRoomInfo(changedProps);
		}

		private void UpdateOtherPlayerSlot(byte state)
		{
			var changedSlots = playerContent.GetComponentsInChildren<WaitingPlayer>();

			string slotState = Convert.ToString(state, 2).PadLeft(8, '0');

			char[] states = slotState.ToCharArray();
			Array.Reverse(states);

			int index = -1;
			if (changedSlots != null)
			{
				foreach(var slot in changedSlots) 
				{
					++index;

					if (slot.CurrentSlotState == SlotState.Use)
						continue;

					if(states[index] == '0' && slot.CurrentSlotState != SlotState.Close) 
					{
						slot.UpdateSlotState(SlotState.Close);
					}
					else if (states[index] == '1' && slot.CurrentSlotState != SlotState.Open) 
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
			Debug.Log($"[UpdateOtherPlayerCharacter] {player.name} / {characterKey}");

			CharacterEnum character = (CharacterEnum)Enum.Parse(typeof(CharacterEnum), characterKey);

			CharacterData data = GetCharacterData(character);
			if (data != null)
			{
				Debug.Log($"[UpdateOtherPlayerCharacter] data != null : {data}");

				player.PlayerSet.CharData = data;

				Debug.Log($"[UpdateOtherPlayerCharacter] player.PlayerSet.CharData  : {player.PlayerSet.CharData}");
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

			Debug.Log($"[CheckPlayerTeamBalance] {mode}");

			if(mode == RoomMode.Manner)
			{
				var userList = playerDictionary.Where(x => x.Value.CurrentSlotState == SlotState.Use);


				int totalPlayer = userList.Count();

				var teams = userList.GroupBy(x => x.Value.PlayerSet.Team)
									.Select(x => new
									{
										Team = x.Key,
										Count = x.Count()
									}).ToList();

				if (teams == null || teams.Count() == 0)
					return false;

				if (totalPlayer == 2)
				{
					if (teams.Count() < 2)
					{
						NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Critical, "팀 구성이 맞지 않아 게임을 시작할 수 없습니다.");
						return false;
					}
				}
				else
				{
					int CntByTeam = totalPlayer / teams.Count();
					Debug.Log($"[CheckPlayerTeamBalance] totalPlayer : {totalPlayer} / teams.Count() {teams.Count()} = {CntByTeam}");

					if (teams.Any(x => x.Count != CntByTeam))
					{
						NotifyChat.OnNotifyChat?.Invoke(NotifyChatType.Critical, "팀 구성이 맞지 않아 게임을 시작할 수 없습니다.");
						return false;
					}
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
			PhotonHashtable property = new PhotonHashtable();
			property[PlayerProp.CHARACTER] = CharacterEnum.Dao;
			PhotonNetwork.LocalPlayer.SetCustomProperties(property);
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