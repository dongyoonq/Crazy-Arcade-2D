using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI.PlayerSetting
{
	[CreateAssetMenu(fileName = "CharacterData", menuName = "Room/Character")]
	public class CharacterData : ScriptableObject
	{
		public string Name;

		public Sprite Character;

		public Sprite Description;
	}
}