using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public delegate void OnInvChanged();
    public OnInvChanged onInvChangedCallback;

    public int miscSpace = 15;
    public int collectableSpace = 15;

    // public List<Dictionary<string, InventoryObject>> Inventory;
    public Dictionary<string, InventoryObject> collectables = new Dictionary<string, InventoryObject>();
    public Dictionary<string, InventoryObject> misc = new Dictionary<string, InventoryObject>();
    public Dictionary<string, InventoryObject> upgrades = new Dictionary<string, InventoryObject>();

    //Singelton region
    #region Instancing

    private static int m_referenceCount = 0;

    private static InventoryManager m_instance;

    public static InventoryManager Instance
    {
        get
        {
            return m_instance;
        }
    }
    void Awake()
    {
        m_referenceCount++;
        if (m_referenceCount > 1)
        {
            Debug.LogError("Two Inventory Managers!");
            DestroyImmediate(this.gameObject);
            return;
        }
        m_instance = this;

    }

    void OnDestroy()
    {
        m_referenceCount--;
        if (m_referenceCount == 0)
        {
            m_instance = null;
        }
    }

    #endregion

    private void Start()
    {
        //replace later with loading inventory from save data **ADD SAVING AND LOADING**
        //Inventory.Add(collectables);
        //Inventory.Add(upgrades);
        //Inventory.Add(misc);
    }

    public void AddObjectToInventory(InventoryObject _object)
    {
        Debug.Log("Adding " + _object.name + " to inventory");

        switch (_object.pickUpType)
        {
            case InventoryObject.Type.Collectable:
                if(collectables.Count > collectableSpace)
                {
                    Debug.LogWarning("Not enough room in collectables.");
                    return;
                }
                collectables.Add(_object.name, _object);
                break;
            case InventoryObject.Type.Misc:
                if (misc.Count > miscSpace)
                {
                    Debug.LogWarning("Not enough room in misc.");
                    return;
                }
                misc.Add(_object.name, _object);
                break;
            case InventoryObject.Type.Upgrade:
                upgrades.Add(_object.name, _object);
                break;
        }

        if (onInvChangedCallback != null)
        {
            onInvChangedCallback.Invoke();
        }
    }

    public void RemoveObjectFromInventory(InventoryObject _object)
    {
        switch (_object.pickUpType)
        {
            case InventoryObject.Type.Collectable:
                if(collectables.ContainsValue(_object))
                {
                    collectables.Remove(_object.name);
                }
                else
                {
                    Debug.LogError(_object.name + " NOT FOUND IN COLLECTABLES!!");
                }
                break;
            case InventoryObject.Type.Misc:
                if (misc.ContainsValue(_object))
                {
                    misc.Remove(_object.name);
                }
                else
                {
                    Debug.LogError(_object.name + " NOT FOUND IN MISC!!");
                }
                break;
            case InventoryObject.Type.Upgrade:
                if (upgrades.ContainsValue(_object))
                {
                    upgrades.Remove(_object.name);
                }
                else
                {
                    Debug.LogError(_object.name + " NOT FOUND IN UPGRADES!!");
                }
                break;
        }
        if (onInvChangedCallback != null)
        {
            onInvChangedCallback.Invoke();
        }
    }

    public InventoryObject[] GetAllCurrentCollectables()
    {
       InventoryObject[] currentCollectables = new List<InventoryObject>(collectables.Values).ToArray();
        return currentCollectables;
    }

    public InventoryObject[] GetAllCurrentUpgrades()
    {
        InventoryObject[] currentUpgrades = new List<InventoryObject>(upgrades.Values).ToArray();
        return currentUpgrades;
    }

    public InventoryObject[] GetAllCurrentMisc()
    {
        InventoryObject[] currentMisc = new List<InventoryObject>(misc.Values).ToArray();
        return currentMisc;
    }

    public InventoryObject FindObjectByName(InventoryObject.Type _type, string _name)
    {
        switch (_type)
        {
            case InventoryObject.Type.Collectable:
                if (collectables.ContainsKey(_name))
                {
                    return collectables[_name];
                }
                else
                {
                    Debug.LogWarning("Collectable Object" + _name + " not found!");
                    break;
                }
            case InventoryObject.Type.Misc:
                if (misc.ContainsKey(_name))
                {
                    return misc[_name];
                }
                else
                {
                    Debug.LogWarning("Misc Object" + _name + " not found!");
                    break;
                }
            case InventoryObject.Type.Upgrade:
                if (upgrades.ContainsKey(_name))
                {
                    return upgrades[_name];
                }
                else
                {
                    Debug.LogWarning("Upgrade Object" + _name + " not found!");
                    break;
                }
        }

        return null;
    }
    
}
