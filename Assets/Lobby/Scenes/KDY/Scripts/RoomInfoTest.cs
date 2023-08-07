using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace KDY
{
	public class RoomInfoTest : MonoBehaviourPun
	{
		private const string UI_PATH = "RoomInfoUI/ChangedRoomInfo";

		[SerializeField]
		private TMP_Text roomNumber;

		[SerializeField]
		private TMP_Text roomName;

		[SerializeField]
		private Image roomMode;

		[SerializeField]
		private List<Sprite> roomModeIcons;

		[SerializeField]
		private Image roomPassword;

		[SerializeField]
		private Button changedRoomInfo;

		private RoomUI.RoomData roomData;
		private RoomUI.ChangedRoomInfo roomInfo;

		private void Awake()
		{
			if (PhotonNetwork.IsMasterClient)
				SetMasterRoomInfo();
			else
			{
				Color color = changedRoomInfo.image.color;
				color.a = 0.35f;
				changedRoomInfo.image.color = color;
			}
		}

		private void OnEnable()
		{
			int num = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomId"];
			string name = GetRoomProperty("RoomName", $"Room{num}");
			string pwd = GetRoomProperty("Password", "");
			bool isPrivate = pwd.Trim() != "";
			SetRoomProperty("Map", "Random");

            roomData = new RoomUI.RoomData(num, name, isPrivateRoom: isPrivate, password: pwd);

			roomNumber.text = string.Format("{0:D3}", roomData.Number);
			SetRoomInfo(roomData);
		}

		public void SetMasterRoomInfo()
		{
			changedRoomInfo.onClick.AddListener(() => OpenChangedRoomInfoUI());

			Color color = changedRoomInfo.image.color;
			color.a = 0;
			changedRoomInfo.image.color = color;
		}

		private void OpenChangedRoomInfoUI()
		{
			if (roomInfo == null)
			{
                RoomUI.ChangedRoomInfo roomInfo = Resources.Load<RoomUI.ChangedRoomInfo>(UI_PATH);
				roomInfo.RoomData = roomData;
				roomInfo.OnClosedView += SetRoomInfo;
			}
		}


		private void SetRoomInfo(RoomUI.RoomData data)
		{
			roomData = data;

			roomName.text = roomData.Name;

			roomMode.sprite = roomModeIcons[(int)roomData.Mode];
			roomPassword.gameObject.SetActive(roomData.IsPrivateRoom);

			//SetRoomProperty("RoomName", roomData.Name);
			//SetRoomProperty("Password", roomData.Password);

			if (roomInfo != null) roomInfo = null;
		}

		private string GetRoomProperty(string propertyKey, string returnVal)
		{
			PhotonHashtable property = PhotonNetwork.CurrentRoom.CustomProperties;

			if (property.ContainsKey(propertyKey))
				return property[propertyKey].ToString();
			else
				return string.Empty;
		}

		private void SetRoomProperty(string propertyKey, string value)
		{
			PhotonHashtable property = PhotonNetwork.CurrentRoom.CustomProperties;

			property[propertyKey] = value;
			PhotonNetwork.CurrentRoom.SetCustomProperties(property);
		}
	}
}