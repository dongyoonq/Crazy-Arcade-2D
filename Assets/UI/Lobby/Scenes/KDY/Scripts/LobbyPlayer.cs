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
        }

        public void Initialized(string name)
        {
            playerNameText.text = name;
            playerName = name;
            GetComponent<Button>().onClick.AddListener(CreateUserInfoPanel);
        }

        private void CreateUserInfoPanel()
        {
            MyPage infoPanel = GameManager.Resource.Instantiate<MyPage>("Prefabs/UserPage");
            infoPanel.transform.SetParent(popupCanvas.transform, false);
    }
        }
}