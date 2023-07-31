using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WaitingPlayer : MonoBehaviour, IChangeableCharacter
{
	[SerializeField]
	private Image WaitingView;

	[SerializeField]
	private Image CloseView;
	
	[SerializeField] 
	private Image StateDisable;

	[SerializeField]
	private Image StateEnable;

	[SerializeField] 
	private Image StateMaster;

	[SerializeField]
	private Image PlayerImg;

	[SerializeField]
	private TMP_Text txtPlayerId;

	public Player player { get; private set; }

	private void Awake()
	{
		
	}

	public void SetPlayer(Player player)
	{
		this.player = player;

		txtPlayerId.text = player.NickName;

		bool isMaster = player.IsMasterClient;

		StateMaster.gameObject.SetActive(isMaster);
		StateDisable.gameObject.SetActive(!isMaster);
		StateEnable.gameObject.SetActive(!isMaster);

		if(player.IsLocal)
		{
			CharacterChanger.OnChangedCharacter += OnChangeCharacter;
		}
	}

	/// <summary>
	/// 게임 준비 활성 여부 업데이트
	/// </summary>
	public void UpdateReadyInfo()
	{
		if(player.IsMasterClient == false)
		{
			bool isReady = player.GetReady();

			StateEnable.gameObject.SetActive(isReady);
			StateDisable.gameObject.SetActive(!isReady);
		}
	}

	public void OnChangeCharacter(CharacterData data)
	{
		PlayerImg.sprite = data.Character;
	}
}
