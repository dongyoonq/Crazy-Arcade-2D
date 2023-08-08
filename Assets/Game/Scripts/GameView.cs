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
using RoomUI.Utils;
using GameUI;


public class GameView : MonoBehaviourPunCallbacks
{
	private const string LobbyScene = "RoomScene ver 2";

	[SerializeField]
	private TMP_Text infoText;

	private float countdownTimer;

	private void Start()
	{
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LocalPlayer.SetLoad(true); //씬에 잘 넘어왔다는 의미로 프로퍼티를 변경
			SetPlayer();
		}
		countdownTimer = 5f;
	}

	public override void OnConnectedToMaster()
	{
	}

	private void SetPlayer()
	{
		float angularStart = (360.0f / 8f) * PhotonNetwork.LocalPlayer.GetPlayerNumber();
		float x = 20.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
		float z = 20.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
		Vector3 position = new Vector3(x, 0.0f, z);

		var newPlayer = PhotonNetwork.Instantiate("GamePlayer", position, Quaternion.identity).transform.GetComponent<GamePlayer>();
		newPlayer.PlayerNickName.text = PhotonNetwork.LocalPlayer.NickName;
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
		if (changedProps.ContainsKey(CustomProperty.PROPERTYKEY_LOAD))
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
		if (propertiesThatChanged.ContainsKey(CustomProperty.PROPERTYKEY_LOADTIME))
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
