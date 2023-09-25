using Photon.Pun;
using UnityEngine;

public class Tile : MonoBehaviourPun
{
    public bool isInstallBombTile = false;

    public void InstallBomb(bool tf)
    {
        photonView.RPC("InstallBombRPC", RpcTarget.AllBuffered, tf);
    }

    [PunRPC]
    private void InstallBombRPC(bool tf)
    {
        isInstallBombTile = tf;
    }
}