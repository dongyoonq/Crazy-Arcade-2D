using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : Tile
{
    [SerializeField] float dissapearTime;

    private void OnEnable()
    {
        Destroy(gameObject, dissapearTime);
    }
}
