using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using TMPro;
using UnityEngine;
using System.Linq;

public class Server : MonoBehaviour
{
    [SerializeField] RectTransform logContent;
    [SerializeField] TMP_Text logTextPrefab;
    [SerializeField] TMP_InputField ipField;
    [SerializeField] TMP_InputField portField;

    public void Open()
    {
        
    }

    public void Close()
    {
        
    }

    public void SendAll(string chat)
    {
        
    }

    private void AddLog(string message)
    {
        Debug.Log(string.Format("[Server] {0}", message));
        TMP_Text newLog = Instantiate(logTextPrefab, logContent);
        newLog.text = message;
    }
}
