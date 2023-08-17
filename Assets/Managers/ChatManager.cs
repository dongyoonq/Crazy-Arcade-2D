using ExitGames.Client.Photon;
using KDY;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    private string userName;
    private string currentChannelName;

    private ChatAppSettings chatAppSettings;

    [SerializeField] LobbyPanel lobbyPanel;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text outputText;
    [SerializeField] RectTransform playerListContent;

    [SerializeField] TMP_Dropdown dropdown;

    [SerializeField] TMP_Text toInput;

    private string privateReceiver = "";
    private string currentChat;
    private bool isInputFieldFocused = false;

    // Use this for initialization
    private void OnEnable()
    {
        Application.runInBackground = true;

        userName = PhotonNetwork.LocalPlayer.NickName; // System.Environment.UserName;
        currentChannelName = "Channel 001";

        chatClient = new ChatClient(this);
        chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.Connect(chatAppSettings.AppIdChat, chatAppSettings.AppVersion, new Photon.Chat.AuthenticationValues(userName));
        lobbyPanel.chatClient = chatClient;

        AddLine(string.Format("����õ�", userName));
    }

    private void OnDisable()
    {
        chatClient.Disconnect();
        chatClient = null;
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
        AddLine("������ ����Ǿ����ϴ�.");

        ChannelCreationOptions options = new ChannelCreationOptions();
        options.PublishSubscribers = true;
        chatClient.Subscribe(currentChannelName, 10, -1, options);
    }

    public void OnDisconnected()
    {
        AddLine("������ ������ ���������ϴ�.");

        chatClient.Unsubscribe(new string[] { currentChannelName });
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        AddLine(string.Format("ä�� ���� ({0})", string.Join(",", channels)));


        if (chatClient.TryGetChannel(currentChannelName, out ChatChannel channel))
        {
            channel.Subscribers.Add(userName);
            lobbyPanel.UpdatePlayerList(channel);
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        AddLine(string.Format("ä�� ���� ({0})", string.Join(",", channels)));


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
        AddLine(string.Format("(�ӼӸ�) {0} : {1}", sender, message.ToString()));
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }


    private void Update()
    {
        chatClient.Service();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isInputFieldFocused)
            {
                // ��ǲ�ʵ忡 �����մϴ�.
                inputField.Select();
                isInputFieldFocused = true;
            }
            else
            {
                // ��ǲ�ʵ忡�� ä���� �����մϴ�.
                SubmitPublicChatOnClick();
                SubmitPrivateChatOnClick();
                isInputFieldFocused = false; // ��Ŀ���� �����մϴ�.
            }
        }

        // ��ǲ�ʵ� �̿��� ������ Ŭ������ �� ��Ŀ���� �����մϴ�.
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
        {
            isInputFieldFocused = false;
        }
    }

    public void Input_OnEndEdit(string text)
    {
        if (chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            if (privateReceiver != "")
            {
                SubmitPrivateChatOnClick();
                SubmitPublicChatOnClick();
                //dropdown.value = dropdown.options.Count;
            }
            else
            {
                //chatClient.PublishMessage(currentChannelName, text);
                chatClient.PublishMessage(currentChannelName, inputField.text);

                inputField.text = "";
            }
        }
    }


    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"{user}�� {channel}�� ����(����)�ߴ�");

        if (chatClient.TryGetChannel(currentChannelName, out ChatChannel ch))
        {
            lobbyPanel.UpdatePlayerList(ch);
        }
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"{user}�� {channel}�� ����(���� ���)�ߴ�");

        if (chatClient.TryGetChannel(currentChannelName, out ChatChannel ch))
        {
            lobbyPanel.UpdatePlayerList(ch);
        }
    }

    // PrivateChat
    public void ReceiverOnValueChange(string valueIn)
    {
        string curReceiver = privateReceiver;
        privateReceiver = valueIn;

        if (dropdown.options.Count >= 2)
        {
            dropdown.options[1].text = valueIn;
            dropdown.options[2].text = curReceiver;
        }
    }

    public void SubmitPrivateChatOnClick()
    {
        if (privateReceiver != "" && isConnectedPlayer(privateReceiver))
        {
            chatClient.SendPrivateMessage(privateReceiver, inputField.text);
            inputField.text = "";
            currentChat = "";
        }      
    }

    bool isConnectedPlayer(string name)
    {
        LobbyPlayer[] lobbyPlayers = playerListContent.GetComponentsInChildren<LobbyPlayer>();

        foreach (LobbyPlayer player in lobbyPlayers)
        {
            if (player.playerName == name)
            {
                return true;
            }
        }

        AddLine($"{name}�� ���� ���������� �ʽ��ϴ�");
        inputField.text = "";
        return false;
    }

    public void SubmitPublicChatOnClick()
    {
        if (privateReceiver == "")
        {
            chatClient.PublishMessage("Channel 001", inputField.text);
            inputField.text = "";
            currentChat = "";
        }
    }

    public void TypeChatOnValueChange(string valueIn)
    {
        currentChat = valueIn;
    }

    public void OnDropdownValueChanging()
    {
        Color textColor = toInput.color;
        float opacity = 1.0f;
        textColor.a = opacity;
        toInput.color = textColor;
    }

    public void OnDropdownValueChanged()
    {
        Debug.Log(dropdown.value);

        Color textColor = toInput.color;


        if (dropdown.value == 0)
        {
            privateReceiver = "";

            float opacity = 0.0f;

            textColor.a = opacity;
            toInput.color = textColor;
        }
        else if (dropdown.value == 1)
        {
            privateReceiver = dropdown.options[1].text;

            float opacity = 1.0f;

            textColor.a = opacity;
            toInput.color = textColor;
        }
        else if (dropdown.value == 2)
        {
            privateReceiver = dropdown.options[2].text;

            float opacity = 0.0f;

            textColor.a = opacity;
            toInput.color = textColor;
        }
    }

    public void OnToInputEndEdit()
    {
        dropdown.value = 1;
        Debug.Log(dropdown.value);
    }
}