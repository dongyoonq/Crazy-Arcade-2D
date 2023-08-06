using Photon.Realtime;
using SYJ;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDY
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] TMP_Text playerNameText;
        private Canvas popupCanvas;
        private string playerName;

        private void Start()
        {
            popupCanvas = GameObject.Find("PopUp").GetComponent<Canvas>();
        }

        public void Initialized(string name)
        {
            playerNameText.text = name;
            playerName = name;
            GetComponent<Button>().onClick.AddListener(CreateUserInfoPanel);
        }

        private void CreateUserInfoPanel()
        {
            UserInfoPanel infoPanel = GameManager.Resource.Instantiate<UserInfoPanel>("Prefabs/UserInfo");
            infoPanel.transform.SetParent(popupCanvas.transform, false);
            infoPanel.ConnectDataBase();
            infoPanel.ReadSqlData(playerName);
    }
        }
}
