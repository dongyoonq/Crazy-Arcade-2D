using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseMap : MonoBehaviour
{
    [SerializeField] MapList mapList;

    [SerializeField] RectTransform mapContent;

    [SerializeField] MapEntry mapEntryPrefab;

    [SerializeField] TMP_Text mapTitle;
    [SerializeField] TMP_Text maxPlayer;
    [SerializeField] TMP_Text rank;
    [SerializeField] TMP_Text mapInfo;

    [SerializeField] Image mapImg;
    [SerializeField] Image levelImg;
    
    [SerializeField] Image popularityImg1;
    [SerializeField] Image popularityImg2;
    [SerializeField] Image popularityImg3;
    [SerializeField] Image popularityImg4;
    [SerializeField] Image popularityImg5;

    public Map prevMap;
    public Map curChoosedMap;
    public Map curMap;

    private void Awake()
    {
        foreach (Map maps in mapList.maps)
        {
            MapEntry entry = Instantiate(mapEntryPrefab, mapContent);
            entry.SetMapInfo(maps);
        }

        curChoosedMap = mapList.maps[0];
        OnMapChoosed();
    }

    private void OnEnable()
    {
        prevMap = curMap;
        Debug.Log("curMap = {0}", curMap);
    }

    public void OnMapChoosed()
    {
        mapTitle.text = curChoosedMap.title;
        maxPlayer.text = curChoosedMap.maxPlayer.ToString();
        rank.text = curChoosedMap.rank.ToString();
        mapInfo.text = curChoosedMap.info;
        mapImg.sprite = curChoosedMap.mapImg;

        if (curChoosedMap.popularity == 1)
        {
            popularityImg1.sprite = curChoosedMap.star1;
            popularityImg2.sprite = curChoosedMap.star2;
            popularityImg3.sprite = curChoosedMap.star2;
            popularityImg4.sprite = curChoosedMap.star2;
            popularityImg5.sprite = curChoosedMap.star2;
        }
        else if (curChoosedMap.popularity == 2)
        {
            popularityImg1.sprite = curChoosedMap.star1;
            popularityImg2.sprite = curChoosedMap.star1;
            popularityImg3.sprite = curChoosedMap.star2;
            popularityImg4.sprite = curChoosedMap.star2;
            popularityImg5.sprite = curChoosedMap.star2;
        }
        else if (curChoosedMap.popularity == 3)
        {
            popularityImg1.sprite = curChoosedMap.star1;
            popularityImg2.sprite = curChoosedMap.star1;
            popularityImg3.sprite = curChoosedMap.star1;
            popularityImg4.sprite = curChoosedMap.star2;
            popularityImg5.sprite = curChoosedMap.star2;
        }
        else if (curChoosedMap.popularity == 4)
        {
            popularityImg1.sprite = curChoosedMap.star1;
            popularityImg2.sprite = curChoosedMap.star1;
            popularityImg3.sprite = curChoosedMap.star1;
            popularityImg4.sprite = curChoosedMap.star1;
            popularityImg5.sprite = curChoosedMap.star2;
        }
        else if (curChoosedMap.popularity == 5)
        {
            popularityImg1.sprite = curChoosedMap.star1;
            popularityImg2.sprite = curChoosedMap.star1;
            popularityImg3.sprite = curChoosedMap.star1;
            popularityImg4.sprite = curChoosedMap.star1;
            popularityImg5.sprite = curChoosedMap.star1;
        }
    }

    public Map OnOkButtonClicked()
    {
        curMap = curChoosedMap;
        Debug.Log("curMap = {0}", curMap);
        return curMap;
    }

    public void OnCancelButtonClicked()
    {
        curMap = prevMap;
        this.gameObject.SetActive(false);
        Debug.Log("curMap = {0}", curMap);
    }
}
