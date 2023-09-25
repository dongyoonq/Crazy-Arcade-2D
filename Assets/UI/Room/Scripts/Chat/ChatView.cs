using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace RoomUI.Chat
{
	public enum NotifyChatType
	{
		Normal,
		Warning,
		Critical,
		Log
	}

	public class ChatView : MonoBehaviourPunCallbacks
	{
		//private const string UI_PATH = "ChatView/ChatMessage";
		private const string UI_PATH = "ChatView/TxtChatMessage";

		[SerializeField]
		private Transform ChatContent;

		[SerializeField]
		public ScrollRect scrollView;

		[SerializeField]
		private TMP_InputField whisperTarget;

		[SerializeField]
		private TMP_InputField chatField;

		[SerializeField]
		private List<Color> chatColorByType;

		private void OnSendMessage(InputValue value)
		{
			if (chatField.text != "")
			{
				string sendMsg = $"▷ {PhotonNetwork.LocalPlayer.NickName} : {chatField.text}";

				if (whisperTarget.text != "")
				{
					if (PhotonNetwork.PlayerList.Any(x => x.NickName == whisperTarget.text))
					{
						SendWhisperSpecificPlayer(NotifyChatType.Normal, sendMsg, whisperTarget.text);
						return;
					}
				}
				SendMessageToAllPlayers(NotifyChatType.Normal, sendMsg);
			}
		}

		private void SendWhisperSpecificPlayer(NotifyChatType chatType, string message, string targetId)
		{
			photonView.RPC("ChatMessage", RpcTarget.All, chatType, message, PhotonNetwork.LocalPlayer.NickName, targetId);
		}

		public void SendMessageToAllPlayers(NotifyChatType chatType, string message)
		{
			photonView.RPC("ChatMessage", RpcTarget.All, chatType, message);
		}


		[PunRPC]
		private void ChatMessage(NotifyChatType chatType, string message)
		{
			AddChatMessage(message, (int)chatType);
		}

		[PunRPC]
		private void ChatMessage(NotifyChatType chatType, string message, string sender, string targetId)
		{
			if (PhotonNetwork.LocalPlayer.NickName == sender || PhotonNetwork.LocalPlayer.NickName == targetId)
				AddChatMessage(message, (int)chatType);
		}

		private void AddChatMessage(string message, int color)
		{
			/*var newChatMsg = Instantiate(Resources.Load<ChatMessage>(UI_PATH), Vector3.zero, Quaternion.identity);
			newChatMsg.transform.SetParent(ChatContent);
			newChatMsg.txtChatMessage.text = message;

			Canvas.ForceUpdateCanvases(); // UI 업데이트 강제 호출
			scrollView.verticalNormalizedPosition = 0f;*/

			var newChatMsg = Instantiate<TMP_Text>(Resources.Load<TMP_Text>(UI_PATH), ChatContent);
			newChatMsg.color = chatColorByType[color];
			newChatMsg.text = message;

			chatField.text = "";
		}
	}

}

