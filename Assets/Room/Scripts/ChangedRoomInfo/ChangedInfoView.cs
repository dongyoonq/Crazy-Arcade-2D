using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoomUI.ChangedRoomInfo
{
	public class ChangedInfoView : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField roomName;

		[SerializeField]
		private ToggleGroup roomMode;

		[SerializeField]
		private ChangedPassword roomPassword;

		[SerializeField]
		private Button btnOK;

		[SerializeField]
		private Button btnCancel;

		private RoomData roomData;

		public UnityAction<RoomData> OnClosedView;

		private void Awake()
		{
			btnOK.onClick.AddListener(() => CloseView());
			btnCancel.onClick.AddListener(() => CancelView());

			Debug.Log("AWAKE");
		}

		public void SetExistingInfo(RoomData data)
		{
			roomData = data;

			roomName.text = data.Name;
			roomPassword.SetRoomPassword(data.IsPrivateRoom, data.Password);
		}

		private void CancelView()
		{
			DisableView(null);
		}

		private void CloseView()
		{
			roomData.Name = roomName.text;
			roomData.Mode = GetActiveRoomMode();
			roomData.IsPrivateRoom = roomPassword.IsSetPrivate();
			roomData.Password = roomData.IsPrivateRoom ? roomPassword.GetRoomPassword() : "";

			DisableView(roomData);
		}

		private RoomMode GetActiveRoomMode()
		{
			var activeToggles = roomMode.ActiveToggles().ToArray();

			if (activeToggles.Length > 0)
			{
				Toggle selectedToggle = activeToggles[0];

				switch (selectedToggle.name)
				{
					case "ChkManner": return RoomMode.Manner;
					case "ChkFree":   return RoomMode.Free;
					case "ChkRandom": return RoomMode.Random;
					default: return roomData.Mode;
				}
			}
			else
				return roomData.Mode;
		}

		private void DisableView(RoomData data)
		{
			gameObject.SetActive(false);
			OnClosedView?.Invoke(data);
		}
	}
}