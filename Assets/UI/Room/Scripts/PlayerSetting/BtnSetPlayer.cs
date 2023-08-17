using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoomUI.PlayerSetting
{
	public class BtnSetPlayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private CharacterData charData;
		public CharacterData CharacterData { get { return charData; } }

		private Button btnChooseCharacter;

		private CharacterChanger characterChanger;
		public UnityAction<BtnSetPlayer> OnChooseCharacter;

		private bool isMouseOverUI = false;

		private void Awake()
		{
			characterChanger = GetComponentInParent<CharacterChanger>();
			
			btnChooseCharacter = GetComponent<Button>();
			btnChooseCharacter.image.sprite = charData.DefaultImage;
			btnChooseCharacter.onClick.AddListener(() => SelectedCharacter());
		}

		public void InitButton(bool isActive)
		{
			btnChooseCharacter.image.sprite = isActive ? charData.ActiveImage : charData.DefaultImage;
		}

		private void SelectedCharacter()
		{
            GameManager.Sound.Onclick();

            btnChooseCharacter.image.sprite = charData.ActiveImage;
			characterChanger.ChooseCharacter(charData);
			OnChooseCharacter?.Invoke(this);
		}

		public void UnSelectedCharacter()
		{
			btnChooseCharacter.image.sprite = charData.DefaultImage;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			characterChanger.FocusOnCharacter(charData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			characterChanger.FocusOffCharacter();
		}
	}
}