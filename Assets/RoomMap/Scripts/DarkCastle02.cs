using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkCastle02 : Map
{
    protected override void Awake()
    {
        base.Awake();

        data = Resources.Load<MapData>("Data/DarkCastle02Data");

        title = data.maps[0].title;
        maxPlayer = data.maps[0].maxPlayer;
        level = data.maps[0].level;
        popularity = data.maps[0].popularity;
        rank = data.maps[0].rank;
        info = data.maps[0].info;

        favorites = data.maps[0].favorites;

        mapImg = data.maps[0].mapImg;
        star1 = data.maps[0].star1;
        star2 = data.maps[0].star2;
    }
}
