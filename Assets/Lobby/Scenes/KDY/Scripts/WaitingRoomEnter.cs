using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class WaitingRoomEnter : MonoBehaviour
{
    private const string MAP_PROP_KEY = "Map";
    private const string MODE_PROP_KEY = "Mode";
    private const string STATE_PROP_KEY = "RoomState";

    public string selectedMode;

    public void RandomMatching()
    {
        PhotonHashtable expectedCustomRoomProperties;

        if (string.IsNullOrEmpty(selectedMode))
            expectedCustomRoomProperties = new PhotonHashtable { { STATE_PROP_KEY, "Waiting" } };
        else
            expectedCustomRoomProperties = new PhotonHashtable { { STATE_PROP_KEY, "Waiting" }, { MODE_PROP_KEY, selectedMode } };

        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }
}
