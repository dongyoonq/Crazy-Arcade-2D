using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RoomUI.Chat
{
	public class NotifyChat : MonoBehaviour
	{
		[SerializeField]
		private ChatView chatView;

		public static UnityAction<NotifyChatType, string> OnNotifyChat;

		private void Awake()
		{
			OnNotifyChat += PrintNotifyChatMessage;
		}

		private void PrintNotifyChatMessage(NotifyChatType type, string message)
		{
			chatView.SendMessageToAllPlayers(type, message);
		}
	}
}