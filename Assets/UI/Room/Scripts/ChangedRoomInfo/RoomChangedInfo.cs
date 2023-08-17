using CustomProperty;
using CustomProperty.Utils;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace RoomUI.ChangedRoomInfo
{
	public class RoomChangedInfo : MonoBehaviourPun
	{
		[SerializeField]
		private TMP_Text txtRoomNumber;

		[SerializeField]
		private TMP_Text txtRoomName;

		[SerializeField]
		private Image imgRoomMode;

		[SerializeField]
		private List<Sprite> roomModeIcons;

		[SerializeField]
		private Image imgRoomPassword;

		[SerializeField]
		private Button btnChangedRoomInfo;

		[SerializeField]
		private ChangedInfoView viewChangedRoomInfo;

		//private RoomData roomData;

		private bool isActiveChangedView;

		private void Awake()
		{
			if (PhotonNetwork.IsMasterClient)
				btnChangedRoomInfo.onClick.AddListener(() => OpenChangedRoomInfoUI());
			SetActiveChanged(PhotonNetwork.IsMasterClient);

			viewChangedRoomInfo.OnClosedView += CloseChangedInfoView;
			isActiveChangedView = false;
		}

		private void OnEnable()
		{
			InitRoomInfo();
		}

		public void SetMasterRoomInfo()
		{
			SetActiveChanged(true);
			btnChangedRoomInfo.onClick.AddListener(() => OpenChangedRoomInfoUI());
		}

		private void OpenChangedRoomInfoUI()
		{
            GameManager.Sound.Onclick();

            if (isActiveChangedView == false)
			{
				viewChangedRoomInfo.SetExistingInfo();
				viewChangedRoomInfo.gameObject.SetActive(true);
				isActiveChangedView = true;
			}
		}

		private void InitRoomInfo()
		{
			int num = PhotonNetwork.CurrentRoom.GetRoomProperty(RoomProp.ROOM_ID, 0);
			txtRoomNumber.text = string.Format("{0:D3}", num);

			txtRoomName.text = PhotonNetwork.CurrentRoom.GetRoomProperty(RoomProp.ROOM_NAME, $"Room{num}");

			string pwd = PhotonNetwork.CurrentRoom.GetRoomProperty(RoomProp.ROOM_PASSWORD, "");
			bool isPrivate = pwd.Trim() != "";
			imgRoomPassword.gameObject.SetActive(isPrivate);

			imgRoomMode.sprite = roomModeIcons[(int)GetRoomModeProperty()];

			isActiveChangedView = false;
		}

		public void SetChangedRoomInfo(PhotonHashtable changedProps)
		{
			if(changedProps.ContainsKey(RoomProp.ROOM_NAME))
			{
				txtRoomName.text = changedProps[RoomProp.ROOM_NAME].ToString();
			}

			if (changedProps.ContainsKey(RoomProp.ROOM_MODE))
			{
				RoomMode mode = (RoomMode)Enum.Parse(typeof(RoomMode), changedProps[RoomProp.ROOM_MODE].ToString());
				imgRoomMode.sprite = roomModeIcons[(int)mode];
			}

			if (changedProps.ContainsKey(RoomProp.ROOM_PASSWORD))
			{
				string pwd = changedProps[RoomProp.ROOM_PASSWORD].ToString();
				bool isPrivate = pwd.Trim() != "";
				imgRoomPassword.gameObject.SetActive(isPrivate);
			}
		}

		private void CloseChangedInfoView()
		{
			isActiveChangedView = false;
		}

		private void SetActiveChanged(bool isActive)
		{
			Color color = btnChangedRoomInfo.image.color;
			color.a = isActive ? 0f : 0.35f;
			btnChangedRoomInfo.image.color = color;
		}

		private RoomMode GetRoomModeProperty()
		{
			PhotonHashtable property = PhotonNetwork.CurrentRoom.CustomProperties;

			if (property.ContainsKey(RoomProp.ROOM_MODE))
				return (RoomMode)Enum.Parse(typeof(RoomMode), property[RoomProp.ROOM_MODE].ToString());
			else
				return RoomMode.Manner;
		}
	}
}