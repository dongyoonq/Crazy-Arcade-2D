using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListPanel : MonoBehaviour
{
    [SerializeField] private RectTransform playerContent;
    [SerializeField] private Button confirmButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(() => Destroy(gameObject));
    }

    public void ShowPlayers(Dictionary<int, Player> players)
    {
        foreach (Player player in players.Values)
        {
            PlayerList l_player = GameManager.Resource.Instantiate<PlayerList>("Prefabs/PlayerList", playerContent);
            l_player.playerName.text = player.NickName;
        }
    }
}
