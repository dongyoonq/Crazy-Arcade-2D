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


    TMP_Dropdown dropDown; // TMP_Dropdown ������Ʈ�� �����ϱ� ���� ����
    Image[] optionList; // ��Ӵٿ� �ɼ� �̹����� �����ϱ� ���� �迭
    private bool isActive;

    Map selectedMap;

    [SerializeField] GameObject[] gameObjectsToActivate; // �� ��Ӵٿ� �ɼǿ� �����ϴ� ���� ������Ʈ��

    private void Start()
    {
        dropDown = GetComponent<TMP_Dropdown>(); // ���� GameObject�� �پ��ִ� TMP_Dropdown ������Ʈ ��������
        isActive = false; // ��Ӵٿ��� Ȯ��Ǿ����� ���θ� ��Ÿ���� ���� �ʱ�ȭ
        confirmBtn.onClick.AddListener(() => SetActiveImage());
    }

    private void Update()
    {
        if (dropDown.IsExpanded && !isActive)
        {
            // ��Ӵٿ��� Ȯ��Ǿ��� �� �� ���� �����
            optionList = GameObject.Find("Dropdown List").transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>();

            for (int i = 0; i < optionList.Length; i++)
            {
                optionList[i].sprite = spriteList[i];
            }

            optionList[0].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OptionOneClicked(x, optionList[0]));
            optionList[1].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OptionTwoClicked(x, optionList[1]));
            optionList[2].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OptionThreeClicked(x, optionList[2]));
            optionList[3].transform.parent.GetComponent<Toggle>().onValueChanged.AddListener(x => OptionFourClicked(x, optionList[3]));

            isActive = true; // Ȱ��ȭ ���·� ����
        }
        else if (!dropDown.IsExpanded)
        {
            isActive = false; // ��Ӵٿ��� ������ �� ��Ȱ��ȭ ���·� ����
        }
    }

    // ��Ӵٿ� �ɼ� ���� �� ȣ��� �Լ�
    public void OnDropdownOptionSelected()
    {
        int selectedOptionIndex = dropDown.value; // ������ �ɼ��� �ε��� ��������

        // ��� ���� ������Ʈ ��Ȱ��ȭ
        foreach (GameObject obj in gameObjectsToActivate)
        {
            obj.SetActive(false);
        }

        // ������ ��Ӵٿ� �ɼǿ� �����ϴ� ���� ������Ʈ Ȱ��ȭ
        if (selectedOptionIndex >= 0 && selectedOptionIndex < gameObjectsToActivate.Length)
        {
            gameObjectsToActivate[selectedOptionIndex].SetActive(true);
        }
    }

    // ĳ���Ͱ� ��ġ�� ��� �ڵ����� Ȱ��ȭ�ǵ��� ����
    private void SetActiveImage()
    {
        RandomImg.gameObject.SetActive(selectedMap == Map.Random);
        KfcImg.gameObject.SetActive(selectedMap == Map.Kfc);
        CookieImg.gameObject.SetActive(selectedMap == Map.Cookie);
        PatritImg.gameObject.SetActive(selectedMap == Map.Patrit);
    }

    public void Confirm()
    {
        // ������ �� ������ ���� ó��
        switch (selectedMap)
        {
            case Map.Kfc:

                break;
            case Map.Cookie:

                break;
            case Map.Patrit:

                break;
                // �ٸ� �� ������ ���� ó���� �߰�
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
        // ������ �ʿ� ���� ���� ������Ʈ ��Ȱ��ȭ
        gameObjectsToActivate[(int)map].SetActive(false);
    }
}

