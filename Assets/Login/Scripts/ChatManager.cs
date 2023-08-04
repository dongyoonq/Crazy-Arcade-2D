using ExitGames.Client.Photon;
using KDY;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDY
{
    public class ChatManager : MonoBehaviour, IChatClientListener
    {
        private ChatClient chatClient;
        private string userName;
        private string currentChannelName;

        private ChatAppSettings chatAppSettings;

        [SerializeField] LobbyPanel lobbyPanel;
        [SerializeField] TMP_InputField inputField;
        [SerializeField] TMP_Text outputText;

        // Use this for initialization
        void Start()
        {

            Application.runInBackground = true;

            userName = PhotonNetwork.LocalPlayer.NickName; // System.Environment.UserName;
            currentChannelName = "Channel 001";

            chatClient = new ChatClient(this);
            chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
            chatClient.Connect(chatAppSettings.AppIdChat, chatAppSettings.AppVersion, new Photon.Chat.AuthenticationValues(userName));

            AddLine(string.Format("연결시도", userName));
        }

        public void AddLine(string lineString)
        {
            outputText.text += lineString + "\r\n";
        }

        public void OnApplicationQuit()
        {
            if (chatClient != null)
            {
                chatClient.Disconnect();
            }
        }

        public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
        {
            if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
            {
                Debug.LogError(message);
            }
            else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
            {
                Debug.LogWarning(message);
            }
            else
            {
                Debug.Log(message);
            }
        }

        public void OnConnected()
        {
            AddLine("서버에 연결되었습니다.");

            ChannelCreationOptions options = new ChannelCreationOptions();
            options.PublishSubscribers = true;
            chatClient.Subscribe(currentChannelName, 10, -1, options);
        }

        public void OnDisconnected()
        {
            AddLine("서버에 연결이 끊어졌습니다.");
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("OnChatStateChange = " + state);
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            AddLine(string.Format("채널 입장 ({0})", string.Join(",", channels)));


            if (chatClient.TryGetChannel(currentChannelName, out ChatChannel channel))
            {
                channel.Subscribers.Add(userName);
                lobbyPanel.UpdatePlayerList(channel);
            }
        }

        public void OnUnsubscribed(string[] channels)
        {
            AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));


            if (chatClient.TryGetChannel(currentChannelName, out ChatChannel channel))
            {
                channel.Subscribers.Remove(userName);
                lobbyPanel.UpdatePlayerList(channel);
            }
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
            }
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            Debug.Log("OnPrivateMessage : " + message);
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
        }

        void Update()
        {
            chatClient.Service();
        }

        public void Input_OnEndEdit(string text)
        {
            if (chatClient.State == ChatState.ConnectedToFrontEnd)
            {
                //chatClient.PublishMessage(currentChannelName, text);
                chatClient.PublishMessage(currentChannelName, inputField.text);

                inputField.text = "";
            }
        }

        public void OnUserSubscribed(string channel, string user)
        {
            Debug.Log($"{user}가 {channel}에 입장(구독)했다");

            if (chatClient.TryGetChannel(currentChannelName, out ChatChannel ch))
            {
                lobbyPanel.UpdatePlayerList(ch);
            }
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            Debug.Log($"{user}가 {channel}에 퇴장(구독 취소)했다");

            if (chatClient.TryGetChannel(currentChannelName, out ChatChannel ch))
            {
                lobbyPanel.UpdatePlayerList(ch);
            }
        }
    }

}