using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatView : MonoBehaviourPunCallbacks
{
	//private const string UI_PATH = "ChatView/ChatMessage";
	private const string UI_PATH = "ChatView/TxtChatMessage";

	[SerializeField]
	private Transform ChatContent;

	[SerializeField]
	public ScrollRect scrollView;

	[SerializeField]
	private TMP_InputField chatField;

	private void OnSendMessage(InputValue value)
	{
		if (chatField.text != "")
		{
			string sendMsg = $"▷ {PhotonNetwork.LocalPlayer.NickName} : {chatField.text}";

			photonView.RPC("ChatMessage", RpcTarget.All, sendMsg);
		}
	}

	[PunRPC]
	private void ChatMessage(string message)
	{
		AddChatMessage(message);
	}

	void AddChatMessage(string message)
	{
		/*var newChatMsg = Instantiate(Resources.Load<ChatMessage>(UI_PATH), Vector3.zero, Quaternion.identity);
		newChatMsg.transform.SetParent(ChatContent);
		newChatMsg.txtChatMessage.text = message;

		Canvas.ForceUpdateCanvases(); // UI 업데이트 강제 호출
		scrollView.verticalNormalizedPosition = 0f;*/

		var newChatMsg = Instantiate<TMP_Text>(Resources.Load<TMP_Text>(UI_PATH), ChatContent);
		newChatMsg.text = message;

		chatField.text = "";
	}
}
