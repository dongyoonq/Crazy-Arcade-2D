using Photon.Chat.Demo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    [SerializeField] List<ItemData> dropTable;
    int[] percent = Enumerable.Range(1, 100).ToArray();
    bool isDropped = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WaterBlock") && !isDropped)
        {
            isDropped = true;

            int dropRandom = Random.Range(1, 101);
            int dropItemRandom = Random.Range(0, dropTable.Count);
            int dropPercent = (int)(percent.Length * 0.35f);

            Debug.Log(dropRandom);

            for (int i = 0; i < dropPercent; i++)
            {
                if (percent[i] == dropRandom)
                {
                    Instantiate(dropTable[dropItemRandom].itemPrefab, transform.GetChild(0).position, Quaternion.identity);
                    break;
                }
            }

            Destroy(gameObject);
        }
    }
}
