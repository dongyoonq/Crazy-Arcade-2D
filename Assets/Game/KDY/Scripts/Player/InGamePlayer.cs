using CustomProperty;
using Photon.Pun;
using RoomUI.ChooseTeam;
using System;
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

        public int bombPower = 1;   // 물풍선 세기
        public int maxbombCount;    // 최대 물풍선 설치 개수
        public int currbombCount;   // 현재 물풍선 설치 수

        [SerializeField] public float moveSpeed;
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] float dieTimer;
        [SerializeField] PlayerHitBox hitBox;

        InGameManager gameManager;

        public TEAM currTeam;
        public bool isPrision;
        public float prevMoveSpeed;
        public string playerName;
        public Animator animator;

        private void Awake()
        {
            hitBox.owner = this;
            animator = GetComponent<Animator>();
            currTeam = GetTeamFromProperty();
            nameTxt.color = GetColorFromProperty();
            gameManager = GameObject.Find("InGameManager").GetComponent<InGameManager>();
            playerName = photonView.Owner.NickName;
            nameTxt.text = playerName;

            if (photonView.IsMine)
            {
                gameManager.AddPlayerTeamList(this);
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("WaterBlock") && !isPrision)
            {
                GameManager.Sound.SFXPlay("BombLocked", GameManager.Sound.bombLocked);
                isPrision = true;
                animator.SetBool("BeforeDie", true);
                prevMoveSpeed = moveSpeed;
                moveSpeed = 0.5f;
                hitBox.gameObject.SetActive(true);
            }
        }

        public void OnBeforeDieAnimationFinish()
        {
            GameManager.Sound.SFXPlay("BombDead", GameManager.Sound.bombDead);
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

            // 사망 후 게임 승리조건 판단
            if (PhotonNetwork.IsMasterClient)
            {
                gameManager.RemovePlayerTeamList(this);
                gameManager.CheckGameState();
            }
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
}
