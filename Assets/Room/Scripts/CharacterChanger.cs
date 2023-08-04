using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RoomUI
{
	public interface IChangeableCharacter
	{
		void OnChangeCharacter(CharacterData data);
	}

	public class CharacterChanger : MonoBehaviour
	{
		public static UnityAction<CharacterData> OnChangedCharacter;

		public void ChooseCharacter(CharacterData data)
		{
			OnChangedCharacter?.Invoke(data);
		}
	}
}