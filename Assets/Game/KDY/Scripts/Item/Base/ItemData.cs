using KDY;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = 1000)]
public class ItemData : ScriptableObject
{
    [SerializeField] public Item itemPrefab;
    [SerializeField] public int price;
    [SerializeField] public string description;
}