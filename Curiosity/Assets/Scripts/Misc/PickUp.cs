using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public InventoryObject pickUp;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with " + other.gameObject.name);

        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Player Call");
            InventoryManager.Instance.AddObjectToInventory(pickUp);
        }
    }
}
