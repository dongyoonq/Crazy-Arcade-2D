using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelect : MonoBehaviour
{
    [SerializeField] private Button OneOnOneBtn;
    [SerializeField] private Button RandomBtn;

    [SerializeField] private Sprite OneactiveSprite; // Ȱ��ȭ ��������Ʈ
    [SerializeField] private Sprite OneinactiveSprite; // ��Ȱ�� ��������Ʈ

    [SerializeField] private Sprite RanactiveSprite; // Ȱ��ȭ ��������Ʈ
    [SerializeField] private Sprite RaninactiveSprite; // ��Ȱ�� ��������Ʈ

    private Button selectedButton; // ���� ���õ� ��ư
    public string selectedMode;

    private void Start()
    {
        // ��ư �̹����� ��Ȱ�� ��������Ʈ�� �ʱ�ȭ
        OneOnOneBtn.image.sprite = OneinactiveSprite;
        RandomBtn.image.sprite = RaninactiveSprite;

        // ��ư�� Ŭ�� �̺�Ʈ �߰�
        OneOnOneBtn.onClick.AddListener(() => SelectButton(OneOnOneBtn));
        RandomBtn.onClick.AddListener(() => SelectButton(RandomBtn));
    }

    private void SelectButton(Button button)
    {
        if (selectedButton != null)
        {
            // ������ ���õ� ��ư�� �̹����� ��Ȱ�� ��������Ʈ�� ����
            if(selectedButton == OneOnOneBtn)
            selectedButton.image.sprite = OneinactiveSprite;
            if(selectedButton== RandomBtn)
            selectedButton.image.sprite = RaninactiveSprite;
        }

        // ���õ� ��ư�� ���� ���õ� ��ư���� ������Ʈ
        selectedButton = button;

        // ���õ� ��ư�� �̹����� Ȱ��ȭ ��������Ʈ�� ����
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
