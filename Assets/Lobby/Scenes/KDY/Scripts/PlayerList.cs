using CustomProperty.Utils;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerList : MonoBehaviour
{
    [SerializeField]
	private TMP_Text playerName;

    [SerializeField]
	private Image state_Waiting;

    [SerializeField]
    private Image state_Playing;

    public void SetEnteredPlayer(Player player)
    {
        playerName.text = player.NickName;

        bool isLoad = player.GetLoad();
        state_Waiting.gameObject.SetActive(!isLoad);
		state_Playing.gameObject.SetActive(isLoad);
	}

	public void SetEnteredPlayer(string nickName, bool isLoad)
	{
		playerName.text = nickName;
		state_Waiting.gameObject.SetActive(!isLoad);
		state_Playing.gameObject.SetActive(isLoad);
	}
}
