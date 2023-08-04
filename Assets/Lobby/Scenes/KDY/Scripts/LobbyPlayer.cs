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

        public void Initialized(string name)
        {
            playerName.text = name;
        }
    }
}
