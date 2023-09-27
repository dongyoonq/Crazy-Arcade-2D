using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using CustomProperty.Utils;
using CustomProperty;
using static Extension;
using KDY;
using System.IO;
using System.Linq;
using Unity.VisualScripting;

public class InGameManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private RectTransform enteredPlayerList;
	[SerializeField] private TMP_Text infoText;
	[SerializeField] private float countdownTimer;
	[SerializeField] private PlayerSpawn playerSpawn;
	[SerializeField] private ResultScreen resultPanel;

	public Dictionary<string, TEAM> teamPlayerDic = new Dictionary<string, TEAM>();
	public List<InGamePlayer> playerList = new List<InGamePlayer>();

	private void Start()
	{
		GameManager.Sound.SFXPlay("Start", GameManager.Sound.startSound);
		GameManager.Sound.BgmStop(GameManager.Sound.lobbySource);
		GameManager.Sound.BgmPlay(GameManager.Sound.patritSource);  // 맵에 맞는 음악 설정

		// Normal game mode
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LocalPlayer.SetLoad(true);
		}
		// Debug game mode
		else
		{
			infoText.text = "Debug Mode";
			PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(0, 1000)}";
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinOrCreateRoom("DebugRoom", new RoomOptions() { IsVisible = false }, TypedLobby.Default);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log($"Disconnected : {cause}");
		SceneManager.LoadScene("Lobby_Test2");
	}

	public override void OnLeftRoom()
	{
		Debug.Log("Left Room");
		PhotonNetwork.LoadLevel("Lobby_Test2");
	}

	public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
	{
		if (changedProps.ContainsKey(PlayerProp.LOAD))
		{
			// 모든 플레이어 로딩 완료
			if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)
			{
				// 게임 시작
				if (PhotonNetwork.IsMasterClient)
					PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.ServerTimestamp);
			}
			else
			{
				// 다른 플레이어 로딩 될 때까지 대기
				Debug.Log($"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}");
				infoText.text = $"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
			}
		}
	}

	public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey(RoomProp.LOAD_TIME))
		{
			StartCoroutine(GameStartTimer());
		}
	}

	IEnumerator GameStartTimer()
	{
		int loadTime = PhotonNetwork.CurrentRoom.GetLoadTime();
		while (countdownTimer > (PhotonNetwork.ServerTimestamp - loadTime) / 1000f)
		{
			int remainTime = (int)(countdownTimer - (PhotonNetwork.ServerTimestamp - loadTime) / 1000f);
			infoText.text = $"All Player Loaded, Start count down : {remainTime + 1}";
			yield return new WaitForEndOfFrame();
		}

		infoText.text = "Game Start !";
		GameStart();

		yield return new WaitForSeconds(1f);
		infoText.text = "";
	}

	public override void OnJoinedRoom()
	{
		StartCoroutine(DebugGameSetupDelay());
	}

	private void GameStart()
	{
		// Character Set
		CharacterInstantiate();

		// PlayerList Update
		SetEnteredPlayerList();
	}

	IEnumerator DebugGameSetupDelay()
	{
		// 서버에게 여유시간 1초 기다려주기
		yield return new WaitForSeconds(1f);
		DebugGameStart();
	}

	private void DebugGameStart()
	{
		// Team Set
		SetPlayerProperty();

		// Debug Character Instantiate
		CharacterInstantiate();

		// PlayerList Update
		SetEnteredPlayerList();
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		//
	}

	private int PlayerLoadCount()
	{
		return PhotonNetwork.PlayerList.Where(x => x.GetLoad()).Count();
	}

	private void SetPlayerProperty()
	{
		PhotonHashtable property = PhotonNetwork.LocalPlayer.CustomProperties;
		property[PlayerProp.TEAM] = "Red";
		property[PlayerProp.TEAMCOLOR] = $"#{Color.red.ToHexString()}";
		property[PlayerProp.CHARACTER] = CharacterEnum.Dao;
		PhotonNetwork.LocalPlayer.SetCustomProperties(property);
	}

	private void SetEnteredPlayerList()
	{
		PhotonNetwork.Instantiate("EnteredPlayer", Vector3.zero, Quaternion.identity);
	}

	/// <summary>
	/// 방에서 선택된 캐릭터로 생성될수 있게 함수 작성
	/// </summary>
	private void CharacterInstantiate()
	{
		PhotonHashtable property = PhotonNetwork.LocalPlayer.CustomProperties;

		if (!property.ContainsKey(PlayerProp.CHARACTER))
		{
			Debug.Log("프로퍼티가 없습니다");
		}

		Vector3 position = playerSpawn.spawnPoints[PhotonNetwork.LocalPlayer.GetPlayerNumber()].transform.position;

		switch ((CharacterEnum)property[PlayerProp.CHARACTER])
		{
			case CharacterEnum.Dao:
				PhotonNetwork.Instantiate("Prefabs/Dao", position, Quaternion.identity);
				Debug.Log("Dao Create");
				break;
			case CharacterEnum.Cappi:
				PhotonNetwork.Instantiate("Prefabs/Cappi", position, Quaternion.identity);
				Debug.Log("Cappi Create");
				break;
			case CharacterEnum.Marid:
				PhotonNetwork.Instantiate("Prefabs/Marid", position, Quaternion.identity);
				Debug.Log("Marid Create");
				break;
			case CharacterEnum.Bazzi:
				PhotonNetwork.Instantiate("Prefabs/Bazzi", position, Quaternion.identity);
				Debug.Log("Bazzi Create");
				break;
		}
	}

	public void AddPlayerTeamList(InGamePlayer teamPlayer)
	{
		photonView.RPC("SyncAllAddTeamList", RpcTarget.MasterClient, SerializePlayerData(teamPlayer));
	}

	public void RemovePlayerTeamList(InGamePlayer teamPlayer)
	{
		photonView.RPC("SyncAllRemoveTeamList", RpcTarget.MasterClient, SerializePlayerData(teamPlayer));
	}

	public void CheckGameState()
	{
		photonView.RPC("SyncCheckGameState", RpcTarget.MasterClient);
	}

	[PunRPC]
	private void SyncAllAddTeamList(byte[] serializedData)
	{
		InGamePlayer receivedData = DeserializePlayerData(serializedData);
		teamPlayerDic.Add(receivedData.playerName, receivedData.currTeam);
		playerList.Add(receivedData);
	}

	[PunRPC]
	private void SyncAllRemoveTeamList(byte[] serializedData)
	{
		InGamePlayer receivedData = DeserializePlayerData(serializedData);
		//teamPlayerDic.Remove(teamPlayerDic.Find(x => x.playerName == receivedData.playerName));

		foreach (string player in teamPlayerDic.Keys)
		{
			if (player == receivedData.playerName)
			{
				Debug.Log($"{player} 리스트 삭제");
				teamPlayerDic.Remove(player);
				break;
			}
		}
	}

	public byte[] SerializePlayerData(InGamePlayer data)
	{
		using (MemoryStream stream = new MemoryStream())
		{
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				writer.Write((int)data.currTeam);
				writer.Write(data.playerName);
			}
			return stream.ToArray();
		}
	}

	public InGamePlayer DeserializePlayerData(byte[] data)
	{
		InGamePlayer result = new InGamePlayer();
		using (MemoryStream stream = new MemoryStream(data))
		{
			using (BinaryReader reader = new BinaryReader(stream))
			{
				result.currTeam = (TEAM)reader.ReadInt32();
				result.playerName = reader.ReadString();
			}
		}
		return result;
	}

	[PunRPC]
	public void SyncCheckGameState()
	{
		/*
         *       Manner = 0,
         *       Free = 1,
         *       Random = 2
        */
		foreach (KeyValuePair<string, TEAM> player in teamPlayerDic)
		{
			Debug.Log($"{player.Key} : {player.Value} 생존");
		}

		PhotonHashtable property = PhotonNetwork.CurrentRoom.CustomProperties;

		Dictionary<TEAM, int> aliveTeams = new Dictionary<TEAM, int>();

		// 생존하는 팀들을 저장
		foreach (KeyValuePair<string, TEAM> player in teamPlayerDic)
		{
			if (!aliveTeams.ContainsKey(player.Value))
				aliveTeams.Add(player.Value, 0);

			aliveTeams[player.Value] += 1;
		}

		// 팀이 하나만 남아있으면
		if (aliveTeams.Count == 1)
		{
			foreach (TEAM team in aliveTeams.Keys)
			{
				// 결과 표시
				photonView.RPC("RequestShowGameResult", RpcTarget.MasterClient, (int)team);
				break;
			}
		}


	}

	[PunRPC]
	private void RequestShowGameResult(int winTeam)
	{
		TEAM WinTeam = (TEAM)winTeam;
		Debug.Log($"{winTeam} 팀 승리");

		List<string> winPlayerList = new List<string>();
		List<string> losePlayerList = new List<string>();

		foreach (InGamePlayer player in playerList)
		{
			if (player.currTeam == WinTeam)
			{
				winPlayerList.Add(player.playerName);
			}
			else
			{
				losePlayerList.Add(player.playerName);
			}
		}

		// 결과창 표시
		photonView.RPC("ShowResultAll", RpcTarget.AllViaServer, winPlayerList.ToArray(), losePlayerList.ToArray());
		// 보상 적용
		// 게임 종료 및 방이동
		StartCoroutine(ReturnRoom());
	}

	[PunRPC]
	private void ShowResultAll(string[] winPlayerList, string[] losePlayerList)
	{
		resultPanel.gameObject.SetActive(true);

		foreach (string playerName in winPlayerList)
		{
			InstantiateResultPlayer(playerName, "Win", out ResultLine resultPlayer);

			if (playerName == PhotonNetwork.LocalPlayer.NickName)
			{
				GameManager.Sound.SFXPlay("Win", GameManager.Sound.win);
				resultPanel.SetCharacterImg(PhotonNetwork.LocalPlayer);
				resultPanel.SetResultImg("Win");
				resultPanel.SetExpMoney((Mathf.Round((300f / resultPlayer.prevMaxExp) * 100) * 0.01f) * 100.0f, 600f);
			}

			resultPlayer.UpdateInfo(playerName);
		}

		foreach (string playerName in losePlayerList)
		{
			InstantiateResultPlayer(playerName, "Lose", out ResultLine resultPlayer);

			if (playerName == PhotonNetwork.LocalPlayer.NickName)
			{
				GameManager.Sound.SFXPlay("Lose", GameManager.Sound.lose);
				resultPanel.SetCharacterImg(PhotonNetwork.LocalPlayer);
				resultPanel.SetResultImg("Lose");
				resultPanel.SetExpMoney((Mathf.Round((200f / resultPlayer.prevMaxExp) * 100) * 0.01f) * 100.0f, 400f);
			}

			resultPlayer.UpdateInfo(playerName);
		}
	}

	private IEnumerator ReturnRoom()
	{
		yield return new WaitForSeconds(5f);
		PhotonNetwork.LoadLevel("BeforeGameScene");
	}

	private void InstantiateResultPlayer(string playerName, string res, out ResultLine resultPlayer)
	{
		ResultLine result = GameManager.Resource.Instantiate<ResultLine>("ResultLine");
		result.SetResultImg(res);
		result.SetID(playerName);
		result.SetInfo(playerName);
		result.transform.SetParent(resultPanel.content, false);
		resultPlayer = result;
	}
}