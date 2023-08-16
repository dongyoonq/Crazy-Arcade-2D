using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] Sprite winImg;
    [SerializeField] Sprite loseImg;
    [SerializeField] private Image resultImg;

    private Image characterImg;

    private Slider slider;

    [SerializeField] TMP_Text expText;
    [SerializeField] TMP_Text moneyText;

    private void Awake()
    {
        // resultImg
        // if 이겼으면 resultImg = winImg; else loseImg

        // 캐릭터이미지 룸패널이나 게임씬에서 가져와야할듯

        slider = GetComponentInChildren<Slider>();

        // expGet 계산

        // ***** ScoreRank에 ResultLine Prefab 생성 *****
    }
}
