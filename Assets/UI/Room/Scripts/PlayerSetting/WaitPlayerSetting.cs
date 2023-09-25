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
		private CharacterData _charData;
		public CharacterData CharData 
		{ 
			get { return _charData; }
			set
			{
				_charData = value;
				PlayerImg.sprite = _charData.Character;
			}
		}

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