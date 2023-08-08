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

namespace RoomUI.ChangedRoomInfo
{
	public enum RoomMode
	{
		Manner,
		Free,
		Random
	}

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

		private RoomData roomData;

		private bool isActiveChangedView;

		private void Awake()
		{
			if (PhotonNetwork.IsMasterClient)
				btnChangedRoomInfo.onClick.AddListener(() => OpenChangedRoomInfoUI());
			SetActiveChanged(PhotonNetwork.IsMasterClient);

			isActiveChangedView = false;
		}

		private void OnEnable()
		{
			int num = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomId"];
			string name = GetRoomProperty("RoomName", $"Room{num}");
			string pwd = GetRoomProperty("Password", "");
			bool isPrivate = pwd.Trim() != "";
            SetRoomProperty("Map", "Random"); // Add From Lobby

            roomData = new RoomData(num, name, isPrivateRoom: isPrivate, password: pwd);

			txtRoomNumber.text = string.Format("{0:D3}", roomData.Number);
			SetRoomInfo(roomData);
		}

		public void SetMasterRoomInfo()
		{
			SetActiveChanged(true);
			btnChangedRoomInfo.onClick.AddListener(() => OpenChangedRoomInfoUI());
		}

		private void OpenChangedRoomInfoUI()
		{
			if (isActiveChangedView == false)
			{
				viewChangedRoomInfo.SetExistingInfo(roomData);
				viewChangedRoomInfo.gameObject.SetActive(true);
				viewChangedRoomInfo.OnClosedView += SetRoomInfo;
				isActiveChangedView = true;
			}
		}

		private void SetRoomInfo(RoomData data)
		{
			if (data == null) // cancel
				return;

			roomData = data;

			txtRoomName.text = roomData.Name;

			imgRoomMode.sprite = roomModeIcons[(int)roomData.Mode];
			imgRoomPassword.gameObject.SetActive(roomData.IsPrivateRoom);

			SetRoomProperty("RoomName", roomData.Name);
			SetRoomProperty("Password", roomData.Password);

			viewChangedRoomInfo.OnClosedView -= SetRoomInfo;
			isActiveChangedView = false;

			photonView.RPC("SetRoomInfoImg", RpcTarget.Others, (int)roomData.Mode, roomData.IsPrivateRoom);
		}
		
		private void SetActiveChanged(bool isActive)
		{
			Color color = btnChangedRoomInfo.image.color;
			color.a = isActive ? 0f : 0.35f;
			btnChangedRoomInfo.image.color = color;
		}


		[PunRPC]
		private void SetRoomInfoImg(int mode, bool isPrivate)
		{
			imgRoomMode.sprite = roomModeIcons[mode];
			imgRoomPassword.gameObject.SetActive(isPrivate);
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
};