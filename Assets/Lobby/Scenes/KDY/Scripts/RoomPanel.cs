using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry playerEntryPrefab;
    [SerializeField] Button startButton;

    public void UpdatePlayerList()
    {
        // Clear Player List
        for (int i = 0; i < playerContent.childCount; i++)
            Destroy(playerContent.GetChild(i).gameObject);

        // Update Player List
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerEntry entry = Instantiate(playerEntryPrefab, playerContent);
            entry.Initialized(player);
        }

        if (PhotonNetwork.IsMasterClient)
            CheckPlayerReady();
        else
            startButton.gameObject.SetActive(false);
    }
    
    public void OnStartRoomButtonClicked()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void CheckPlayerReady()
    {
        int readyCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 만약에 플레이어가 레디 커스텀 프로퍼티가 true 이면
            PhotonHashtable property = player.CustomProperties;
            if (player.GetReady())
                readyCount++;
        }

        startButton.gameObject.SetActive(readyCount == PhotonNetwork.PlayerList.Length);
    }
}
