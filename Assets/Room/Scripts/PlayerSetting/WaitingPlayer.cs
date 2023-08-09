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
		private SlotController slotController;

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

		private bool IsEmptySlot;

		public void SetPlayer(Player player)
		{
			this.player = player;
			playerId.text = player.NickName;

			PlayerSet.gameObject.SetActive(true);

			Debug.Log($"{player.NickName} : {player.IsMasterClient} / {player.IsLocal}");

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
					playerWaitState.UpdateReadyInfo(player.GetReady());

				if(player.CustomProperties.ContainsKey(PlayerProp.TEAM))
				{
					string hexColor = player.CustomProperties[PlayerProp.TEAM].ToString();

					Color teamColor;
					UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out teamColor);
					PlayerSet.TeamColor.color = teamColor;
				}
			}
			slotController.RemoveCloseSlot();
		}

		public void UpdateReadyInfo()
		{
			bool isReady = player.GetReady();

			if (player.IsMasterClient == false)
				playerWaitState.UpdateReadyInfo(isReady);
		}

		public void OnChangeCharacter(CharacterData data)
		{
			PlayerSet.PlayerImg.sprite = data.Character;

			PhotonHashtable property = new PhotonHashtable();
			property[PlayerProp.CHARACTER] = data.Name;
			player.SetCustomProperties(property);
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
				player.SetReady(true);
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