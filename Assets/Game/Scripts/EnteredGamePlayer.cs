using CustomProperty;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnteredGamePlayer : MonoBehaviourPunCallbacks
{
	public Player CurPalyer;

	[SerializeField]
	private Image PlayerCharacter;

	[SerializeField]
	private Image PlayerLevel;

	[SerializeField]
	private TMP_Text NickName;

	[SerializeField]
	private Image TeamColor;

	public void SetEnteredPlayer(Player player)
	{
		CurPalyer = player;

		NickName.text = player.NickName;

        Color teamColor; 
		string hexColor = player.CustomProperties[PlayerProp.TEAM].ToString();
		ColorUtility.TryParseHtmlString(hexColor, out teamColor);
		TeamColor.color = teamColor;

		PlayerLevel.gameObject.SetActive(false);
	}
}
