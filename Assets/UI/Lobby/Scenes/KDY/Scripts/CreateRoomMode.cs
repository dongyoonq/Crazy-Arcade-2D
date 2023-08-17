using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Extension;

public class CreateRoomMode : MonoBehaviour
{
	[SerializeField]
	private Toggle tog_Manner;

	[SerializeField]
	private Toggle tog_Free;

	[SerializeField]
	private Toggle tog_Random;

	public void ChooseRoomMode(RoomMode mode)
	{
		switch(mode)
		{
			case RoomMode.Manner:
				tog_Manner.isOn = true;
				break;

			case RoomMode.Free:
				tog_Free.isOn = true;
				break;

			case RoomMode.Random:
				tog_Random.isOn = true;
				break;
		}
	}

	public RoomMode GetSeletedRoom()
	{
		if (tog_Free.isOn)
			return RoomMode.Free;

		if(tog_Random.isOn)
			return RoomMode.Random;

		return RoomMode.Manner;
	}
}
