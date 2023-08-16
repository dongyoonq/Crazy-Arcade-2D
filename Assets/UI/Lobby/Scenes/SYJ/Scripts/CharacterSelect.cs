using LobbyUI.QuickStart;
using RoomUI.PlayerSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
	[SerializeField]
	private RectTransform selectContents;

	[SerializeField]
	private Image characterDesc;

	[SerializeField]
	private Image characterSpec;

	[SerializeField]
	private Button BtnOk;

	public CharacterInfo CurrentCharInfo { get; private set; }

	public UnityAction<CharacterData> OnClosedView;

	private void Awake()
	{
		var contents = selectContents.GetComponentsInChildren<CharacterInfo>();

		foreach(var info in contents)
		{
			info.OnSelectedCharacter += SetCharacterInfo;
		}

		BtnOk.onClick.AddListener(() => ColsedView());
		InitView();
	}

	public void InitView()
	{
		CurrentCharInfo = selectContents.GetComponentsInChildren<CharacterInfo>()[0];
		characterDesc.sprite = CurrentCharInfo.characterData.Description;
		characterSpec.sprite = CurrentCharInfo.characterData.CharacterSpec;
		CurrentCharInfo.ChoosedBtn.image.sprite = CurrentCharInfo.characterData.ActiveImage;
	}

	public void DisableView()
	{
		CurrentCharInfo.UnSelectedCharacter();
	}
 
  private void SetCharacterInfo(CharacterInfo data)
	{
		  CurrentCharInfo.UnSelectedCharacter();

		  CurrentCharInfo = data;

		  characterDesc.sprite = data.characterData.Description;
		  characterSpec.sprite = data.characterData.CharacterSpec;
	}

	private void ColsedView()
	{
		gameObject.SetActive(false);
		OnClosedView?.Invoke(CurrentCharInfo.characterData);
	}
}