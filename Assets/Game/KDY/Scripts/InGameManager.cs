using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using CustomProperty.Utils;
using GameUI;
using CustomProperty;
using static Extension;


public class InGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float countdownTimer;
    [SerializeField] PlayerSpawn playerSpawn;

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
        if (changedProps.ContainsKey(PlayerProp.LOAD))
        {
            // ��� �÷��̾� �ε� �Ϸ�
            if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)
            {
                // ���� ����
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.ServerTimestamp);
            }
            else
            {
                // �ٸ� �÷��̾� �ε� �� ������ ���
                Debug.Log($"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}");
                infoText.text = $"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            }
        }
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(RoomProp.LOAD_TIME))
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
        CharacterPropertyInstantiate();
    }

    IEnumerator DebugGameSetupDelay()
    {
        // �������� �����ð� 1�� ��ٷ��ֱ�
        yield return new WaitForSeconds(1f);
        DebugGameStart();
    }

    private void DebugGameStart()
    {
        // Team Property Test : �����δ� Room���� ������Ƽ ������
        string[] randoms = { "Red", "Yellow", "Orange", "Green", "Skyblue", "Blue", "Purple", "Pink" };

        SetPlayerTeamProperty(randoms[Random.Range(0,8)]);

        Vector3 position = playerSpawn.spawnPoints[PhotonNetwork.LocalPlayer.GetPlayerNumber()].transform.position;

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

    private void SetPlayerTeamProperty(string team)
    {
        PhotonHashtable property = PhotonNetwork.LocalPlayer.CustomProperties;
        property[PlayerProp.TEAM] = team;
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    }

    /// <summary>
    /// �濡�� ���õ� ĳ���ͷ� �����ɼ� �ְ� �Լ� �ۼ�
    /// </summary>
    private void CharacterPropertyInstantiate()
    {
        PhotonHashtable property = PhotonNetwork.LocalPlayer.CustomProperties;

        if (!property.ContainsKey(PlayerProp.CHARACTER))
        {
            Debug.Log("������Ƽ�� �����ϴ�");
        }

        Vector3 position = playerSpawn.spawnPoints[PhotonNetwork.LocalPlayer.GetPlayerNumber()].transform.position;

        switch ((CharacterEnum)property[PlayerProp.CHARACTER])
        {
            case CharacterEnum.Dao:
                PhotonNetwork.Instantiate("Prefabs/Dao", position, Quaternion.identity);
                Debug.Log("Dao Create");
                break;
            case CharacterEnum.Cappi:
                PhotonNetwork.Instantiate("Prefabs/Cappi", position, Quaternion.identity);
                Debug.Log("Cappi Create");
                break;
            case CharacterEnum.Marid:
                PhotonNetwork.Instantiate("Prefabs/Marid", position, Quaternion.identity);
                Debug.Log("Marid Create");
                break;
            case CharacterEnum.Bazzi:
                PhotonNetwork.Instantiate("Prefabs/Bazzi", position, Quaternion.identity);
                Debug.Log("Bazzi Create");
                break;
        }
    }
}
