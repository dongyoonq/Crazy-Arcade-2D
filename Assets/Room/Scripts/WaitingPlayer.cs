using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WaitingPlayer : MonoBehaviourPun, IChangeableCharacter
{
	[SerializeField]
	private SlotController slotController;

	[SerializeField]
	private PlayerWaitState playerWaitState;

	public Image playerImg;

	[SerializeField]
	private TMP_Text playerId;

	public Player player { get; private set; }

	public UnityAction<int, CharacterData> OnChangedOtherPlayerCharacter;

	private bool IsEmptySlot;

	private void Awake()
	{
		
	}

	public void SetPlayer(Player player)
	{
		this.player = player;
		
		playerWaitState.SetPlayerState(player.IsMasterClient);

		playerId.text = player.NickName;
		playerImg.gameObject.SetActive(true);

		if (player.IsLocal)
			CharacterChanger.OnChangedCharacter += OnChangeCharacter;

 		slotController.SetButtonAction();
	}

	/// <summary>
	/// 게임 준비 활성 여부 업데이트
	/// </summary>
	public void UpdateReadyInfo()
	{
		if(player.IsMasterClient == false) 
			playerWaitState.UpdateReadyInfo(player.GetReady());
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
}
