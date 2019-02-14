using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pick Up", menuName = "Inventory")]
public class InventoryObject : ScriptableObject
{
    public Sprite icon;

    public enum Type
    {
        Upgrade,
        Collectable,
        Misc
    }

    public Type pickUpType;

}
