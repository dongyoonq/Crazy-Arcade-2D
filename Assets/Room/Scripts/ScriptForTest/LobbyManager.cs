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
		/// 현재 포톤 네트워크 상황에 맞는 단계로 넘어가기 위한 세팅
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
		/// 접속됐을 때 행동
		/// </summary>
		public override void OnConnectedToMaster()
		{
			SetActivePanel(Panel.Menu);
		}

		/// <summary>
		/// 접속 끊겼을 때 행동
		/// </summary>
		/// <param name="cause"></param>
		public override void OnDisconnected(DisconnectCause cause)
		{
			SetActivePanel(Panel.Login);
		}


		/// <summary>
		/// 방 만들기 실패
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="message"></param>
		public override void OnCreateRoomFailed(short returnCode, string message)
		{
			BackToTheMenuPanel(returnCode, message);
		}

		/// <summary>
		/// 플레이어가 방에 입장
		/// </summary>
		public override void OnJoinedRoom()
		{
			SetActivePanel(Panel.Room); //방 만들기 성공했으니 방 panel을 활성화 시킴

			//플레이어의 상태를 초기화해줘야 함 -> 다른 방에 들어갔을 때 이전 상태가 유지될 수 있음
			PhotonNetwork.LocalPlayer.SetReady(false);
			PhotonNetwork.LocalPlayer.SetLoad(false);
			//모든 플레이어가 동일하게 시작하기 위해서 일단 게임 방에 들어갈 때 Load 상태 값을 false로 들어가도록 함. 
			// -> 다음 씬으로 이동한 시점에서 상태를 true로 변경하면, 접속 순서에 따라 플레이어 load의 상태값이 변경 됨.
			//이와 같은 과정을 통해 모든 플레이어가 씬에 접속했는지 여부를 판단할 수 있음. -> 모두가 true일 때 게임을 시작하도록 함
			//(GameManager > Start 메소드에서 true로 변경)

			PhotonNetwork.AutomaticallySyncScene = true; // 마스터 클라이언트와 일반 클라이언트들이 레벨을 동기화함.
														 // => 씬을 전환할 때 방장이 있는 씬으로 모두 같이 이동함
		}

		/// <summary>
		/// 플레이어가 방을 나감
		/// </summary>
		public override void OnLeftRoom()
		{
			PhotonNetwork.AutomaticallySyncScene = false; //false로 안 돌려 두면 Local Player가 방을 나갈 때 모든 유저가 같이 따라서 나가게 됨.

			SetActivePanel(Panel.Menu);
		}

		/// <summary>
		/// 이미 내가 방안에 있는 상태에서 새로운 플레이어가 들어 왔을 때 호출
		/// </summary>
		/// <param name="newPlayer"></param>
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			roomPanel.EntryPlayer(newPlayer);
		}

		/// <summary>
		/// 이미 내가 방안에 있는 상태에서 다른 플레이어가 방을 나갈 때 호출
		/// </summary>
		/// <param name="otherPlayer"></param>
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			roomPanel.LeavePlayer(otherPlayer);
		}

		/// <summary>
		/// 방장이 바뀌었을 때
		/// </summary>
		/// <param name="newMasterClient"></param>
		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (newMasterClient.IsMasterClient)
				roomPanel.SwitchedMasterPlayer(newMasterClient);
		}

		/// <summary>
		/// 플레이어의 레디 상황이 바뀌었을 때 호출함 -> 바뀐 결과를 통보하는 역할
		/// </summary>
		/// <param name="targetPlayer"></param>
		/// <param name="changedProps"></param>
		public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
		{
			roomPanel.UpdatePlayerState(targetPlayer);
		}

		/// <summary>
		/// 방 입장 실패
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="message"></param>
		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			BackToTheMenuPanel(returnCode, message);
		}

		/// <summary>
		/// 방 랜덤 입장 실패
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="message"></param>
		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			BackToTheMenuPanel(returnCode, message);
		}


		/// <summary>
		/// 지정된 특정 패널만 활성화, 나머지는 비활성화 하도록 설정
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
			SetActivePanel(Panel.Menu); //실패했으니까 다시 메뉴 창으로 복귀

			string errorMsg = $"Create room faild with error ({returnCode}) : {message}";
			Debug.Log(errorMsg); //실패 사유 로그로 기록
		}
	}

}
