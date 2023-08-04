using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI.ChooseMap
{
    [CreateAssetMenu(fileName = "MapData", menuName = "Data/Map")]
    public class MapData : ScriptableObject
    {
        public MapInfo[] maps;

        [Serializable]
        public class MapInfo
        {
            public Map map;

            public string title;
            public int maxPlayer;
            public int level;
            public int popularity;
            public int rank;
            public string info;

            public bool favorites;

            public Sprite mapImg;

            public Sprite star1;
            public Sprite star2;
        }
    }
}