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
using KDY;
using UnityEngine.Events;
using System.IO;
using static Photon.Pun.UtilityScripts.PunTeams;

public class InGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float countdownTimer;
    [SerializeField] PlayerSpawn playerSpawn;

    public Dictionary<string, TEAM> teamPlayerDic = new Dictionary<string, TEAM>();
    public List<InGamePlayer> playerList = new List<InGamePlayer>();

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

        Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerNumber());
        Vector3 position = playerSpawn.spawnPoints[PhotonNetwork.LocalPlayer.GetPlayerNumber()].transform.position;

        // Debug Character
        PhotonNetwork.Instantiate("Prefabs/Marid", position, Quaternion.identity); 
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

    public void AddPlayerTeamList(InGamePlayer teamPlayer)
    {
        photonView.RPC("SyncAllAddTeamList", RpcTarget.All, SerializePlayerData(teamPlayer));
    }

    public void RemovePlayerTeamList(InGamePlayer teamPlayer)
    {
        photonView.RPC("SyncAllRemoveTeamList", RpcTarget.All, SerializePlayerData(teamPlayer));
    }

    public void CheckGameState()
    {
        photonView.RPC("SyncCheckGameState", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void SyncAllAddTeamList(byte[] serializedData)
    {
        InGamePlayer receivedData = DeserializePlayerData(serializedData);
        teamPlayerDic.Add(receivedData.playerName, receivedData.currTeam);
        playerList.Add(receivedData);
    }

    [PunRPC]
    private void SyncAllRemoveTeamList(byte[] serializedData)
    {
        InGamePlayer receivedData = DeserializePlayerData(serializedData);
        //teamPlayerDic.Remove(teamPlayerDic.Find(x => x.playerName == receivedData.playerName));

        foreach (string player in teamPlayerDic.Keys)
        {
            if (player == receivedData.playerName)
            {
                Debug.Log($"{player} ����Ʈ ����");
                teamPlayerDic.Remove(player);
                break;
            }
        }
    }

    public byte[] SerializePlayerData(InGamePlayer data)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write((int)data.currTeam);
                writer.Write(data.playerName);
            }
            return stream.ToArray();
        }
    }

    public InGamePlayer DeserializePlayerData(byte[] data)
    {
        InGamePlayer result = new InGamePlayer();
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                result.currTeam = (TEAM)reader.ReadInt32();
                result.playerName = reader.ReadString();
            }
        }
        return result;
    }

    [PunRPC]
    public void SyncCheckGameState()
    {
        /*
         * 		Manner = 0,
         * 		Free = 1,
         * 		Random = 2
        */
        foreach (KeyValuePair<string, TEAM> player in teamPlayerDic)
        {
            Debug.Log($"{player.Key} : {player.Value} ����");
        }

        PhotonHashtable property = PhotonNetwork.CurrentRoom.CustomProperties;

        Dictionary<TEAM, int> aliveTeams = new Dictionary<TEAM, int>();

        // �����ϴ� ������ ����
        foreach (KeyValuePair<string, TEAM> player in teamPlayerDic)
        {
            if (!aliveTeams.ContainsKey(player.Value))
                aliveTeams.Add(player.Value, 0);

            aliveTeams[player.Value] += 1;
        }

        // ���� �ϳ��� ����������
        if (aliveTeams.Count == 1)
        {
            foreach (TEAM team in aliveTeams.Keys)
            {
                // ��� ǥ��
                ShowGameResult(team);
                break;
            }
        }

        /*
        // MannerMode Check
        if ((RoomMode)property[RoomProp.ROOM_MODE] == RoomMode.Manner)
        {

        }
        else if ((RoomMode)property[RoomProp.ROOM_MODE] == RoomMode.Free)
        {

        }
        */
    }

    private void ShowGameResult(TEAM winTeam)
    {
        Debug.Log($"{winTeam} �� �¸�");

        List<string> winPlayerList = new List<string>();
        List<string> losePlayerList = new List<string>();

        foreach (InGamePlayer player in playerList)
        {
            if (player.currTeam == winTeam)
            {
                winPlayerList.Add(player.playerName);
            }
            else
            {
                losePlayerList.Add(player.playerName);
            }
        }

        Debug.Log("�¸� ����");
        foreach (string player in winPlayerList) { Debug.Log(player); }
        Debug.Log("�й� ����");
        foreach (string player in losePlayerList) { Debug.Log(player); }

        // ���â ǥ��
        // ���� ����
    }
}