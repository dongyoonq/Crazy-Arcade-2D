using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace RoomUI.ScriptForTest
{
	public class MenuPanel : MonoBehaviour
	{
		[SerializeField] GameObject createRoomPanel;
		[SerializeField] TMP_InputField roomNameInputField;
		[SerializeField] TMP_InputField maxPlayerInputField;
		[SerializeField] TMP_Text UserId;

		public void OnEnable()
		{
			createRoomPanel.SetActive(false);
			roomNameInputField.text = string.Empty; //�� �̸� Ŭ����
			maxPlayerInputField.text = string.Empty; //�ο��� Ŭ����

			UserId.text = PhotonNetwork.NickName;
		}

		public void CreateRoomMenu()
		{
			createRoomPanel.SetActive(true);
		}

		public void CreateRoomConfirm()
		{
			string roomName = roomNameInputField.text;

			if (roomName == "")
			{
				roomName = $"Room {Random.Range(1000, 10000)}";
				roomNameInputField.text = roomName;
			}

			int maxPlayer = maxPlayerInputField.text == "" ? 8 : int.Parse(maxPlayerInputField.text);
			maxPlayer = Mathf.Clamp(maxPlayer, 1, 8); //1~8������ ���� �����ϵ��� ������

			PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)maxPlayer }); //�ɼ����� �ִ� �ο����� ������
																							//RoomOptions�� ������ ��û �پ���. + �ɼ��� ������ ��ü ������.
																							//=> RoomOptions�� Ȱ���ؼ� �پ��� �� ������ ������.

			//CreateRoom�� �� ����� ��û�� ��, ���� Ȯ�������� ���� ���� �ƴ�. �������� ���� ������� ����� �޾ƾ� ���� ��������ٴ� ����� Ȯ�� ���� �� ����
			//�׿� ���� ������ �����ؾ��� -> �������� �޾ƿ�? -> callback�Լ��� Ȱ��
			//���� �� OnJoinedRoom, ���� �� OnCreateRoomFailed ȣ����
		}

		public void CreateRoomCancel()
		{
			createRoomPanel.SetActive(false);
		}

		public void RandomMatching()
		{
			string roomNm = "001";// $"Room {Random.Range(1000, 10000)}";
			RoomOptions roomOps = new RoomOptions { MaxPlayers = 8 };
			PhotonNetwork.JoinRandomOrCreateRoom(roomName: roomNm, roomOptions: roomOps); //���� ��Ī���� ������ �ߴµ� ���� ������, ���� ���� ������ ��.
		}

		public void JoinLobby()
		{
			PhotonNetwork.JoinLobby(); //�κ� ���ڴ�
		}

		public void Logout()
		{
			PhotonNetwork.Disconnect();
		}
	}
}