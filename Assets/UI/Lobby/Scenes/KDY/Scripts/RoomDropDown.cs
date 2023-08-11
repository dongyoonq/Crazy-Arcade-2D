using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomDropDown : MonoBehaviour
{
    [SerializeField] Image selectImg;
    [SerializeField] WaitingRoomEnter waitingRoomEnter;
    TMP_Dropdown dropDown;

    Image[] optionLists;

    bool isActive;

    private void Start()
    {
        dropDown = GetComponent<TMP_Dropdown>();
    }

    private void Update()
    {
        if (dropDown.IsExpanded && !isActive)
        {
            Debug.Log(GameObject.Find("Dropdown List").name);
            optionLists = GameObject.Find("Dropdown List").transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>();
            Sprite[] sprites = Resources.LoadAll<Sprite>("드롭다운");
            optionLists[0].sprite = sprites[1];
            optionLists[0].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OnSelectedOption_1(x));
            optionLists[1].sprite = sprites[4];
            optionLists[1].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OnSelectedOption_2(x));
            optionLists[2].sprite = sprites[6];
            optionLists[2].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OnSelectedOption_3(x));
            optionLists[3].sprite = sprites[8];
            optionLists[3].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OnSelectedOption_4(x));

            isActive = true;
        }
        else if (!dropDown.IsExpanded)
        {
            isActive = false;
        }
    }

    private void OnSelectedOption_1(bool selected)
    {
        if (selected)
        {
            selectImg.sprite = Resources.LoadAll<Sprite>("드롭다운")[0];
            waitingRoomEnter.selectedMode = "";
        }
    }

    private void OnSelectedOption_2(bool selected)
    {
        if (selected)
        {
            selectImg.sprite = Resources.LoadAll<Sprite>("드롭다운")[3];
            waitingRoomEnter.selectedMode = "Normal";
        }
    }

    private void OnSelectedOption_3(bool selected)
    {
        if (selected)
        {
            selectImg.sprite = Resources.LoadAll<Sprite>("드롭다운")[5];
            waitingRoomEnter.selectedMode = "Monster";
        }
    }

    private void OnSelectedOption_4(bool selected)
    {
        if (selected)
        {
            selectImg.sprite = Resources.LoadAll<Sprite>("드롭다운")[7];
            waitingRoomEnter.selectedMode = "Pincers";
        }
    }
}
