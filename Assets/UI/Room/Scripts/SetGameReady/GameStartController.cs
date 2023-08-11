using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using CustomProperty.Utils;
using CustomProperty;

namespace RoomUI.SetGameReady
{
	public class GameStartController : MonoBehaviourPun
	{
		public BtnReady BtnGameReady;

		[SerializeField]
		private Toggle autoReady;

		//public UnityAction<Player, bool> OnChangeReadyState;

		private void Awake()
		{
			BtnGameReady.ReadyBtn.onClick.AddListener(() => ClickedReadyButton());
			autoReady.onValueChanged.AddListener((isChecked) => CheckedAutoReady(isChecked));
		}

		private void CheckedAutoReady(bool isChecked)
		{
			if (isChecked)
			{
				if (PhotonNetwork.IsMasterClient == false)
					PhotonNetwork.LocalPlayer.SetPlayerProperty(PlayerProp.READY, true);
			}
		}

		private void ClickedReadyButton()
		{
			if (PhotonNetwork.IsMasterClient == false)
			{
				bool isReady = !PhotonNetwork.LocalPlayer.GetPlayerProperty(PlayerProp.READY, true);
				PhotonNetwork.LocalPlayer.SetPlayerProperty(PlayerProp.READY, isReady);
			}
		}
	}
}