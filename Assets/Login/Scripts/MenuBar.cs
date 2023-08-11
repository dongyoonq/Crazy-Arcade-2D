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
        [SerializeField] Gangbie.LobbyManager lobbyManager;

        [SerializeField] LoginPanel loginPanel;
        [SerializeField] KDY.LobbyPanel lobbyPanel;
        [SerializeField] RoomUI.RoomPanel roomPanel;

        [SerializeField] GameObject myPage;
        [SerializeField] Button myPageItemButton;

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
            Debug.Log("Shop Button Clicked");
            menuPopUpClose();
            lobbyManager.SetActivePanel(LobbyManager.Panel.Shop);
        }

        public void OnItemButtonClicked()
        {
            Debug.Log("Item Button Clicked");
            menuPopUpClose();
            myPage.SetActive(true);
            myPageItemButton.onClick.Invoke();
        }

        public void OnInfoButtonClicked()
        {
            Debug.Log("Info Button Clicked");
            menuPopUpClose();
            myPage.SetActive(true);
        }

        public void OnBackButtonClicked()
        {
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
            Debug.Log("Exit Button Clicked");
            menuPopUpClose();
            exitPopUp.SetActive(true);
        }

        public void ReLogin()
        {
            lobbyManager.SetActivePanel(LobbyManager.Panel.Login);
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
            if (!GameManager.Data.reader.IsClosed)
                GameManager.Data.reader.Close();
        }
    }
}
