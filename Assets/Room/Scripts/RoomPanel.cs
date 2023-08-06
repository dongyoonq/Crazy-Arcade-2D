using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using RoomUI.Utils;
using RoomUI.ChangedRoomInfo;

namespace RoomUI
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
			CheckPlayerReadyState();
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

		/// <summary>
		/// �̹� ������ �ִ� ����� + ���� ����
		/// </summary>
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

		/// <summary>
		/// ��� �÷��̾ ready ���°� �Ǹ� start button Ȱ�� ó��
		/// </summary>
		public void CheckPlayerReadyState()
		{
			isPassableStarting = false;

			if (PhotonNetwork.IsMasterClient) //������ �ƴϸ� ���� Ȯ���� �ʿ�� ����. 
			{
				int readyCount = PhotonNetwork.PlayerList.Count(x => x.GetReady());

				if (readyCount > 1) //���� ȥ�� ���� ���� ���� ���� ���ϰ� ����
				{
					isPassableStarting = (readyCount == PhotonNetwork.PlayerList.Length);
				}
			}
		}

		public void SwitchedMasterPlayer(Player newMaster)
		{
			roomInfo.SetMasterRoomInfo();
			playerDictionary[newMaster.ActorNumber].UpdateMasterInfo();
			gameStartController.BtnGameReady.SetReadyBtnImg();
			CheckPlayerReadyState();
		}

		/// <summary>
		/// ���� ������ �̵�
		/// </summary>
		public void StartGame()
		{
			if (isPassableStarting)
			{
				PhotonNetwork.CurrentRoom.IsOpen = false;
				PhotonNetwork.CurrentRoom.IsVisible = false;

				//PhotonNetwork.LoadLevel("GameScene");
				Debug.Log("���ӽ���!!");
			}
		}

		/// <summary>
		/// �� ������
		/// </summary>
		public void LeaveRoom()
		{
			PhotonNetwork.LeaveRoom(); //���� ��Ʈ��ũ�� �� �����ٰ� ��û�ϱ�   
		}
	}
}