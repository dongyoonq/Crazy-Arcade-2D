using CustomProperty;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class WaitingRoomEnter : MonoBehaviour
{
    public string selectedMode;

    public void RandomMatching()
    {
        PhotonHashtable expectedCustomRoomProperties;

        if (string.IsNullOrEmpty(selectedMode))
            expectedCustomRoomProperties = new PhotonHashtable { { RoomProp.ROOM_STATE, "Waiting" } };
        else
            expectedCustomRoomProperties = new PhotonHashtable { { RoomProp.ROOM_STATE, "Waiting" }, { RoomProp.ROOM_MODE, selectedMode } };

        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }
}
