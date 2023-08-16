using CustomProperty;
using Photon.Pun;
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
			BtnChangeSlot.onClick.RemoveAllListeners();
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
			Debug.Log("ChangedSlotState");

			if (PhotonNetwork.IsMasterClient)
			{
				if (SlotCurState == SlotState.Use)
					return;

				SlotCurState = SlotCurState == SlotState.Open ? SlotState.Close : SlotState.Open;

				//OpenSlot.gameObject.SetActive(isSlotOpen);
				//CloseSlot.gameObject.SetActive(!isSlotOpen);

				PhotonHashtable property = new PhotonHashtable();
				property[RoomProp.SLOT_NUMBER] = SlotNumber;
				property[RoomProp.SLOT_STATE] = (int)SlotCurState;

				PhotonNetwork.CurrentRoom.SetCustomProperties(property);
			}
		}

		public void clickedBtn()
		{

			Debug.Log("clickedBtn");
			ChangedSlotState();
		}

		public void SetSlot(SlotState state)
		{
			Debug.Log($"SetSlot {state.ToString()}");

			bool isOpen = state == SlotState.Open;
			OpenSlot.gameObject.SetActive(isOpen);
			CloseSlot.gameObject.SetActive(!isOpen);
		}
	}
}