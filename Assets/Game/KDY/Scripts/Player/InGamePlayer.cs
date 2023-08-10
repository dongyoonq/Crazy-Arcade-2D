using CustomProperty;
using Photon.Pun;
using RoomUI.ChooseTeam;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace KDY
{
    public class InGamePlayer : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] TMP_Text nameTxt;

        public TEAM currTeam;
        public List<Item> itemLists;

        private string playerName;
        private Color teamColor;

        private void Awake()
        {
            currTeam = GetTeamFromProperty();
            nameTxt.color = GetColorFromProperty();

            if (photonView.IsMine)
            {
                playerName = photonView.Owner.NickName;
                nameTxt.text = playerName;
            }
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                nameTxt.text = playerName;
            }
        }

        private Color GetColorFromProperty()
        {
            PhotonHashtable property = photonView.Owner.CustomProperties;

            string hexColor = (string)property[PlayerProp.TEAMCOLOR];

            if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            {
                teamColor = color;
                return color;
            }
            else
            {
                Debug.LogWarning("Invalid hex color string: " + hexColor);
                teamColor = Color.white;
                return Color.white; // 또는 다른 기본값
            }
        }

        
        private TEAM GetTeamFromProperty()
        {
            PhotonHashtable property = photonView.Owner.CustomProperties;

            if (!property.ContainsKey(PlayerProp.TEAM))
            {
                Debug.Log("프로퍼티가 없습니다");
                return TEAM.NONE;
            }

            switch ((string)property[PlayerProp.TEAM])
            {
                case "Red":
                    return TEAM.RED;
                case "Yellow":
                    return TEAM.YELLOW;
                case "Orange":
                    return TEAM.ORANGE;
                case "Green":
                    return TEAM.GREEN;
                case "Skyblue":
                    return TEAM.SKY;
                case "Blue":
                    return TEAM.BLUE;
                case "Purple":
                    return TEAM.PURPLE;
                case "Pink":
                    return TEAM.PINK;
                default:
                    Debug.Log("선택된 팀이 없습니다.");
                    return TEAM.NONE;
            }
        }

        private void SetTeamColor()
        {
            switch (currTeam)
            {
                case TEAM.RED:
                    teamColor = Color.red;
                    break;
                case TEAM.YELLOW:
                    teamColor = Color.yellow;
                    break;
                case TEAM.ORANGE:
                    teamColor = new Color(1, 0.5f, 0, 1);
                    break;
                case TEAM.GREEN:
                    teamColor = Color.green;
                    break;
                case TEAM.SKY:
                    teamColor = new Color(0, 1, 1, 1);
                    break;
                case TEAM.BLUE:
                    teamColor = Color.blue;
                    break;
                case TEAM.PURPLE:
                    teamColor = new Color(0.5f, 0, 1, 1);
                    break;
                case TEAM.PINK:
                    teamColor = Color.magenta;
                    break;
                default:
                    Debug.Log("선택된 팀이 없습니다.");
                    break;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(playerName);
                stream.SendNext(currTeam);
            }
            else
            {
                playerName = (string)stream.ReceiveNext();
                currTeam = (TEAM)stream.ReceiveNext();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("WaterBlock"))
            {
                Debug.Log("플레이어 물감옥");

                //Todo : 플레이어 물감옥 상태 처리
            }
        }
    }
}
