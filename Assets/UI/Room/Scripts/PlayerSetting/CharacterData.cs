using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Extension;

namespace RoomUI.PlayerSetting
{
	[CreateAssetMenu(fileName = "CharacterData", menuName = "Room/Character")]
	public class CharacterData : ScriptableObject
	{
		public string Name;

		public string NameKOR;

		public CharacterEnum CharacterEnum; 

		public Sprite Character;

		public Sprite Description;

		public Sprite DefaultImage;

		public Sprite ActiveImage;

		public Sprite CharacterSpec;

		public Sprite SelectedImage;
	}
}