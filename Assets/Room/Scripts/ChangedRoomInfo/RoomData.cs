using RoomUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomUI.ChangedRoomInfo
{
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
		/// �� ���� ��ȣ 
		/// </summary>
		public int Number { get; private set; }

		/// <summary>
		/// �� �̸�
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// �� ���
		/// </summary>
		public RoomMode Mode { get; set; }

		/// <summary>
		/// ��й� ���� ����
		/// </summary>
		public bool IsPrivateRoom { get; set; }

		/// <summary>
		/// �� ��й�ȣ
		/// </summary>
		public string Password { get; set; }
	}
}
