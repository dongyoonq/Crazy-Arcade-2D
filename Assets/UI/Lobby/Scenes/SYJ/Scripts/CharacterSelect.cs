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
        // 랜덤 버튼이 선택된 상태로 시작
    }

    private void OnEnable()
    {
        checkButton.onClick.AddListener(Confirm);
    }

    // 캐릭터의 이름 부여
    public void SetCharacter(string name)
    {
        character = name;
    }

    // 확인버튼 선택 시 이전 창으로 돌아가며 선택한 정보가 저장됨
    public void Confirm()
    {
        // Confirm 버튼을 눌렀을 때의 동작
        DeactivateCharacterImage();
        SetActiveImage();
    }

    // 캐릭터가 일치할 경우 자동으로 활성화되도록 설정
    private void SetActiveImage()
    {
        RandomImg.gameObject.SetActive(selectedCharacter == Character.Random);
        DaoImg.gameObject.SetActive(selectedCharacter == Character.Dao);
        MaridImg.gameObject.SetActive(selectedCharacter == Character.Marid);
        BazziImg.gameObject.SetActive(selectedCharacter == Character.Bazzi);
        KeppiImg.gameObject.SetActive(selectedCharacter == Character.Keppi);
    }

    // 선택되지 않은 캐릭터의 이미지 비활성화
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