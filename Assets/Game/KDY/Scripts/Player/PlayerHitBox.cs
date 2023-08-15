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
        // �÷��̾� ������ ���¿����� ó��
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && owner.isPrision)
        {
            // ���� ������ �ٸ� �÷��̾ ��ġ�� ó��
            InGamePlayer otherPlayer = collision.gameObject.GetComponent<InGamePlayer>();

            // ���� ���� �ٸ���
            if (otherPlayer.currTeam != owner.currTeam)
            {
                KillPlayer();
            }
            // ���� ������
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
