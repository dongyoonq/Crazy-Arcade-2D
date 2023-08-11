using KDY;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class InGameItem : Item
{
    public abstract void ApplyStatus(InGamePlayer player);
    public abstract void RemoveStatus(InGamePlayer player);

    private void OnEnable()
    {
        StartCoroutine(collisionActive(0.4f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            InGamePlayer player = collision.gameObject.GetComponent<InGamePlayer>();
            ApplyStatus(player);
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("WaterBlock"))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator collisionActive(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<Collider2D>().enabled = true;
    }
}