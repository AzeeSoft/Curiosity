using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Dictionary<string, Dictionary<string, InventoryObject>> Inventory;
    public Dictionary<string, InventoryObject> collectables, upgrades, misc;

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
        Inventory.Add("collectables", collectables);
        Inventory.Add("upgrades", upgrades);
        Inventory.Add("misc", misc);
    }

    public void AddObjectToInventory(InventoryObject _object)
    {
        switch (_object.pickUpType)
        {
            case InventoryObject.Type.Collectable:
                collectables.Add(_object.name, _object);
                break;
            case InventoryObject.Type.Misc:
                misc.Add(_object.name, _object);
                break;
            case InventoryObject.Type.Upgrade:
                upgrades.Add(_object.name, _object);
                break;
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
