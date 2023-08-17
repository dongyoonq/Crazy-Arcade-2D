using CustomProperty;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class EnteredGamePlayer : MonoBehaviourPunCallbacks
{
	[SerializeField]
	private List<Sprite> characterImgs;

	[SerializeField]
	private Image PlayerCharacter;

	[SerializeField]
	private Image PlayerLevel;

	[SerializeField]
	private TMP_Text NickName;

	[SerializeField]
	private Image TeamColor;

    private void Awake()
    {
        transform.SetParent(GameObject.Find("EnteredPlayerList").transform, false);
        SetCharacterImg();
        TeamColor.color = GetColorFromProperty();
        PlayerLevel.gameObject.SetActive(false);
        NickName.text = photonView.Owner.NickName;
    }

    private Color GetColorFromProperty()
    {
        PhotonHashtable property = photonView.Owner.CustomProperties;

        string hexColor = (string)property[PlayerProp.TEAMCOLOR];

        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogWarning("Invalid hex color string: " + hexColor);
            return Color.white; // 또는 다른 기본값
        }
    }

    private void SetCharacterImg()
    {
        PhotonHashtable property = photonView.Owner.CustomProperties;

        if (!property.ContainsKey(PlayerProp.CHARACTER))
        {
            Debug.Log("프로퍼티가 없습니다");
        }

        switch ((CharacterEnum)property[PlayerProp.CHARACTER])
        {
            case CharacterEnum.Dao:
                PlayerCharacter.sprite = characterImgs[0];
                break;
            case CharacterEnum.Cappi:
                PlayerCharacter.sprite = characterImgs[1];
                break;
            case CharacterEnum.Marid:
                PlayerCharacter.sprite = characterImgs[2];
                break;
            case CharacterEnum.Bazzi:
                PlayerCharacter.sprite = characterImgs[3];
                break;
        }
    }
}
