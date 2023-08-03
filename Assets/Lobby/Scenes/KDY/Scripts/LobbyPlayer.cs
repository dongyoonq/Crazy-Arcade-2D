using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KDY
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] TMP_Text playerName;
        private Player player;

        public void Initialized(Player player)
        {
            this.player = player;
            playerName.text = player.NickName;
        }
    }
}
