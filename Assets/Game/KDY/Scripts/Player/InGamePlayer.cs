using CustomProperty;
using Photon.Pun;
using RoomUI.ChooseTeam;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace KDY
{
    public class InGamePlayer : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] public float moveSpeed;
        public int bombPower = 1;

        [SerializeField] TMP_Text nameTxt;
        [SerializeField] float dieTimer;

        public TEAM currTeam;

        private string playerName;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();

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
                return color;
            }
            else
            {
                Debug.LogWarning("Invalid hex color string: " + hexColor);
                return Color.white; // �Ǵ� �ٸ� �⺻��
            }
        }


        private TEAM GetTeamFromProperty()
        {
            PhotonHashtable property = photonView.Owner.CustomProperties;

            if (!property.ContainsKey(PlayerProp.TEAM))
            {
                Debug.Log("������Ƽ�� �����ϴ�");
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
                    Debug.Log("���õ� ���� �����ϴ�.");
                    return TEAM.NONE;
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
                //Todo : �÷��̾� ������ ���� ó��
                animator.SetBool("BeforeDie", true);
                moveSpeed = 0.5f;

                // BeforeDie �ִϸ��̼� �� �÷��̾ ��ġ�� ó��
            }
        }

        public void OnBeforeDieAnimationFinish()
        {
            animator.SetBool("BeforeDie", false);
            animator.SetBool("Die", true);
        }

        public void OnDieAnimationFinish()
        {
            animator.SetBool("Die", false);
            animator.SetBool("Died", true);

            if (photonView.IsMine)
                GetComponent<PlayerInput>().enabled = false;
        }
    }
}
