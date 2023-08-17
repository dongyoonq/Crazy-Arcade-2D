using CustomProperty.Utils;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KDY
{
    public class PlayerEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerReady;
        [SerializeField] private Button playerReadyButton;

        private Player player;

        public void Initialized(Player player)
        {
            this.player = player;
            playerName.text = player.NickName;
            playerReady.text = player.GetReady() ? "Ready" : "";
            playerReadyButton.gameObject.SetActive(player.IsLocal);
        }

        public void OnReadyButtonClicked()
        {
            bool ready = player.GetReady();
            ready = !ready;
            player.SetReady(ready);
        }
    }
}