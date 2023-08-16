using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] List<CharacterImage> imageLists = new List<CharacterImage>();

    public enum Character { Random, Dao, Marid, Bazzi, Keppi }
    private bool selected;

    [SerializeField] Button checkButton;
    [SerializeField] Image RandomImg;
    [SerializeField] Image DaoImg;
    [SerializeField] Image MaridImg;
    [SerializeField] Image BazziImg;
    [SerializeField] Image KeppiImg;

    Character selectedCharacter;

    public string character;

    private void Awake()
    {
        // ���� ��ư�� ���õ� ���·� ����
    }

    private void OnEnable()
    {
        checkButton.onClick.AddListener(Confirm);
    }

    // ĳ������ �̸� �ο�
    public void SetCharacter(string name)
    {
        character = name;
    }

    // Ȯ�ι�ư ���� �� ���� â���� ���ư��� ������ ������ �����
    public void Confirm()
    {
        // Confirm ��ư�� ������ ���� ����
        DeactivateCharacterImage();
        SetActiveImage();
    }

    // ĳ���Ͱ� ��ġ�� ��� �ڵ����� Ȱ��ȭ�ǵ��� ����
    private void SetActiveImage()
    {
        RandomImg.gameObject.SetActive(selectedCharacter == Character.Random);
        DaoImg.gameObject.SetActive(selectedCharacter == Character.Dao);
        MaridImg.gameObject.SetActive(selectedCharacter == Character.Marid);
        BazziImg.gameObject.SetActive(selectedCharacter == Character.Bazzi);
        KeppiImg.gameObject.SetActive(selectedCharacter == Character.Keppi);
    }

    // ���õ��� ���� ĳ������ �̹��� ��Ȱ��ȭ
    private void DeactivateCharacterImage()
    {
        switch (character)
        {
            case "Random":
                selectedCharacter = Character.Random;
                break;
            case "Dao":
                selectedCharacter = Character.Dao;
                break;
            case "Marid":
                selectedCharacter = Character.Marid;
                break;
            case "Bazzi":
                selectedCharacter = Character.Bazzi;
                break;
            case "Keppi":
                selectedCharacter = Character.Keppi;
                break;
        }
    }

    public void OnClickCharacter()
    {
        GameManager.Sound.Onclick();
        foreach (CharacterImage image in imageLists)
        {
            if (image.isActive)
            {
                image.isActive = false;
                image.gameObject.SetActive(image.isActive);
            }
        }

        
    }
}