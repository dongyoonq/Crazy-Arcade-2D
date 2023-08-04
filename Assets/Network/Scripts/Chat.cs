using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [SerializeField] Client client;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] RectTransform content;
    [SerializeField] ScrollRect scrollRect;

    [SerializeField] TMP_Text chatTextPrefab;

    private void Awake()
    {
        inputField.onSubmit.AddListener(SendChat);
    }

    public void AddMessage(string message)
    {
        TMP_Text newMessage = Instantiate(chatTextPrefab);
        newMessage.text = message;
        newMessage.transform.SetParent(content);
        scrollRect.verticalScrollbar.value = 0;
    }

    public void SendChat(string chat)
    {
        if (client != null)
            client.SendChat(chat);

        inputField.text = "";
        inputField.ActivateInputField();
    }
}
