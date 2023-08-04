using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace KDY
{
    public static class CustomProperty
    {
        private const string READY = "Ready";
        private const string LOAD = "Load";
        private const string SCORE = "Score";
        private const string NUMBER = "Number";

        private const string LOADTIME = "LoadTime";

        public static bool GetReady(this Player player)
        {
            PhotonHashtable property = player.CustomProperties;
            if (property.ContainsKey(READY))
                return (bool)property[READY];
            else
                return false;
        }

        public static void SetReady(this Player player, bool ready)
        {
            PhotonHashtable property = player.CustomProperties;
            property[READY] = ready;
            player.SetCustomProperties(property);
        }

        public static bool GetLoad(this Player player)
        {
            PhotonHashtable property = player.CustomProperties;
            if (property.ContainsKey(LOAD))
                return (bool)property[LOAD];
            else
                return false;
        }

        public static void SetLoad(this Player player, bool load)
        {
            PhotonHashtable property = player.CustomProperties;
            property[LOAD] = load;
            player.SetCustomProperties(property);
        }

        public static int GetLoadTime(this Room room)
        {
            PhotonHashtable property = room.CustomProperties;
            if (property.ContainsKey(LOADTIME))
                return (int)property[LOADTIME];
            else
                return -1;
        }

        public static void SetLoadTime(this Room room, int loadTime)
        {
            PhotonHashtable property = room.CustomProperties;
            property[LOADTIME] = loadTime;
            room.SetCustomProperties(property);
        }

        public static int GetScore(this Player player)
        {
            PhotonHashtable property = player.CustomProperties;
            if (property.ContainsKey(SCORE))
                return (int)property[SCORE];
            else
                return -1;
        }

        public static void SetScore(this Player player, int score)
        {
            PhotonHashtable property = player.CustomProperties;
            property[SCORE] = score;
            player.SetCustomProperties(property);
        }
    }
}