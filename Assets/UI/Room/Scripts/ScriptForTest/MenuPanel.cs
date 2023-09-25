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
			roomNameInputField.text = string.Empty; //방 이름 클리어
			maxPlayerInputField.text = string.Empty; //인원수 클리어

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
			maxPlayer = Mathf.Clamp(maxPlayer, 1, 8); //1~8까지만 설정 가능하도록 제한함

			PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)maxPlayer }); //옵션으로 최대 인원수를 제한함
																							//RoomOptions이 종류가 엄청 다양함. + 옵션은 언제든 교체 가능함.
																							//=> RoomOptions를 활용해서 다양한 방 설정이 가능함.

			//CreateRoom은 방 만들기 신청일 뿐, 방을 확정적으로 만든 것이 아님. 서버에서 방이 만들어진 결과를 받아야 방이 만들어졌다는 사실을 확정 지을 수 있음
			//그에 따른 반응을 구현해야함 -> 서버에서 받아와? -> callback함수를 활용
			//성공 시 OnJoinedRoom, 실패 시 OnCreateRoomFailed 호출함
		}

		public void CreateRoomCancel()
		{
			createRoomPanel.SetActive(false);
		}

		public void RandomMatching()
		{
			string roomNm = "001";// $"Room {Random.Range(1000, 10000)}";
			RoomOptions roomOps = new RoomOptions { MaxPlayers = 8 };
			PhotonNetwork.JoinRandomOrCreateRoom(roomName: roomNm, roomOptions: roomOps); //랜덤 매칭으로 들어가려고 했는데 방이 없으면, 방을 만들어서 들어가도록 함.
		}

		public void JoinLobby()
		{
			PhotonNetwork.JoinLobby(); //로비에 들어가겠다
		}

		public void Logout()
		{
			PhotonNetwork.Disconnect();
		}
	}
}