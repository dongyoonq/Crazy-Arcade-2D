using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI
{
	public class ExplainPlayer : MonoBehaviour, IChangeableCharacter
	{
		private Image imgCharacterExplain;

		private void Awake()
		{
			imgCharacterExplain = gameObject.GetComponent<Image>();
		}

		private void Start()
		{
			CharacterChanger.OnChangedCharacter += OnChangeCharacter;
		}

		public void OnChangeCharacter(CharacterData data)
		{
			imgCharacterExplain.sprite = data.Description;
		}
	}
}