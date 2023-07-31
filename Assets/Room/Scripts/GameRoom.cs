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
	/// �̹� ������ �ִ� ����� + ���� ����
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
	/// ��� �÷��̾ ready ���°� �Ǹ� start button Ȱ�� ó��
	/// </summary>
	public void CheckPlayerReadyState()
	{
		if (PhotonNetwork.IsMasterClient) //������ �ƴϸ� ���� Ȯ���� �ʿ�� ����. 
		{
			int readyCount = PhotonNetwork.PlayerList.Count(x => x.GetReady());

			if(readyCount > 1) //���� ȥ�� ���� ���� ���� ���� ���ϰ� ����
			{
				if (readyCount == PhotonNetwork.PlayerList.Length)
					startButton.onClick.AddListener(() => StartGame());
				else
					startButton.onClick.RemoveListener(() => StartGame());
			}
		}
	}

	/// <summary>
	/// ���� ������ �̵�
	/// </summary>
	public void StartGame()
	{
		PhotonNetwork.CurrentRoom.IsOpen = false;
		PhotonNetwork.CurrentRoom.IsVisible = false;

		//PhotonNetwork.LoadLevel("GameScene");
		Debug.Log("���ӽ���!!");
	}

	/// <summary>
	/// �� ������
	/// </summary>
	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom(); //���� ��Ʈ��ũ�� �� �����ٰ� ��û�ϱ�   
	}
}
