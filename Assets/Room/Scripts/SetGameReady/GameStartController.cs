using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using RoomUI.Utils;

namespace RoomUI.SetGameReady
{
	public class GameStartController : MonoBehaviourPun
	{
		public BtnReady BtnGameReady;

		[SerializeField]
		private Toggle autoReady;

		public UnityAction<Player, bool> OnChangeReadyState;

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
					UpdatePlayerState(true);
			}
		}

		private void ClickedReadyButton()
		{
			if (PhotonNetwork.IsMasterClient == false)
			{
				bool isReady = !PhotonNetwork.LocalPlayer.GetReady();

				PhotonNetwork.LocalPlayer.SetReady(isReady);
				UpdatePlayerState(isReady);
			}

		}

		private void UpdatePlayerState(bool isReady)
		{
			OnChangeReadyState?.Invoke(PhotonNetwork.LocalPlayer, isReady);
		}
	}
}