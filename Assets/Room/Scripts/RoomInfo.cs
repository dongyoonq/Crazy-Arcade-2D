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

namespace RoomUI
{
	public enum RoomMode
	{
		Manner,
		Free,
		Random
	}

	public class RoomData
	{
		public RoomData(int number, string name, RoomMode mode = RoomMode.Manner, bool isPrivateRoom = false, string password = "")
		{
			Number = number;
			Name = name;

			Mode = mode;
			IsPrivateRoom = isPrivateRoom;
			Password = password;
		}

		/// <summary>
		/// 방 고유 번호 
		/// </summary>
		public int Number { get; private set; }

		/// <summary>
		/// 방 이름
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 방 모드
		/// </summary>
		public RoomMode Mode { get; set; }

		/// <summary>
		/// 비밀방 설정 여부
		/// </summary>
		public bool IsPrivateRoom { get; set; }

		/// <summary>
		/// 방 비밀번호
		/// </summary>
		public string Password { get; set; }
	}

	public class RoomInfo : MonoBehaviourPun
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

		private RoomData roomData;
		private ChangedRoomInfo roomInfo;

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

			roomData = new RoomData(num, name, isPrivateRoom: isPrivate, password: pwd);

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
				ChangedRoomInfo roomInfo = Resources.Load<ChangedRoomInfo>(UI_PATH);
				roomInfo.RoomData = roomData;
				roomInfo.OnClosedView += SetRoomInfo;
			}
		}


		private void SetRoomInfo(RoomData data)
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