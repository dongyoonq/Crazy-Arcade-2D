using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Extension;

public class PlayerSpawn : MonoBehaviourPun
{
    [SerializeField] GameObject tileContents;

    public SpawnPoint[] spawnPoints;

    private void Start()
    {
        spawnPoints = tileContents.GetComponentsInChildren<SpawnPoint>();
    }
}
