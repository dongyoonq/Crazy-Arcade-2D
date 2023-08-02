using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField]
    private RectTransform roomType;
    [SerializeField]
    private TMP_Text roomName;
    [SerializeField]
    private TMP_Text roomNumber;
    [SerializeField]
    private Image roomState;
    [SerializeField]
    private TMP_Text currentPlayer;
    [SerializeField]
    private Button joinRoomButton;
    [SerializeField]
    private Button infoButton;

    PasswordRoomPanel passwordPanel;

    private int roomId;
    private string roomPassword;
    private bool isPasswordRoom;
    private RoomInfo info;

    private void Start()
    {
        passwordPanel = transform.parent.parent.parent.parent.GetChild(1).GetComponent<PasswordRoomPanel>();
    }

    public void Initialized(RoomInfo roomInfo, int number)
    {
        info = roomInfo;
        roomName.text = info.Name;
        currentPlayer.text = string.Format("{0} / {1}", info.PlayerCount, info.MaxPlayers);
        joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;
        roomState.sprite = (info.PlayerCount < info.MaxPlayers) ? Resources.Load<Sprite>("Waiting") : Resources.Load<Sprite>("Full");
        roomId = number;
        roomNumber.text = number.ToString();
        if (roomInfo.CustomProperties.ContainsKey("Password"))
        {
            isPasswordRoom = true;
            roomPassword = (string)roomInfo.CustomProperties["Password"];
        }
    }

    public void OnJoinButtonClicked()
    {
        //PhotonNetwork.LeaveLobby();
        if (isPasswordRoom)
        {
            passwordPanel.gameObject.SetActive(true);
            passwordPanel.confirmBtn.onClick.RemoveAllListeners();
            passwordPanel.confirmBtn.onClick.AddListener(PasswordRoomJoin);
            return;
        }

        PhotonNetwork.JoinRoom(roomName.text);
    }

    private void PasswordRoomJoin()
    {
        if (passwordPanel.passwordInput.text == roomPassword)
        {
            PhotonNetwork.JoinRoom(roomName.text);
            passwordPanel.gameObject.SetActive(false);
        }
        else
        {
            // Todo Fail Match Password
            Debug.Log("방 비밀번호가 다릅니다");
        }
    }
}
