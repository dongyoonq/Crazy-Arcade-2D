using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatTest : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    private string userName;
    private string currentChannelName;

    private ChatAppSettings chatAppSettings;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text outputText;

    [SerializeField] TMP_Dropdown dropdown;

    [SerializeField] TMP_Text toInput;

    private string privateReceiver = "";
    private string currentChat;

    // Use this for initialization
    void Start()
    {

        Application.runInBackground = true;

        userName = PhotonNetwork.LocalPlayer.NickName; // System.Environment.UserName;
        currentChannelName = "Channel 001";

        chatClient = new ChatClient(this);
        chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        chatClient.Connect(chatAppSettings.AppIdChat, chatAppSettings.AppVersion, new AuthenticationValues(userName));

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

        chatClient.Subscribe(new string[] { currentChannelName }, 10);
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
        Debug.Log(dropdown.value);
    }

    public void OnUnsubscribed(string[] channels)
    {
        AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));
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
        AddLine(string.Format("(귓속말) {0} : {1}", sender, message.ToString()));
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
    }

    void Update()
    {
        chatClient.Service();

        if (inputField.text != "" && Input.GetKey(KeyCode.Return))
        {
            SubmitPublicChatOnClick();
            SubmitPrivateChatOnClick();
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
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
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
        if (privateReceiver != "")
        {
            chatClient.SendPrivateMessage(privateReceiver, inputField.text);
            inputField.text = "";
            currentChat = "";
        }
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
