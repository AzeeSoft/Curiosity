using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public InventoryObject pickUp;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            InventoryManager.Instance.AddObjectToInventory(pickUp);
        }
    }
}
