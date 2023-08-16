using KDY;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KDY
{
    public class Bomb : MonoBehaviourPun
    {
        [SerializeField] float tileYInterval;
        [SerializeField] float tileXInterval;
        [SerializeField] float bombCoolTime;

        [SerializeField] float castingXRange;
        [SerializeField] float castingYRange;

        public InGamePlayer owner;
        private Vector2 prevLeftWaterPos;
        private Vector2 prevRightWaterPos;
        private Vector2 prevUpWaterPos;
        private Vector2 prevDownWaterPos;
        private Tile currTile;

        private void OnEnable()
        {
            prevLeftWaterPos = transform.position;
            prevRightWaterPos = transform.position;
            prevUpWaterPos = transform.position;
            prevDownWaterPos = transform.position;

            SetBombTilePosition();
            StartCoroutine(BombCoolTimer());
        }

        private void OnDisable()
        {
            owner.currbombCount--;
            currTile.InstallBomb(false);
        }

        IEnumerator BombCoolTimer()
        {
            yield return new WaitForSeconds(bombCoolTime);
            GameManager.Sound.SFXPlay("ExplosionBomb", GameManager.Sound.bombExplosion);
            Explosion(owner.bombPower);
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
            PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_center", transform.position, Quaternion.identity);
        }

        private void CreateBombWaterLeft(int count)
        {
            if (CheckUI(prevLeftWaterPos, Vector2.left))
                return;

            if (CheckStaticBlockObject(prevLeftWaterPos, Vector2.left))
                return;

            if (CheckBlockObject(prevLeftWaterPos, Vector2.left))
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_leftend", (Vector3)prevLeftWaterPos + -transform.right * 1f * tileXInterval, Quaternion.identity);
                return;
            }

            if (count == 1)
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_leftend", transform.position + -transform.right * owner.bombPower * tileXInterval, Quaternion.identity);
            }
            else if (count > 1)
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_left", transform.position + -transform.right * ((owner.bombPower - count + 1) * tileXInterval), Quaternion.identity);
                prevLeftWaterPos = transform.position + -transform.right * ((owner.bombPower - count + 1) * tileXInterval);
            }
        }

        private void CreateBombWaterRight(int count)
        {
            if (CheckUI(prevRightWaterPos, Vector2.right))
                return;

            if (CheckStaticBlockObject(prevRightWaterPos, Vector2.right))
                return;

            if (CheckBlockObject(prevRightWaterPos, Vector2.right))
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_rightend", (Vector3)prevRightWaterPos + transform.right * 1f * tileXInterval, Quaternion.identity);
                return;
            }

            if (count == 1)
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_rightend", transform.position + transform.right * owner.bombPower * tileXInterval, Quaternion.identity);
            }
            else if (count > 1)
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_right", transform.position + transform.right * ((owner.bombPower - count + 1) * tileXInterval), Quaternion.identity);
                prevRightWaterPos = transform.position + transform.right * ((owner.bombPower - count + 1) * tileXInterval);
            }
        }

        private void CreateBombWaterUp(int count)
        {
            if (CheckUI(prevUpWaterPos, Vector2.up, 90f))
                return;

            if (CheckStaticBlockObject(prevUpWaterPos, Vector2.up, 90f))
                return;

            if (CheckBlockObject(prevUpWaterPos, Vector2.up, 90f))
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_upend", (Vector3)prevUpWaterPos + transform.up * 1f * tileYInterval, Quaternion.identity);
                return;
            }

            if (count == 1)
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_upend", transform.position + transform.up * owner.bombPower * tileYInterval, Quaternion.identity);
            }

            else if (count > 1)
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_up", transform.position + transform.up * ((owner.bombPower - count + 1) * tileYInterval), Quaternion.identity);
                prevUpWaterPos = transform.position + transform.up * ((owner.bombPower - count + 1) * tileYInterval);
            }
        }

        private void CreateBombWaterDown(int count)
        {
            if (CheckUI(prevDownWaterPos, Vector2.down, 90f))
                return;

            if (CheckStaticBlockObject(prevDownWaterPos, Vector2.down, 90f))
                return;

            if (CheckBlockObject(prevDownWaterPos, Vector2.down, 90f))
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_downend", (Vector3)prevDownWaterPos + -transform.up * 1f * tileYInterval, Quaternion.identity);
                return;
            }

            if (count == 1)
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_downend", transform.position + -transform.up * owner.bombPower * tileYInterval, Quaternion.identity);
            }

            else if (count > 1)
            {
                PhotonNetwork.InstantiateRoomObject("Prefabs/bombwater_down", transform.position + -transform.up * ((owner.bombPower - count + 1) * tileYInterval), Quaternion.identity);
                prevDownWaterPos = transform.position + -transform.up * ((owner.bombPower - count + 1) * tileYInterval);
            }
        }

        private bool CheckStaticBlockObject(Vector2 position, Vector2 direction, float angle = 0f)
        {
            RaycastHit2D hit = Physics2D.BoxCast(position, new Vector2(castingXRange, castingYRange), angle, direction, 0.5f, LayerMask.GetMask("StaticBlock"));

            if (hit)
                return true;
            else
                return false;
        }

        private bool CheckBlockObject(Vector2 position, Vector2 direction, float angle = 0f)
        {
            RaycastHit2D hit = Physics2D.BoxCast(position, new Vector2(castingXRange, castingYRange), angle, direction, 0.5f, LayerMask.GetMask("Block"));

            if (hit)
                return true;
            else
                return false;
        }

        private bool CheckUI(Vector2 position, Vector2 direction, float angle = 0f)
        {
            RaycastHit2D hit = Physics2D.BoxCast(position, new Vector2(castingXRange, castingYRange), angle, direction, 0.5f, LayerMask.GetMask("UI"));

            if (hit)
                return true;
            else
                return false;
        }

        private void SetBombTilePosition()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.01f, LayerMask.GetMask("Tile"));

            currTile = hit.collider.GetComponent<Tile>();

            currTile.InstallBomb(true);

            if (hit)
            {
                transform.position = hit.transform.position;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube((Vector2)transform.position + Vector2.left * 0.5f, new Vector2(castingXRange, castingYRange));
            Gizmos.DrawWireCube((Vector2)transform.position + Vector2.right * 0.5f, new Vector2(castingXRange, castingYRange));
            Gizmos.DrawWireCube((Vector2)transform.position + Vector2.up * 0.5f, new Vector2(castingXRange, castingYRange));
            Gizmos.DrawWireCube((Vector2)transform.position + Vector2.down * 0.5f, new Vector2(castingXRange, castingYRange));
        }
    }

}