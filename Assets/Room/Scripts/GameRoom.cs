using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameRoom : MonoBehaviour
{
	[SerializeField] 
	private RectTransform playerContent;
	
	[SerializeField]
	private Button startButton;

	private Dictionary<int, WaitingPlayer> playerDictionary;

	private void Awake()
	{
		playerDictionary = new Dictionary<int, WaitingPlayer>();
	}

	private void OnEnable()
	{
		SetInPlayer();
		CheckPlayerReadyState();
	}

	private void OnDisable()
	{
		foreach (int actorNumber in playerDictionary.Keys)
		{
			Destroy(playerDictionary[actorNumber].gameObject);
		}
		playerDictionary.Clear();
	}

	public void EntryPlayer(Player player)
	{
		InstantiatePlayer(player);
		CheckPlayerReadyState();
	}

	public void LeavePlayer(Player leavePlayer)
	{
		Destroy(playerDictionary[leavePlayer.ActorNumber].gameObject);
		playerDictionary.Remove(leavePlayer.ActorNumber);
		CheckPlayerReadyState();
	}

	public void UpdatePlayerState(Player player)
	{
		GetPalyerEntry(player)?.UpdateReadyInfo();

		if (PhotonNetwork.IsMasterClient)
			CheckPlayerReadyState();
	}

	/// <summary>
	/// 이미 접속해 있는 사용자 + 본인 생성
	/// </summary>
	private void SetInPlayer()
	{
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			InstantiatePlayer(player);
		}
	}

	private void InstantiatePlayer(Player player)
	{
		int index = playerDictionary.Count();

		if (index == 8)
			return;

		WaitingPlayer waitingPlayer = playerContent.GetComponentsInParent<WaitingPlayer>()[index];
		waitingPlayer.SetPlayer(player);
		playerDictionary.Add(player.ActorNumber, waitingPlayer);
	}



	private WaitingPlayer GetPalyerEntry(Player chkPlayer)
	{
		return playerContent.GetComponentsInChildren<WaitingPlayer>().Where(x => x.player.ActorNumber == chkPlayer.ActorNumber).FirstOrDefault();
	}

	public void UpdateActiveStartButton(int actorNumber)
	{
		//startButton.gameObject.SetActive()
	}

	

	

	/// <summary>
	/// 모든 플레이어가 ready 상태가 되면 start button 활성 처리
	/// </summary>
	public void CheckPlayerReadyState()
	{
		if (PhotonNetwork.IsMasterClient) //방장이 아니면 굳이 확인할 필요는 없음. 
		{
			int readyCount = PhotonNetwork.PlayerList.Count(x => x.GetReady());

			if(readyCount > 1) //방장 혼자 있을 때는 게임 시작 못하게 막음
			{
				if (readyCount == PhotonNetwork.PlayerList.Length)
					startButton.onClick.AddListener(() => StartGame());
				else
					startButton.onClick.RemoveListener(() => StartGame());
			}
		}
	}

	/// <summary>
	/// 게임 씬으로 이동
	/// </summary>
	public void StartGame()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;

		//PhotonNetwork.LoadLevel("GameScene");
		Debug.Log("게임시작!!");
	}

	/// <summary>
	/// 방 떠나기
	/// </summary>
	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom(); //포톤 네트워크에 방 나간다고 신청하기   
	}
}
