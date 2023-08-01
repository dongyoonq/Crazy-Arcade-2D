using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BtnSetPlayer : MonoBehaviour
{
	[SerializeField]
    private CharacterData characterInfo;

	private Button btnChooseCharacter;
	private CharacterChanger characterChanger;

	public UnityAction<CharacterData> OnChooseCharacter;

	private void Awake()
	{
		characterChanger = GetComponentInParent<CharacterChanger>();

		btnChooseCharacter = GetComponent<Button>();
		btnChooseCharacter.onClick.AddListener(() => characterChanger.ChooseCharacter(characterInfo));
	}
}
