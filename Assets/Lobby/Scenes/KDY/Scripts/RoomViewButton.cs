using KDY;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomViewButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] RectTransform roomContent;
    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] LobbyPanel lobbyPanel;

    Image buttonImg;
    Sprite waitingRoomImg;
    Sprite allRoomImg;
    bool isAllRoomBtn;

    private void Start()
    {
        isAllRoomBtn = true;
        button = GetComponent<Button>();
        buttonImg = GetComponent<Image>();
        waitingRoomImg = Resources.Load<Sprite>("대기방보기&입장");
        allRoomImg = Resources.Load<Sprite>("AllRoomView");

        button.onClick.AddListener(ButtonChange);
    }

    private void ButtonChange()
    {
        RoomEntry[] roomEntries = roomContent.GetComponentsInChildren<RoomEntry>();

        if (isAllRoomBtn)
        {
            buttonImg.sprite = waitingRoomImg;

            // Todo : WaitingRoomView
            isAllRoomBtn = false;

            foreach (RoomEntry roomEntry in roomEntries)
            {
                if ((string)roomEntry.RoomInfo.CustomProperties["RoomState"] != "Waiting")
                    Destroy(roomEntry.gameObject);
            }
        }
        else
        {
            buttonImg.sprite = allRoomImg;

            // Todo : AllRoomView
            isAllRoomBtn = true;
            lobbyPanel.UpdateRoomList(lobbyManager.Rooms);
        }
    }
}
