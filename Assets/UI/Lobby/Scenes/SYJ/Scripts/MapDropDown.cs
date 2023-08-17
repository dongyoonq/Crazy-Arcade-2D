using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapDropDown : MonoBehaviour
{
    [SerializeField] Sprite[] spriteList;
    [SerializeField] Button confirmBtn;
    [SerializeField] RectTransform RandomImg;
    [SerializeField] RectTransform KfcImg;
    [SerializeField] RectTransform CookieImg;
    [SerializeField] RectTransform PatritImg;
    public enum Map { Random, Kfc, Cookie, Patrit }


    TMP_Dropdown dropDown; // TMP_Dropdown 컴포넌트를 저장하기 위한 변수
    Image[] optionList; // 드롭다운 옵션 이미지를 저장하기 위한 배열
    private bool isActive;

    Map selectedMap;

    [SerializeField] GameObject[] gameObjectsToActivate; // 각 드롭다운 옵션에 대응하는 게임 오브젝트들

    private void Start()
    {
        dropDown = GetComponent<TMP_Dropdown>(); // 현재 GameObject에 붙어있는 TMP_Dropdown 컴포넌트 가져오기
        isActive = false; // 드롭다운이 확장되었는지 여부를 나타내는 변수 초기화
        confirmBtn.onClick.AddListener(() => SetActiveImage());
    }

    private void Update()
    {
        if (dropDown.IsExpanded && !isActive)
        {
            // 드롭다운이 확장되었을 때 한 번만 실행됨
            optionList = GameObject.Find("Dropdown List").transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>();

            for (int i = 0; i < optionList.Length; i++)
            {
                optionList[i].sprite = spriteList[i];
            }

            optionList[0].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OptionOneClicked(x, optionList[0]));
            optionList[1].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OptionTwoClicked(x, optionList[1]));
            optionList[2].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OptionThreeClicked(x, optionList[2]));
            optionList[3].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OptionFourClicked(x, optionList[3]));

            isActive = true; // 활성화 상태로 변경
        }
        else if (!dropDown.IsExpanded)
        {
            isActive = false; // 드롭다운이 닫혔을 때 비활성화 상태로 변경
        }
    }

    // 드롭다운 옵션 선택 시 호출될 함수
    public void OnDropdownOptionSelected()
    {
        int selectedOptionIndex = dropDown.value; // 선택한 옵션의 인덱스 가져오기

        // 모든 게임 오브젝트 비활성화
        foreach (GameObject obj in gameObjectsToActivate)
        {
            obj.SetActive(false);
        }

        // 선택한 드롭다운 옵션에 대응하는 게임 오브젝트 활성화
        if (selectedOptionIndex >= 0 && selectedOptionIndex < gameObjectsToActivate.Length)
        {
            gameObjectsToActivate[selectedOptionIndex].SetActive(true);
        }
    }

    // 캐릭터가 일치할 경우 자동으로 활성화되도록 설정
    private void SetActiveImage()
    {
        RandomImg.gameObject.SetActive(selectedMap == Map.Random);
        KfcImg.gameObject.SetActive(selectedMap == Map.Kfc);
        CookieImg.gameObject.SetActive(selectedMap == Map.Cookie);
        PatritImg.gameObject.SetActive(selectedMap == Map.Patrit);
    }

    public void Confirm()
    {
        // 선택한 맵 정보에 따른 처리
        switch (selectedMap)
        {
            case Map.Kfc:

                break;
            case Map.Cookie:

                break;
            case Map.Patrit:

                break;
                // 다른 맵 정보에 대한 처리도 추가
        }
    }

    public void Cancel()
    {

    }

    private void OptionOneClicked(bool select, Image image)
    {
        if (select)
        {
            GetComponent<Image>().sprite = image.sprite;
            selectedMap = Map.Random;
        }
    }

    private void OptionTwoClicked(bool select, Image image)
    {
        if (select)
        {
            GetComponent<Image>().sprite = image.sprite;
            selectedMap = Map.Kfc;
        }
    }

    private void OptionThreeClicked(bool select, Image image)
    {
        if (select)
        {
            GetComponent<Image>().sprite = image.sprite;
            selectedMap = Map.Cookie;
        }
    }

    private void OptionFourClicked(bool select, Image image)
    {
        if (select)
        {
            GetComponent<Image>().sprite = image.sprite;
            selectedMap = Map.Patrit;
        }
    }

    private void SetInactiveObject(Map map)
    {
        // 선택한 맵에 따른 게임 오브젝트 비활성화
        gameObjectsToActivate[(int)map].SetActive(false);
    }
}

