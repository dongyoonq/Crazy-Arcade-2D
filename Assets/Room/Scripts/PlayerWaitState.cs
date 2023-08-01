using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWaitState : MonoBehaviour
{
	[SerializeField]
	private Image StateDisable;

	[SerializeField]
	private Image StateEnable;

	[SerializeField]
	private Image StateMaster;

	public void SetPlayerState(bool isMaster)
	{
		StateMaster.gameObject.SetActive(isMaster);
		StateDisable.gameObject.SetActive(!isMaster);
		StateEnable.gameObject.SetActive(!isMaster);
	}

	/// <summary>
	/// 게임 준비 활성 여부 업데이트
	/// </summary>
	public void UpdateReadyInfo(bool isReady)
	{
		StateEnable.gameObject.SetActive(isReady);
		StateDisable.gameObject.SetActive(!isReady);
	}
}
