using CustomProperty;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class ResultScreen : MonoBehaviour
{
    [SerializeField] List<Sprite> characterImgs;
    [SerializeField] Sprite winImg;
    [SerializeField] Sprite loseImg;
    [SerializeField] Image resultImg;
    [SerializeField] public RectTransform content;

    [SerializeField] private Image characterImg;

    private Slider slider;

    [SerializeField] TMP_Text expText;
    [SerializeField] TMP_Text moneyText;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void SetCharacterImg(Player player)
    {
        PhotonHashtable property = player.CustomProperties;

        switch ((CharacterEnum)property[PlayerProp.CHARACTER])
        {
            case CharacterEnum.Dao:
                characterImg.sprite = characterImgs[0];
                break;
            case CharacterEnum.Cappi:
                characterImg.sprite = characterImgs[1];
                break;
            case CharacterEnum.Marid:
                characterImg.sprite = characterImgs[2];
                break;
            case CharacterEnum.Bazzi:
                characterImg.sprite = characterImgs[3];
                break;
        }
    }

    public void SetResultImg(string result)
    {
        resultImg.sprite = (result == "Win") ? winImg : loseImg;
    }

    public void SetExpMoney(float exp, float money)
    {
        expText.text = $"{exp.ToString()}%";
        slider.value = exp;
        moneyText.text = money.ToString();
    }    
}
