using Photon.Chat.Demo;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Block : MonoBehaviourPun, IPunObservable
{
    [SerializeField] List<string> dropTableResource;
    int[] percent = Enumerable.Range(1, 100).ToArray();
    bool isDropped = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WaterBlock") && !isDropped)
        {
            photonView.RPC("RequestCreateItem", RpcTarget.MasterClient, transform.GetChild(0).position + (transform.up * 0.3f), Quaternion.identity);
        }
    }

    [PunRPC]
    protected void RequestCreateItem(Vector3 position, Quaternion rotation)
    {
        isDropped = true;
        int dropPercent = (int)(percent.Length * 0.35f);
        int dropRandom = Random.Range(1, 101);
        int dropItemRandom = Random.Range(0, dropTableResource.Count);

        for (int i = 0; i < dropPercent; i++)
        {
            if (percent[i] == dropRandom)
            {
                PhotonNetwork.InstantiateRoomObject(dropTableResource[dropItemRandom], position, rotation);
                break;
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isDropped);
        }
        else
        {
            isDropped = (bool)stream.ReceiveNext();
        }
    }
}
