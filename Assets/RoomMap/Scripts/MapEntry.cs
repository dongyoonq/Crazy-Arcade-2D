using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapEntry : MonoBehaviour
{
    public ChooseMap chooseMap;

    public MapList maplist;

    public TMP_Text title;
    public TMP_Text maxPlayer;
    public TMP_Text level;
    public Toggle favoritesCheck;

    private Map map;
    protected virtual void Awake()
    {
        chooseMap = GetComponentInParent<ChooseMap>();
        maplist = GetComponentInParent<MapList>();
    }

    public void SetMapInfo(Map mapInfo)
    {
        map = mapInfo;
        title.text = mapInfo.title;
        maxPlayer.text = mapInfo.maxPlayer.ToString();
        level.text = mapInfo.level.ToString();
    }

    public void OnChooseMapClicked()
    {
        chooseMap.curChoosedMap = this.map;
        chooseMap.OnMapChoosed();
    }
}
