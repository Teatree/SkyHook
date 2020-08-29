using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ItemWorld", order = 1)]
public class ItemWorld : ScriptableObject
{
    public string name;
    public Rarity rarity;
    public Mesh mesh;

    public enum Rarity {
        Common, Uncommon, Rare, Epic, Legendary, Nastya
    }
}
