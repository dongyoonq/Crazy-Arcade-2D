using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class QuickTeam : MonoBehaviour
{
	[SerializeField]
	private QuickTeamEntry BtnOneMatch;

	[SerializeField]
	private QuickTeamEntry BtnTeamMatch;

	private bool IsOneMatch;

	private void Awake()
	{
		BtnOneMatch.BtnTeam.onClick.AddListener(() => ChoosedOneMatch());
		BtnTeamMatch.BtnTeam.onClick.AddListener(() => ChoosedTeamMatch());
	}

	private void OnEnable()
	{
		ChoosedOneMatch();
	}

	private void ChoosedOneMatch()
	{
		IsOneMatch = true;
		BtnOneMatch.SetBtnImage(true);
		BtnTeamMatch.SetBtnImage(false);
	}

	private void ChoosedTeamMatch()
	{
		IsOneMatch = false;
		BtnOneMatch.SetBtnImage(false);
		BtnTeamMatch.SetBtnImage(true);
	}
}
