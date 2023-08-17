using System.IO;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] Chat chat;

    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    public void Connect()
    {
        
    }

    public void DisConnect()
    {
        
    }

    public void SendChat(string chatText)
    {

    }

    public void ReceiveChat(string chatText)
    {
        
    }

    private void AddMessage(string message)
    {
        Debug.Log($"[Client] {message}");
        chat.AddMessage($"[Client] {message}");
    }
}
