using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelect : MonoBehaviour
{
    [SerializeField] private Button OneOnOneBtn;
    [SerializeField] private Button RandomBtn;

    [SerializeField] private Sprite OneactiveSprite; // 활성화 스프라이트
    [SerializeField] private Sprite OneinactiveSprite; // 비활성 스프라이트

    [SerializeField] private Sprite RanactiveSprite; // 활성화 스프라이트
    [SerializeField] private Sprite RaninactiveSprite; // 비활성 스프라이트

    private Button selectedButton; // 현재 선택된 버튼
    public string selectedMode;

    private void Start()
    {
        // 버튼 이미지를 비활성 스프라이트로 초기화
        OneOnOneBtn.image.sprite = OneinactiveSprite;
        RandomBtn.image.sprite = RaninactiveSprite;

        // 버튼에 클릭 이벤트 추가
        OneOnOneBtn.onClick.AddListener(() => SelectButton(OneOnOneBtn));
        RandomBtn.onClick.AddListener(() => SelectButton(RandomBtn));
    }

    private void SelectButton(Button button)
    {
        if (selectedButton != null)
        {
            // 이전에 선택된 버튼의 이미지를 비활성 스프라이트로 변경
            if(selectedButton == OneOnOneBtn)
            selectedButton.image.sprite = OneinactiveSprite;
            if(selectedButton== RandomBtn)
            selectedButton.image.sprite = RaninactiveSprite;
        }

        // 선택된 버튼을 현재 선택된 버튼으로 업데이트
        selectedButton = button;

        // 선택된 버튼의 이미지를 활성화 스프라이트로 변경
        if (selectedButton == OneOnOneBtn)
        {
            button.image.sprite = OneactiveSprite;
            selectedMode = "OneOnOne";
        }

        else
        {
            button.image.sprite = RanactiveSprite;
            selectedMode = "Random";
        }
    }
}
