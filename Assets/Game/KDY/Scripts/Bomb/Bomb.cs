using KDY;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KDY
{
    public class Bomb : MonoBehaviourPun
    {
        [SerializeField] float bombCoolTime;
        [SerializeField] int bombPower = 1;

        public InGamePlayer owner;

        private void OnEnable()
        {
            // GetOwnerItem();
            StartCoroutine(BombCoolTimer());
        }

        private void GetOwnerItem()
        {
            foreach (Item item in owner.itemLists)
            {
                // 물줄기 아이템 읽고 bombPower를 정함
            }
        }

        IEnumerator BombCoolTimer()
        {
            yield return new WaitForSeconds(bombCoolTime);
            Explosion(bombPower);
            Destroy(gameObject);
        }

        private void Explosion(int count)
        {
            if (count == 0)
            {
                CreateBombWaterCenter();
                return;
            }

            CreateBombWaterLeft(count);
            CreateBombWaterRight(count);
            CreateBombWaterUp(count); 
            CreateBombWaterDown(count);
            Explosion(--count);
        }

        private void CreateBombWaterCenter()
        {
            PhotonNetwork.Instantiate("Prefabs/bombwater_center", transform.position, Quaternion.identity);
        }

        private void CreateBombWaterLeft(int count)
        {
            if (count == 1)
                PhotonNetwork.Instantiate("Prefabs/bombwater_leftend", transform.position + -transform.right * bombPower, Quaternion.identity);
            else if (count > 1)
                PhotonNetwork.Instantiate("Prefabs/bombwater_left", transform.position + -transform.right * (bombPower - count + 1), Quaternion.identity);
        }

        private void CreateBombWaterRight(int count)
        {
            if (count == 1)
                PhotonNetwork.Instantiate("Prefabs/bombwater_rightend", transform.position + transform.right * bombPower, Quaternion.identity);
            else if (count > 1)
                PhotonNetwork.Instantiate("Prefabs/bombwater_right", transform.position + transform.right * (bombPower - count + 1), Quaternion.identity);
        }

        private void CreateBombWaterUp(int count)
        {
            if (count == 1)
                PhotonNetwork.Instantiate("Prefabs/bombwater_upend", transform.position + transform.up * bombPower, Quaternion.identity);
            else if (count > 1)
                PhotonNetwork.Instantiate("Prefabs/bombwater_up", transform.position + transform.up * (bombPower - count + 1), Quaternion.identity);
        }

        private void CreateBombWaterDown(int count)
        {
            if (count == 1)
                PhotonNetwork.Instantiate("Prefabs/bombwater_downend", transform.position + -transform.up * bombPower, Quaternion.identity);
            else if (count > 1)
                PhotonNetwork.Instantiate("Prefabs/bombwater_down", transform.position + -transform.up * (bombPower - count + 1), Quaternion.identity);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                GetComponent<Collider2D>().isTrigger = false;
            }
        }
    }

}