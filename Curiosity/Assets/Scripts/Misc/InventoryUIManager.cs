using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public Transform itemsParent;

    public GameObject invPanel;
    public bool invActive = false;

    InventoryManager inventory;

    InventorySlot[] slots;

    private void Start()
    {
        inventory = InventoryManager.Instance;
        inventory.onInvChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        inventory.miscSpace = slots.Length;

        UpdateUI();

    }

    private void Update()
    {
        if(Input.GetButtonUp("Inventory"))
        {
            if (!invActive)
            {
                invActive = true;
            }
            else
            {
                invActive = false;
            }
        }

        if (invActive)
        {
            invPanel.SetActive(true);
        }
        else
        {
            invPanel.SetActive(false);
        }
    }

    void UpdateUI()
    {
        InventoryObject[] allCurrentMisc = InventoryManager.Instance.GetAllCurrentMisc();

        for (int i = 0; i < slots.Length; i++)
        {
            Debug.Log("Updating UI");
            slots[i].ClearSlot();
            if (i < inventory.misc.Count)
            {
                slots[i].AddItem(allCurrentMisc[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
