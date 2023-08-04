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

public class WaitingPlayer : MonoBehaviourPun, IChangeableCharacter
{
	[SerializeField]
	private SlotController slotController;

	[SerializeField]
	private PlayerWaitState playerWaitState;
	public PlayerWaitState WaitState { get { return playerWaitState; } }

	public Image playerImg;

	[SerializeField]
	private TMP_Text playerId;

	public Player player { get; private set; }

	public UnityAction<int, CharacterData> OnChangedOtherPlayerCharacter;
	public UnityAction<int, bool> OnChangedOtherPlayerState;
	public UnityAction<int> OnChangedMasterPlayerState;

	private bool IsEmptySlot;

	private void Awake()
	{
		
	}

	public void SetPlayer(Player player)
	{
		this.player = player;
		
		playerId.text = player.NickName;
		playerImg.gameObject.SetActive(true);

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
		}

		slotController.RemoveCloseSlot();
	}

	/// <summary>
	/// 게임 준비 활성 여부 업데이트
	/// </summary>
	public void UpdateReadyInfo()
	{
		if (player.IsMasterClient == false) 
			playerWaitState.UpdateReadyInfo(player.GetReady());
	}

	public void UpdateMasterInfo()
	{
		if (player.IsMasterClient)
		{
			player.SetReady(true);
			photonView.RPC("ChangedMasterPlayerState", RpcTarget.Others, player.ActorNumber);
		}
	}

	/// <summary>
	/// 게임 준비 활성 여부 업데이트
	/// </summary>
	public void UpdateReadyInfo(bool isReady)
	{
		if (player.IsMasterClient == false)
		{
			playerWaitState.UpdateReadyInfo(isReady);
			photonView.RPC("ChangedOtherPlayerState", RpcTarget.Others, player.ActorNumber, isReady);
		}
	}

	public void OnChangeCharacter(CharacterData data)
	{ 
		playerImg.sprite = data.Character;
		photonView.RPC("ChangedOtherCharacter", RpcTarget.Others, player.ActorNumber, data.Name);
	}

	[PunRPC]
	private void ChangedOtherCharacter(int actorNum, string dataName)
	{
		CharacterData data  = Resources.Load<CharacterData>($"CharacterData/{dataName}");
		OnChangedOtherPlayerCharacter?.Invoke(actorNum, data);
	}

	[PunRPC]
	private void ChangedOtherPlayerState(int actorNum, bool isReady)
	{
		OnChangedOtherPlayerState?.Invoke(actorNum, isReady);
	}

	[PunRPC]
	private void ChangedMasterPlayerState(int actorNum)
	{
		OnChangedMasterPlayerState?.Invoke(actorNum);
	}
}
