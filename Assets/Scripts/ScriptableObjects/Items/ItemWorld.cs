using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ItemWorld", order = 1)]
public class ItemWorld : ScriptableObject
{
    public string name;
    public Rarity rarity;
    public Mesh mesh;
    public Sprite icon;

    public enum Rarity {
        Common, Uncommon, Rare, Epic, Legendary, Nastya
    }
}
