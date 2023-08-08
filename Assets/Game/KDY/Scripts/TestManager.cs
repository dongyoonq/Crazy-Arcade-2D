using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using RoomUI.Utils;
using GameUI;
using static Photon.Pun.UtilityScripts.PunTeams;

public class TestManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float countdownTimer;

    private void Start()
    {
        // Normal game mode
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LocalPlayer.SetLoad(true);
        }
        // Debug game mode
        else
        {
            infoText.text = "Debug Mode";
            PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(0, 1000)}";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("DebugRoom", new RoomOptions() { IsVisible = false }, TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected : {cause}");
        SceneManager.LoadScene("Lobby_Test2");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        PhotonNetwork.LoadLevel("Lobby_Test2");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey("Load"))
        {
            // 모든 플레이어 로딩 완료
            if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)
            {
                // 게임 시작
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.ServerTimestamp);
            }
            else
            {
                // 다른 플레이어 로딩 될 때까지 대기
                Debug.Log($"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}");
                infoText.text = $"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            }
        }
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("LoadTime"))
        {
            StartCoroutine(GameStartTimer());
        }
    }

    IEnumerator GameStartTimer()
    {
        int loadTime = PhotonNetwork.CurrentRoom.GetLoadTime();
        while (countdownTimer > (PhotonNetwork.ServerTimestamp - loadTime) / 1000f)
        {
            int remainTime = (int)(countdownTimer - (PhotonNetwork.ServerTimestamp - loadTime) / 1000f);
            infoText.text = $"All Player Loaded, Start count down : {remainTime + 1}";
            yield return new WaitForEndOfFrame();
        }

        infoText.text = "Game Start !";
        GameStart();

        yield return new WaitForSeconds(1f);
        infoText.text = "";
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(DebugGameSetupDelay());
    }

    private void GameStart()
    {
        // Todo : debug game start
        Debug.Log("Game Mode");
    }

    IEnumerator DebugGameSetupDelay()
    {
        // 서버에게 여유시간 1초 기다려주기
        yield return new WaitForSeconds(1f);
        DebugGameStart();
    }

    private void DebugGameStart()
    {
        // Team Property Test : 실제로는 Room에서 프로퍼티 설정함
        string[] randoms = { "RED", "YELLOW", "ORANGE", "GREEN", "SKY", "BLUE", "PURPLE", "MAGENTA" };

        SetPlayerTeamProperty(randoms[Random.Range(0,8)]);

        Vector3 position = new Vector3(Random.Range(-5f,5f), Random.Range(-5f, 5f), 0f);

        Debug.Log($"[SetPlayer] {PhotonNetwork.LocalPlayer.NickName}");

        PhotonNetwork.Instantiate("Prefabs/Bazzi", position, Quaternion.identity); 
        // Complete Room than Replace CharacterPropertyInstantiate() Method
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //
    }

    private int PlayerLoadCount()
    {
        int loadCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
            if (player.GetLoad())
                loadCount++;

        return loadCount;
    }

    private void SetPlayerTeamProperty(string teamColor)
    {
        PhotonHashtable property = PhotonNetwork.LocalPlayer.CustomProperties;
        property["Team"] = teamColor;
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    }

    /// <summary>
    /// 방에서 선택된 캐릭터로 생성될수 있게 함수 작성
    /// </summary>
    private void CharacterPropertyInstantiate()
    {
        PhotonHashtable property = PhotonNetwork.LocalPlayer.CustomProperties;

        if (!property.ContainsKey("Character"))
        {
            Debug.Log("프로퍼티가 없습니다");
        }

        switch (property["Character"])
        {
            // ex
            /*
             * case Character.Bazzi:
             *      PhotonNetwork.Instantiate("Prefabs/Bazzi", position, Quaternion.identity); 
             *      break;
             */
        }
    }
}
