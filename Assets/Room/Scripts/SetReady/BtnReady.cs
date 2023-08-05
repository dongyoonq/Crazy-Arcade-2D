using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI
{
	public class BtnReady : MonoBehaviourPun
	{
		[SerializeField]
		private Sprite Ready_Master;

		[SerializeField]
		private Sprite Ready_Player;

		public Button ReadyBtn;

		private void Awake()
		{
			SetReadyBtnImg();
		}

		public void SetReadyBtnImg()
		{
			ReadyBtn.image.sprite = PhotonNetwork.IsMasterClient ? Ready_Master : Ready_Player;
		}
	}


}
