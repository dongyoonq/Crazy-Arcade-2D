using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickTeamEntry : MonoBehaviour
{
	[SerializeField]
	private Sprite ChoosedImg;

	[SerializeField]
	private Sprite DefaultImg;

	public Button BtnTeam;

	public void SetBtnImage(bool isChoosed)
	{
		BtnTeam.image.sprite = isChoosed ? ChoosedImg : DefaultImg;
	}
}
