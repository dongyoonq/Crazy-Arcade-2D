using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryTestPlayer : MonoBehaviourPunCallbacks
{
	private void Start()
	{
		PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(1000, 10000)}";
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		//�׽�Ʈ�� �ϱ� ���� ���� ���̱� ������ ������� ����.
		RoomOptions options = new RoomOptions() { IsVisible = false };
		PhotonNetwork.JoinOrCreateRoom("DebugRoom", options, TypedLobby.Default);
	}
}
