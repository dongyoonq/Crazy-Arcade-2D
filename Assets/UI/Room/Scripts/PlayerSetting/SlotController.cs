using CustomProperty;
using CustomProperty.Utils;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace RoomUI.PlayerSetting
{
	public enum SlotState
	{
		Open,
		Use,
		Close
	}

	public class SlotController : MonoBehaviourPunCallbacks
	{
		[SerializeField]
		private Image OpenSlot;

		[SerializeField]
		private Image CloseSlot;

		[SerializeField]
		private Button BtnChangeSlot;

		public bool IsUsedSlot = false;

		public SlotState SlotCurState { get; set; } = SlotState.Open;

		public int SlotNumber;

		private void Awake()
		{
			AddCloseSlot();
		}

		/// <summary>
		/// ½½·Ô ´Ý±â ±â´É Á¦°Å
		/// </summary>
		public void RemoveCloseSlot()
		{
			//BtnChangeSlot.onClick.RemoveAllListeners();
		}

		/// <summary>
		/// ½½·Ô ´Ý±â ±â´É Ãß°¡
		/// </summary>
		public void AddCloseSlot()
		{
			BtnChangeSlot.onClick.AddListener(() => ChangedSlotState());
		}

		private void ChangedSlotState()
		{
			if (PhotonNetwork.IsMasterClient)
			{
				if (SlotCurState == SlotState.Use)
					return;

				Debug.Log("ChangedSlotState");

				byte state = (byte)PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.SLOT_STATE];
				
				byte[] stateBit = BitConverter.GetBytes(state);
				byte[] curSlotBit = BitConverter.GetBytes((byte)Math.Pow(2, SlotNumber));

				byte result;

				if (SlotCurState == SlotState.Open)
				{
					Debug.Log("OPEN");

					//close
					result = (byte)(stateBit[0] & ~curSlotBit[0]);
				}
				else
				{
					Debug.Log($"{Convert.ToString(stateBit[0], 2)} ^ {Convert.ToString(curSlotBit[0], 2)} = {Convert.ToString((stateBit[0] ^ curSlotBit[0]), 2)}");

					//open
					result = (byte)(stateBit[0] ^ curSlotBit[0]);
				}
				PhotonNetwork.CurrentRoom.SetRoomProperty(RoomProp.SLOT_STATE, result);
			}
		}

		public void clickedBtn()
		{

			Debug.Log("clickedBtn");
			ChangedSlotState();
		}

		public void SetSlot(SlotState state)
		{
			bool isOpen = state == SlotState.Open;
			OpenSlot.gameObject.SetActive(isOpen);
			CloseSlot.gameObject.SetActive(!isOpen);

			SlotCurState = state;
		}
	}
}