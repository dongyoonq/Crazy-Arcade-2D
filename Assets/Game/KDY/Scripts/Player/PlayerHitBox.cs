using KDY;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHitBox : MonoBehaviour
{
    public InGamePlayer owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 물감옥 상태에서의 처리
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && owner.isPrision)
        {
            // 감옥 상태중 다른 플레이어가 터치시 처리
            InGamePlayer otherPlayer = collision.gameObject.GetComponent<InGamePlayer>();

            // 서로 팀이 다르면
            if (otherPlayer.currTeam != owner.currTeam)
            {
                KillPlayer();
            }
            // 팀이 같으면
            else
            {
                RescuePlayer();
            }

            gameObject.SetActive(false);
        }
    }

    private void KillPlayer()
    {
        owner.animator.SetBool("BeforeDie", false);
        owner.animator.SetBool("Die", true);

        if (owner.photonView.IsMine)
            owner.GetComponent<PlayerInput>().enabled = false;

        owner.StartCoroutine(owner.DissapearRoutime());
    }

    private void RescuePlayer()
    {
        owner.animator.SetBool("BeforeDie", false);
        owner.animator.SetBool("Rescue", true);
        owner.moveSpeed = owner.prevMoveSpeed;
        owner.isPrision = false;
    }
}
