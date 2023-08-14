using CustomProperty;
using Photon.Pun;
using RoomUI.ChooseTeam;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static Extension;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

namespace KDY
{
    public class InGamePlayer : MonoBehaviourPun, IPunObservable
    {
        public const int limitbombPower = 8;
        public const int limitbombCount = 8;
        public const float limitmoveSpeed = 4.2f;

        public int bombPower = 1;   // ��ǳ�� ����
        public int maxbombCount;    // �ִ� ��ǳ�� ��ġ ����
        public int currbombCount;   // ���� ��ǳ�� ��ġ ��

        [SerializeField] public float moveSpeed;
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] float dieTimer;
        [SerializeField] PlayerHitBox hitBox;

        public TEAM currTeam;
        public bool isPrision;
        public float prevMoveSpeed;
        public Animator animator;

        private string playerName;

        private void Awake()
        {
            hitBox.owner = this;
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
            if (collision.gameObject.layer == LayerMask.NameToLayer("WaterBlock") && !isPrision)
            {
                isPrision = true;
                animator.SetBool("BeforeDie", true);
                prevMoveSpeed = moveSpeed;
                moveSpeed = 0.5f;
                hitBox.gameObject.SetActive(true);
            }
        }

        public void OnBeforeDieAnimationFinish()
        {
            animator.SetBool("BeforeDie", false);
            animator.SetBool("Die", true);

            if (photonView.IsMine)
                GetComponent<PlayerInput>().enabled = false;
        }

        public void OnDieAnimationFinish()
        {
            animator.SetBool("Die", false);
            animator.SetBool("Died", true);
            StartCoroutine(DissapearRoutime());
        }

        public void OnRescueAnimationFinish()
        {
            animator.SetBool("Rescue", false);
        }

        public IEnumerator DissapearRoutime()
        {
            yield return new WaitForSeconds(1f);

            animator.SetBool("Died", false);
            animator.SetBool("Dissapear", true);

            yield return new WaitForSeconds(3f);

            Destroy(gameObject);
        }
    }
}
