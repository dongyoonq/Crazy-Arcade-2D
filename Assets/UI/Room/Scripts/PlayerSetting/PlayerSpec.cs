using RoomUI.PlayerSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerSpec : MonoBehaviour, IChangeableCharacter
{
	[SerializeField]
	private Image ImgSpec;

	private void Awake()
	{
	}

	private void Start()
	{
		CharacterChanger.OnFocusOnCharacter += OnFocusOnCharacter;
		CharacterChanger.OnFocusOffCharacter += OnFocusOffCharacter;
	}

	public void OnChangeCharacter(CharacterData data)
	{
	}

	public void OnFocusOnCharacter(CharacterData data)
	{
		ImgSpec.gameObject.SetActive(true);
		ImgSpec.sprite = data.CharacterSpec;
	}

	public void OnFocusOffCharacter()
	{
		ImgSpec.gameObject.SetActive(false);
	}
}
