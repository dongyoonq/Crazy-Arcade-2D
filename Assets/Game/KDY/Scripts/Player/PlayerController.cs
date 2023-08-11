using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KDY
{
    public class PlayerController : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] float attackCoolTime;

        private InGamePlayer player;
        private PlayerInput playerInput;
        private Rigidbody2D rb;
        private Animator animator;
        private Vector2 inputDir;
        private float lastBombTime = float.MinValue;

        private void Awake()
        {
            player = GetComponent<InGamePlayer>();
            playerInput = GetComponent<PlayerInput>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            if (!photonView.IsMine)
                Destroy(playerInput);
        }

        private void Update()
        {
            Move();
        }

        private void OnMove(InputValue value)
        {
            inputDir = value.Get<Vector2>();
        }

        private void OnAttack(InputValue value)
        {
            CreateBomb();
        }

        private void Move()
        {
            if (inputDir.magnitude <= 0.1f)
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("moveSpeed", rb.velocity.magnitude);
            }
            else
            {
                // 동시 입력 방지를 위한 처리
                if (inputDir.x > 0.1 || inputDir.x < -0.1)
                {
                    inputDir.y = 0;
                }
                else if (inputDir.y > 0.1 || inputDir.y < -0.1)
                {
                    inputDir.x = 0;
                }

                rb.velocity = inputDir * player.moveSpeed;
                animator.SetFloat("moveX", rb.velocity.x);
                animator.SetFloat("moveY", rb.velocity.y);
                animator.SetFloat("moveSpeed", rb.velocity.magnitude);
            }
        }

        private void CreateBomb()
        {
            photonView.RPC("RequestCreateBomb", RpcTarget.MasterClient, transform.position + transform.up * -0.25f, transform.rotation);
        }

        [PunRPC]
        private void RequestCreateBomb(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
        {
            // 마스터 클라이언트 입장(서버)에서 판정을 진행
            if (Time.time < lastBombTime + attackCoolTime)
                return;

            lastBombTime = Time.time;

            float sentTime = (float)info.SentServerTime;
            photonView.RPC("ResultCreateBomb", RpcTarget.AllViaServer, position, rotation, sentTime, info.Sender);
        }

        [PunRPC]
        private void ResultCreateBomb(Vector3 position, Quaternion rotation, float sentTime, Player player)
        {
            float lag = (float)(PhotonNetwork.Time - sentTime);

            Bomb bomb = GameManager.Resource.Instantiate<Bomb>("Prefabs/Bomb", position, rotation);
            bomb.owner = GetComponent<InGamePlayer>();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // 외부 사람들의 속도, 방향 동기화
            if (stream.IsWriting)
            {
                stream.SendNext(rb.velocity);
                stream.SendNext(inputDir);
            }
            else
            {
                rb.velocity = (Vector2)stream.ReceiveNext();
                inputDir = (Vector2)stream.ReceiveNext();
            }
        }
    }

}