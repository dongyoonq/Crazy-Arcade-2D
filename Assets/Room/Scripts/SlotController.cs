using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoomUI
{
	public class SlotController : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		private Image OpenSlot;

		[SerializeField]
		private Image CloseSlot;

		[SerializeField]
		private Button BtnChangeSlot;

		private bool isSlotOpen;
		public bool IsSlotOpen { get { return IsSlotOpen; } }

		private void Awake()
		{
			BtnChangeSlot.onClick.AddListener(ChangedSlotState);
		}

		/// <summary>
		/// ½½·Ô ´Ý±â ±â´É Á¦°Å
		/// </summary>
		public void RemoveCloseSlot()
		{
			BtnChangeSlot.onClick.RemoveAllListeners();
		}

		/// <summary>
		/// ½½·Ô ´Ý±â ±â´É Ãß°¡
		/// </summary>
		public void AddCloseSlot()
		{
			BtnChangeSlot.onClick.AddListener(ChangedSlotState);
		}

		private void ChangedSlotState()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				isSlotOpen = !isSlotOpen;

				OpenSlot.gameObject.SetActive(isSlotOpen);
				CloseSlot.gameObject.SetActive(!isSlotOpen);
			}
		}
	}
}