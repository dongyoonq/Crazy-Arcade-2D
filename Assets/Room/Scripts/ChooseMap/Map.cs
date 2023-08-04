using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoomUI.ChooseMap
{
    public class Map : MonoBehaviour
    {
        protected MapData data;

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

        protected virtual void Awake()
        {

        }
    }
}