using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using CustomProperty.Utils;
using GameUI;
using System;
using CustomProperty;

public class GameView : MonoBehaviourPunCallbacks
{
	private const string LobbyScene = "RoomScene ver 2";

	[SerializeField]
	private TMP_Text infoText;

	[SerializeField]
	private RectTransform EnteredPlayerList;

	[SerializeField]
	private RectTransform Players;  // 부모 오브젝트의 Transform 컴포넌트


	private float countdownTimer;

	private void Start()
	{
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LocalPlayer.SetLoad(true); //씬에 잘 넘어왔다는 의미로 프로퍼티를 변경

			InstantiatePlayer();
		}
		countdownTimer = 5f;
	}


	public override void OnConnectedToMaster()
	{
	}

	private void InstantiatePlayer()
	{
		float baseSizeX = Players.rect.width / 2;
		float baseSizeY = Players.rect.height / 2;

		float randomX = UnityEngine.Random.Range(baseSizeX * -1, baseSizeX);
		float randomY = UnityEngine.Random.Range(baseSizeY * -1, baseSizeY);

		Vector3 postion = new Vector3(randomX, randomY, 0);
		var newPlayer = PhotonNetwork.Instantiate("GamePlayer", postion, Quaternion.identity);
		newPlayer.transform.SetParent(Players.transform, false);
		newPlayer.GetComponent<GamePlayer>().PlayerNickName.text = PhotonNetwork.NickName;

		SetEnteredPlayerList();
	}

	private void SetEnteredPlayerList()
	{
		var enteredList = EnteredPlayerList.GetComponentsInChildren<EnteredGamePlayer>();

		int index = 0;
		foreach (var player in PhotonNetwork.CurrentRoom.Players)
		{
			//player.Value.GetPlayerNumber()
			//enteredList[player.Value.GetPlayerNumber()].SetEnteredPlayer(player.Value);
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.LogError($"Disconnected : {cause}");
		SceneManager.LoadScene(LobbyScene);
	}

	public override void OnLeftRoom()
	{
		Debug.LogError("Left Room");
		PhotonNetwork.LoadLevel(LobbyScene);
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
	{
		if (changedProps.ContainsKey(PlayerProp.LOAD))
		{
			int loadingCnt = PlayerLoadCount();
			if (loadingCnt == PhotonNetwork.PlayerList.Length)
			{
				PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.ServerTimestamp);
				infoText.text = $"All Player Loaded";
			}
			else
			{
				infoText.text = $"Wait Players ({loadingCnt}/{PhotonNetwork.PlayerList.Length})";
			}
		}
	}

	private int PlayerLoadCount()
	{
		return PhotonNetwork.PlayerList.Where(x => x.GetLoad()).Count();
	}

	public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey(RoomProp.LOAD_TIME))
		{
			StartCoroutine(GameStartTimer());
		}
	}

	private IEnumerator GameStartTimer()
	{
		int loadTime = PhotonNetwork.CurrentRoom.GetLoadTime();
		while (countdownTimer > (PhotonNetwork.ServerTimestamp - loadTime) / 1000f) //서버시간 - 로드시간이 countdownTimer보다 작을 동안 반복
																					//서버시간은 단위가 밀리세컨드라 나누기 1000을 해줘야함
		{
			int reaminTime = (int)(countdownTimer - (PhotonNetwork.ServerTimestamp - loadTime) / 1000f);
			infoText.text = $"All Player Loaded, Start Count donw : {reaminTime + 1}";

			yield return new WaitForEndOfFrame();
		}
		GameStart();
	}

	private void GameStart()
	{
		infoText.text = "GAME START!";

		//TODO. game start
	}

}
