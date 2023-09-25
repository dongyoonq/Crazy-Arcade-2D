using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CustomProperty.Utils;
using RoomUI.ChooseTeam;
using CustomProperty;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using static Extension;
using System;

namespace RoomUI.PlayerSetting
{
	public class WaitingPlayer : MonoBehaviourPun, IChangeableCharacter
	{
		[SerializeField]
		private SlotController slot;
		public int SlotNumber
		{ 
			get { return slot == null ? -1 : slot.SlotNumber; } 
			set { if (slot != null) slot.SlotNumber = value; } 
		}
		public SlotState CurrentSlotState { get { return slot == null ? SlotState.Open : slot.SlotCurState; } }

		[SerializeField]
		private PlayerWaitState playerWaitState;
		public PlayerWaitState WaitState { get { return playerWaitState; } }

		[SerializeField]
		private TMP_Text playerId;

		public Player player { get; private set; }

		public WaitPlayerSetting PlayerSet;

		public void SetPlayer(Player player)
		{
			this.player = player;
			playerId.text = player.NickName;
			slot.SlotCurState = SlotState.Use;

			PlayerSet.gameObject.SetActive(true);

			if (player.IsLocal)
			{
				CharacterChanger.OnChangedCharacter += OnChangeCharacter;
				playerWaitState.SetPlayerState(player.IsMasterClient);
			}
			else
			{
				if (player.IsMasterClient)
					playerWaitState.UpdateMasterInfo();
				else
				{
					playerWaitState.UpdateReadyInfo(player.GetPlayerProperty(PlayerProp.READY, false));
					slot.RemoveCloseSlot();
				}

				if (player.CustomProperties.ContainsKey(PlayerProp.TEAMCOLOR))
				{
					string hexColor = player.CustomProperties[PlayerProp.TEAMCOLOR].ToString();

					Color teamColor;
					UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out teamColor);
					PlayerSet.TeamColor.color = teamColor;
				}
			}
			
		}

		public void UpdateReadyInfo()
		{
			bool isReady = player.GetPlayerProperty(PlayerProp.READY, false); 

			if (player.IsMasterClient == false)
				playerWaitState.UpdateReadyInfo(isReady);
		}

		public void OnChangeCharacter(CharacterData data)
		{
			PlayerSet.CharData = data;
			player.SetPlayerProperty(PlayerProp.CHARACTER, data.CharacterEnum);
		}

		public void OnFocusOnCharacter(CharacterData data)
		{
		}

		public void OnFocusOffCharacter()
		{
		}

		/*
		public void OnChangeTeam(Color color)
		{
			PlayerSet.TeamColor.color = color;
		}
		*/

		public void UpdateMasterInfo()
		{
			if (player.IsMasterClient)
			{
				playerWaitState.UpdateMasterInfo();
				player.SetPlayerProperty(PlayerProp.READY, true);
				slot.AddCloseSlot();
			}
		}

		public void UpdateReadyInfo(bool isReady)
		{
			if (player.IsMasterClient == false)
			{
				playerWaitState.UpdateReadyInfo(isReady);
			}
		}

		public void UpdateSlotState(SlotState state)
		{
			slot.SetSlot(state);
		}

		public void InitWaitState()
		{
			PlayerSet.TeamColor.color = new Color(1f, 1f, 1f, 0f);
			PlayerSet.Team = "";
			PlayerSet.gameObject.SetActive(false);
			playerId.text = "";
			slot.SlotCurState = SlotState.Open;

			playerWaitState.UpdateReadyInfo(false);

			CharacterChanger.OnChangedCharacter -= OnChangeCharacter;
		}
	}
}