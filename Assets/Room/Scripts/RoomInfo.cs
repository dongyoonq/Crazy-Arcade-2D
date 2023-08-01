using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoomData
{
	public RoomData(int number, string name)
	{
		Number = number;
		Name = name;
	}

	/// <summary>
	/// 방 고유 번호 
	/// </summary>
	public int Number { get; private set; }

	/// <summary>
	/// 방 이름
	/// </summary>
	public string Name { get; private set; }
}

public class RoomInfo : MonoBehaviour
{
	[SerializeField]
	private TMP_Text roomNumber;

	[SerializeField]
	private TMP_Text roomName;

	[SerializeField]
	private Button changedRoomInfo;

	private RoomData roomData;

	private void OnEnable()
	{
		roomData = new RoomData(PhotonNetwork.CountOfRooms + 1, PhotonNetwork.CurrentRoom.Name);

		roomName.text = roomData.Name;
		roomNumber.text = string.Format("{0:D3}", roomData.Number);
	}
}
