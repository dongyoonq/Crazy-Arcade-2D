using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RoomUI.PlayerSetting
{
	public interface IChangeableCharacter
	{
		void OnChangeCharacter(CharacterData data);

		void OnFocusOnCharacter(CharacterData data);

		void OnFocusOffCharacter();
	}

	public class CharacterChanger : MonoBehaviour
	{
		public static UnityAction<CharacterData> OnChangedCharacter;
		public static UnityAction<CharacterData> OnFocusOnCharacter;
		public static UnityAction OnFocusOffCharacter;

		public void ChooseCharacter(CharacterData data)
		{
			OnChangedCharacter?.Invoke(data);
		}

		public void FocusOnCharacter (CharacterData data)
		{
			OnFocusOnCharacter?.Invoke(data);
		}

		public void FocusOffCharacter()
		{
			OnFocusOffCharacter?.Invoke();
		}
	}
}