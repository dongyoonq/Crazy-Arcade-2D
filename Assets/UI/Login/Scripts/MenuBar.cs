using KDY;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gangbie
{
    public class MenuBar : MonoBehaviour
    {
        [SerializeField] LobbyManager lobbyManager;

        [SerializeField] LoginPanel loginPanel;
        [SerializeField] KDY.LobbyPanel lobbyPanel;
        [SerializeField] RoomUI.RoomPanel roomPanel;
        [SerializeField] ChatManager chatManager;
        [SerializeField] GameObject chatArea;

        [SerializeField] GameObject myPage;
        [SerializeField] Button myPageItemButton;
        [SerializeField] Button myPageInfoButton;

        [SerializeField] GameObject menuPopUp;

        [SerializeField] ShopPanel shopPanel;

        [SerializeField] GameObject backPopUp;
        [SerializeField] GameObject exitPopUp;

        private bool menuPopUpCheck;

        private void Awake()
        {
            menuPopUpCheck = false;
        }

        public void OnMenuButtonClicked()
        {
            GameManager.Sound.Onclick();
            if (menuPopUpCheck == false)
            {
                menuPopUp.SetActive(true);
                menuPopUpCheck = true;
            }
            else
            {
                menuPopUp.SetActive(false);
                menuPopUpCheck = false;
            }
        }

        public void menuPopUpClose()
        {
            if (menuPopUp.active == true)
            {
                menuPopUp.SetActive(false);
                menuPopUpCheck = false;
            }
        }

        public void OnShopButtonClicked()
        {
            GameManager.Sound.Onclick();
            Debug.Log("Shop Button Clicked");
            menuPopUpClose();
            lobbyManager.SetActivePanel(LobbyManager.Panel.Shop);
        }

        public void OnItemButtonClicked()
        {
            GameManager.Sound.Onclick();
            Debug.Log("Item Button Clicked");
            menuPopUpClose();
            myPage.SetActive(true);
            myPageItemButton.onClick.Invoke();
        }

        public void OnInfoButtonClicked()
        {
            GameManager.Sound.Onclick();
            Debug.Log("Info Button Clicked");
            menuPopUpClose();
            myPage.SetActive(true);
            myPageInfoButton.onClick.Invoke();
        }

        public void OnBackButtonClicked()
        {
            GameManager.Sound.Onclick();
            Debug.Log("Back Button Clicked");
            menuPopUpClose();

            if (lobbyManager.curPanel == LobbyManager.Panel.Shop)
            {
                lobbyManager.SetActivePanel(lobbyManager.prevPanel);
            }
            else if (lobbyManager.curPanel == LobbyManager.Panel.Room)
            {
                lobbyManager.SetActivePanel(LobbyManager.Panel.Lobby);
                PhotonNetwork.LeaveRoom();
            }
            else if (lobbyManager.curPanel == LobbyManager.Panel.Lobby)
            {
                backPopUp.SetActive(true);
            }
        }

        public void OnExitButtonClicked()
        {
            GameManager.Sound.Onclick();
            Debug.Log("Exit Button Clicked");
            menuPopUpClose();
            exitPopUp.SetActive(true);
        }

        public void ReLogin()
        {
            GameManager.Sound.Onclick();

            lobbyManager.SetActivePanel(LobbyManager.Panel.Login);
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
            chatManager?.gameObject.SetActive(false);
            chatArea.SetActive(false);
            if (!GameManager.Data.reader.IsClosed)
                GameManager.Data.reader.Close();
        }

        public void OnClicked()
        {
            GameManager.Sound.Onclick();
        }
    }
}
