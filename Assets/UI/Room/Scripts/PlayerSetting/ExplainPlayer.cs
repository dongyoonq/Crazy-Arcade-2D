using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI.PlayerSetting
{
	public class ExplainPlayer : MonoBehaviour, IChangeableCharacter
	{
		private Image imgCharacterExplain;

		[SerializeField]
		private CharacterData selectedData;

		private CharacterData initData;

		private void Awake()
		{
			imgCharacterExplain = gameObject.GetComponent<Image>();
			initData = selectedData;
		}

		private void Start()
		{
			CharacterChanger.OnChangedCharacter += OnChangeCharacter;
			CharacterChanger.OnFocusOnCharacter += OnFocusOnCharacter;
			CharacterChanger.OnFocusOffCharacter += OnFocusOffCharacter;
		}

		public void OnChangeCharacter(CharacterData data)
		{
			selectedData = data;
			imgCharacterExplain.sprite = data.Description;
		}

		public void OnFocusOnCharacter(CharacterData data)
		{
			imgCharacterExplain.sprite = data.Description;
		}

		public void OnFocusOffCharacter()
		{
			imgCharacterExplain.sprite = selectedData.Description;
		}

		public void InitExplainPlayer()
		{
			OnChangeCharacter(initData);
		}
	}
}