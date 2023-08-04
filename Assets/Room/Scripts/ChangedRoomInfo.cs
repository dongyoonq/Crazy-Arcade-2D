using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangedRoomInfo : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private TMP_InputField roomName;

	[SerializeField]
	private TMP_InputField roomPassword;

	[SerializeField] 
	private Button btnOK;

	[SerializeField]
	private Button btnCancel;

	public RoomData RoomData;

	public UnityAction<RoomData> OnClosedView;

	private void Awake()
	{
		btnOK.onClick.AddListener(() => ClosedRoom());
		btnCancel.onClick.AddListener(() => CloseUI());
	}

	private void OnEnable()
	{
		RoomData = new RoomData(1, "RoomName");
		//roomName.text = RoomData.Name;
		//roomPassword.text = RoomData.Password;
	}

	private void CloseUI()
	{
		//todo.Ã¢´Ý±â
	}

	private void ClosedRoom()
	{
		RoomData.Name = roomName.text;
		RoomData.Password = roomPassword.text;
		OnClosedView?.Invoke(RoomData);

		//todo.Ã¢´Ý±â
	}
}
