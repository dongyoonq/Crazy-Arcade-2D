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
        // if �̰����� resultImg = winImg; else loseImg

        // ĳ�����̹��� ���г��̳� ���Ӿ����� �����;��ҵ�

        slider = GetComponentInChildren<Slider>();

        // expGet ���

        // ***** ScoreRank�� ResultLine Prefab ���� *****
    }
}
