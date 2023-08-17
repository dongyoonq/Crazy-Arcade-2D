using CustomProperty;
using CustomProperty.Utils;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Extension;

namespace RoomUI.ChangedRoomInfo
{
	public class ChangedInfoView : MonoBehaviourPun
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

		public UnityAction OnClosedView;

		private void Awake()
		{
			btnOK.onClick.AddListener(() => CloseView());
			btnCancel.onClick.AddListener(() => CancelView());
		}

		public void SetExistingInfo()
		{
			roomName.text = PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.ROOM_NAME].ToString();

			RoomMode mode = (RoomMode)Enum.Parse(typeof(RoomMode), PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.ROOM_MODE].ToString());
			SetActiveRoomMode(mode);

			string pwd = PhotonNetwork.CurrentRoom.CustomProperties[RoomProp.ROOM_PASSWORD].ToString();
			roomPassword.SetRoomPassword(pwd);
		}

		private void CancelView()
		{
            GameManager.Sound.Onclick();
            gameObject.SetActive(false);
		}

		private void CloseView()
		{
            GameManager.Sound.Onclick();

            if (roomName.text.Trim() == "")
			{
				//TODO. error popup 
				Debug.Log("방 제목은 공백이 될 수 없습니다");
			}
			else
				PhotonNetwork.CurrentRoom.SetRoomProperty(RoomProp.ROOM_NAME, roomName.text);

			PhotonNetwork.CurrentRoom.SetRoomProperty(RoomProp.ROOM_MODE, GetActiveRoomMode());


			string pwd = roomPassword.IsSetPrivate() ? roomPassword.GetRoomPassword() : "";
			PhotonNetwork.CurrentRoom.SetRoomProperty(RoomProp.ROOM_PASSWORD, pwd);

			gameObject.SetActive(false);

			OnClosedView?.Invoke();
		}

		private RoomMode GetActiveRoomMode()
		{
			var activeToggles = roomMode.ActiveToggles().ToArray();

			if (activeToggles.Length > 0)
			{
				return activeToggles[0].transform.GetComponent<RoomModeInfo>().Mode;
			}
			else
				return RoomMode.Manner;
		}

		private void SetActiveRoomMode(RoomMode mode)
		{
			var toggles = roomMode.GetComponentsInChildren<RoomModeInfo>();

			var activeTog = toggles.Where(x => x.Mode == mode).FirstOrDefault()?.GetComponent<Toggle>();
			if (activeTog != null)
				activeTog.isOn = true;
		}
	}
}