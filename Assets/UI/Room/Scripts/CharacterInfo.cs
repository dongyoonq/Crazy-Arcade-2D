using RoomUI.PlayerSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
	public Button ChoosedBtn;

	public CharacterData characterData;

	public UnityAction<CharacterInfo> OnSelectedCharacter;

	protected virtual void Awake()
	{
		ChoosedBtn.image.sprite = characterData.DefaultImage;
		ChoosedBtn.onClick.AddListener(() => SelectedCharacter());
	}

	private void SelectedCharacter()
	{
		ChoosedBtn.image.sprite = characterData.ActiveImage;
		OnSelectedCharacter?.Invoke(this);
	}

	public void UnSelectedCharacter()
	{
		ChoosedBtn.image.sprite = characterData.DefaultImage;
	}
}
