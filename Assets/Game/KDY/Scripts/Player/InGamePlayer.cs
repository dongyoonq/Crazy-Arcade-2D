using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace KDY
{
    public class InGamePlayer : MonoBehaviourPun, IPunObservable
    {
        enum TEAM { RED, YELLOW, ORANGE, GREEN, SKY, BLUE, PURPLE, MAGENTA, NONE }

        [SerializeField] TMP_Text nameTxt;

        private string playerName;
        private TEAM currTeam;
        private Color teamColor;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                playerName = photonView.Owner.NickName;
                nameTxt.text = playerName;
                currTeam = GetTeamFromProperty();
                Debug.Log(currTeam.ToString());
                nameTxt.color = teamColor;
            }
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                nameTxt.text = playerName;
                SetTeamColor();
                nameTxt.color = teamColor;
            }
        }

        private TEAM GetTeamFromProperty()
        {
            PhotonHashtable property = photonView.Owner.CustomProperties;

            if (!property.ContainsKey("Team"))
            {
                Debug.Log("프로퍼티가 없습니다");
                return TEAM.NONE;
            }

            switch ((string)property["Team"])
            {
                case "RED":
                    teamColor = Color.red;
                    return TEAM.RED;
                case "YELLOW":
                    teamColor = Color.yellow;
                    return TEAM.YELLOW;
                case "ORANGE":
                    teamColor = new Color(1, 0.5f, 0, 1);
                    return TEAM.ORANGE;
                case "GREEN":
                    teamColor = Color.green;
                    return TEAM.GREEN;
                case "SKY":
                    teamColor = new Color(1, 1, 1, 1);
                    return TEAM.SKY;
                case "BLUE":
                    teamColor = Color.blue;
                    return TEAM.BLUE;
                case "PURPLE":
                    teamColor = new Color(0.5f, 0, 1, 1);
                    return TEAM.PURPLE;
                case "MAGENTA":
                    teamColor = Color.magenta;
                    return TEAM.MAGENTA;
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
                case TEAM.MAGENTA:
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
    }
}
