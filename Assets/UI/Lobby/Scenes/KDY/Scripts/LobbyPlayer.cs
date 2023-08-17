using Photon.Realtime;
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
        public string playerName;

        private void Start()
        {
            popupCanvas = GameObject.Find("PopUp").GetComponent<Canvas>();
            GetComponent<Button>().onClick.AddListener(() => GameManager.Sound.Onclick());
        }

        public void Initialized(string name)
        {
            playerNameText.text = name;
            playerName = name;
            GetComponent<Button>().onClick.AddListener(CreateUserInfoPanel);
        }

        private void CreateUserInfoPanel()
        {
            GameManager.Sound.Onclick();
            UserPage infoPanel = GameManager.Resource.Instantiate<UserPage>("Prefabs/UserPage");
            infoPanel.ShowUserPage(playerName);
            infoPanel.transform.SetParent(popupCanvas.transform, false);
    }
        }
}
