using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using RoomUI.Utils;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;


namespace RoomUI.ScriptForTest
{
	public class LobbyManager : MonoBehaviourPunCallbacks
	{
		public enum Panel { Login, Menu, Lobby, Room }

		[SerializeField] LoginPanel loginPanel;
		[SerializeField] MenuPanel menuPanel;

		[SerializeField] RoomPanel roomPanel;

		private void Start()
		{
			SetNextAction();
		}

		/// <summary>
		/// ���� ���� ��Ʈ��ũ ��Ȳ�� �´� �ܰ�� �Ѿ�� ���� ����
		/// </summary>
		private void SetNextAction()
		{
			if (PhotonNetwork.IsConnected)
				OnConnectedToMaster();
			else if (PhotonNetwork.InRoom)
				OnJoinedRoom();
			else if (PhotonNetwork.InLobby)
				OnJoinedLobby();
			else
				OnDisconnected(DisconnectCause.None);
		}

		/// <summary>
		/// ���ӵ��� �� �ൿ
		/// </summary>
		public override void OnConnectedToMaster()
		{
			SetActivePanel(Panel.Menu);
		}

		/// <summary>
		/// ���� ������ �� �ൿ
		/// </summary>
		/// <param name="cause"></param>
		public override void OnDisconnected(DisconnectCause cause)
		{
			SetActivePanel(Panel.Login);
		}


		/// <summary>
		/// �� ����� ����
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="message"></param>
		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			BackToTheMenuPanel(returnCode, message);
		}

		/// <summary>
		/// �÷��̾ �濡 ����
		/// </summary>
		public override void OnJoinedRoom()
		{
			SetActivePanel(Panel.Room); //�� ����� ���������� �� panel�� Ȱ��ȭ ��Ŵ

			//�÷��̾��� ���¸� �ʱ�ȭ����� �� -> �ٸ� �濡 ���� �� ���� ���°� ������ �� ����
			PhotonNetwork.LocalPlayer.SetReady(false);
			PhotonNetwork.LocalPlayer.SetLoad(false);
			//��� �÷��̾ �����ϰ� �����ϱ� ���ؼ� �ϴ� ���� �濡 �� �� Load ���� ���� false�� ������ ��. 
			// -> ���� ������ �̵��� �������� ���¸� true�� �����ϸ�, ���� ������ ���� �÷��̾� load�� ���°��� ���� ��.
			//�̿� ���� ������ ���� ��� �÷��̾ ���� �����ߴ��� ���θ� �Ǵ��� �� ����. -> ��ΰ� true�� �� ������ �����ϵ��� ��
			//(GameManager > Start �޼ҵ忡�� true�� ����)

			PhotonNetwork.AutomaticallySyncScene = true; // ������ Ŭ���̾�Ʈ�� �Ϲ� Ŭ���̾�Ʈ���� ������ ����ȭ��.
														 // => ���� ��ȯ�� �� ������ �ִ� ������ ��� ���� �̵���
		}

		/// <summary>
		/// �÷��̾ ���� ����
		/// </summary>
		public override void OnLeftRoom()
		{
			PhotonNetwork.AutomaticallySyncScene = false; //false�� �� ���� �θ� Local Player�� ���� ���� �� ��� ������ ���� ���� ������ ��.

			SetActivePanel(Panel.Menu);
		}

		/// <summary>
		/// �̹� ���� ��ȿ� �ִ� ���¿��� ���ο� �÷��̾ ��� ���� �� ȣ��
		/// </summary>
		/// <param name="newPlayer"></param>
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			roomPanel.EntryPlayer(newPlayer);
		}

		/// <summary>
		/// �̹� ���� ��ȿ� �ִ� ���¿��� �ٸ� �÷��̾ ���� ���� �� ȣ��
		/// </summary>
		/// <param name="otherPlayer"></param>
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			roomPanel.LeavePlayer(otherPlayer);
		}

		/// <summary>
		/// ������ �ٲ���� ��
		/// </summary>
		/// <param name="newMasterClient"></param>
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (newMasterClient.IsMasterClient)
				roomPanel.SwitchedMasterPlayer(newMasterClient);
		}

		/// <summary>
		/// �÷��̾��� ���� ��Ȳ�� �ٲ���� �� ȣ���� -> �ٲ� ����� �뺸�ϴ� ����
		/// </summary>
		/// <param name="targetPlayer"></param>
		/// <param name="changedProps"></param>
		public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
		{
			roomPanel.UpdatePlayerState(targetPlayer);
		}

		/// <summary>
		/// �� ���� ����
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="message"></param>
		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			BackToTheMenuPanel(returnCode, message);
		}

		/// <summary>
		/// �� ���� ���� ����
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="message"></param>
		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			BackToTheMenuPanel(returnCode, message);
		}


		/// <summary>
		/// ������ Ư�� �гθ� Ȱ��ȭ, �������� ��Ȱ��ȭ �ϵ��� ����
		/// </summary>
		/// <param name="panel"></param>
		private void SetActivePanel(Panel panel)
		{
			loginPanel.gameObject?.SetActive(panel == Panel.Login);
			menuPanel.gameObject?.SetActive(panel == Panel.Menu);
			roomPanel.gameObject?.SetActive(panel == Panel.Room);
		}

		private void BackToTheMenuPanel(short returnCode, string message)
		{
			SetActivePanel(Panel.Menu); //���������ϱ� �ٽ� �޴� â���� ����

			string errorMsg = $"Create room faild with error ({returnCode}) : {message}";
			Debug.Log(errorMsg); //���� ���� �α׷� ���
		}
	}

}
