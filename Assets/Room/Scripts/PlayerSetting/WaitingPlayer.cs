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
		public SlotController Slot;

		[SerializeField]
		private PlayerWaitState playerWaitState;
		public PlayerWaitState WaitState { get { return playerWaitState; } }

		[SerializeField]
		private TMP_Text playerId;

		public Player player { get; private set; }

		public WaitPlayerSetting PlayerSet;

		public UnityAction<int, CharacterData> OnChangedOtherPlayerCharacter;
		public UnityAction<int, Color> OnChangedOtherPlayerTeam;

		public UnityAction<int, bool> OnChangedOtherPlayerState;
		public UnityAction<int> OnChangedMasterPlayerState;

		public void SetPlayer(Player player)
		{
			this.player = player;
			playerId.text = player.NickName;
			Slot.SlotCurState = SlotState.Use;

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
					playerWaitState.UpdateReadyInfo(player.GetPlayerProperty(PlayerProp.READY, false));

				if(player.CustomProperties.ContainsKey(PlayerProp.TEAMCOLOR))
				{
					string hexColor = player.CustomProperties[PlayerProp.TEAMCOLOR].ToString();

					Color teamColor;
					UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out teamColor);
					PlayerSet.TeamColor.color = teamColor;
				}
			}
			Slot.RemoveCloseSlot();
		}

		public void UpdateReadyInfo()
		{
			bool isReady = player.GetPlayerProperty(PlayerProp.READY, false); 

			if (player.IsMasterClient == false)
				playerWaitState.UpdateReadyInfo(isReady);
		}

		public void OnChangeCharacter(CharacterData data)
		{
			PlayerSet.PlayerImg.sprite = data.Character;
			player.SetPlayerProperty(PlayerProp.CHARACTER, data.CharacterEnum);

			//PhotonHashtable property = new PhotonHashtable();
			//property[PlayerProp.CHARACTER] = data.CharacterEnum;
			//player.SetCustomProperties(property);
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
				player.SetPlayerProperty(PlayerProp.READY, true);
			}
		}

		public void UpdateReadyInfo(bool isReady)
		{
			if (player.IsMasterClient == false)
			{
				playerWaitState.UpdateReadyInfo(isReady);
			}
		}
	}
}