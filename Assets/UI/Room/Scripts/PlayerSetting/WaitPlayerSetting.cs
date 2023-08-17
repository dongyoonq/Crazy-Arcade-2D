using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RoomUI.PlayerSetting
{
	public class WaitPlayerSetting : MonoBehaviourPun
	{
		public Image PlayerImg;

		public Image LevelImg;

		public Image TeamColor;

		public string Team;

		private void Awake()
		{
			//TODO. develing Level 
			LevelImg.gameObject.SetActive(false);
		}
	}
}