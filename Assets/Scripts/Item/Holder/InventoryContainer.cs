using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InitialItemData
{
    public Vector2 inventoryLocation;
    public GameObject Item;
    public int CurrentStack = 1;
    public CharData ItemProperties;
}
[System.Serializable]
public class EquipmentSlot
{
    public string SlotName;
    public InventorySlotType SlotType;
    public Vector2 coordinate;
    public bool CanFitItem(Item i) {
        if (SlotType == InventorySlotType.EQUIPMENT)
        {
            Equipment e = i as Equipment;
            return (e != null);
        }
        if (SlotType == InventorySlotType.PASSIVE)
        {
            PassiveEquipment e = i as PassiveEquipment;
            return (e != null);
        }
        return true;
    }
}
public enum InventorySlotType
{
    NORMAL, EQUIPMENT, PASSIVE
}

public class InventoryItemData
{
    [HideInInspector]
    public string itemName;
    [HideInInspector]
    public string displayName;
    [HideInInspector]
    public Vector2 size;
    [HideInInspector]
    public delegate void OnExitReturnFunc(InventoryContainer i, EquipmentSlot es);
    [HideInInspector]
    public OnExitReturnFunc exitFunc;
    public Item EquipmentInstance;
    public Sprite InvIcon;

    public InventoryItemData(Item i, CharData itemData = null) {
        itemName = i.name;
        displayName = (i.displayname.Length > 0) ? i.displayname : i.name;
        size = i.baseSize;
        exitFunc = i.OnExitInventory;
        EquipmentInstance = i;
        InvIcon = i.InventoryIcon;
    }
}

[RequireComponent(typeof(InventoryHolder))]
public class InventoryContainer : MonoBehaviour
{
    public Vector2 size;
    public List<InitialItemData> initItemData;
    public Dictionary<Vector2, InventoryItemData> items;
    public Dictionary<Vector2, EquipmentSlot> eqpSlotInfo;
    public List<EquipmentSlot> slotData;
    public string InventoryName;
    public InventoryHolder Holder;

    [HideInInspector]
    public bool Dirty = false;

    private bool m_displaying;
    private List<Vector2> m_freeSlots;
    private bool m_inventoryInitialized = false;
    public bool DisplayOnStart = false;
    internal void Awake()
    {
        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }

    void Start()
    {
        eqpSlotInfo = new Dictionary<Vector2, EquipmentSlot>();
        foreach (EquipmentSlot es in slotData)
        {
            eqpSlotInfo[es.coordinate] = es;
        }

        items = new Dictionary<Vector2, InventoryItemData>();

        m_freeSlots = new List<Vector2>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                m_freeSlots.Add(new Vector2(x, y));
            }
        }
        m_freeSlots.Sort((a, b) => (a.x + a.y * 10).CompareTo(b.x + b.y * 10));
        if (!m_inventoryInitialized)
            InitInventory();
        if (DisplayOnStart)
            ToggleDisplay();
    }
    
    public bool CanFit(Vector2 itemPos, Item i)
    {
        if (!hasSpace(itemPos, i.baseSize))
            return false;
        if (eqpSlotInfo.ContainsKey(itemPos) && !eqpSlotInfo[itemPos].CanFitItem(i))
            return false;
        foreach (Vector2 coord in items.Keys)
        {
            if (isOverlap(coord,items[coord].size,
                itemPos, i.baseSize))
                return false;
        }
        return true;
    }
    private bool hasSpace(Vector2 p, Vector2 s)
    {
        return (p.x + s.x <= size.x && p.y + s.y <= size.y);
    }
    private bool isOverlap(Vector2 l1, Vector2 size1,
                          Vector2 l2, Vector2 size2)
    {
        Vector2 r1 = new Vector2(l1.x + size1.x, l1.y + size1.y);
        Vector2 r2 = new Vector2(l2.x + size2.x, l2.y + size2.y);
        if (l1.x > r2.x || l2.x > r1.x)
        {
            return false;
        }
        if (l1.y < r2.y || l2.y < r1.y)
        {
            return false;
        }
        return true;
    }
    public void ToggleDisplay()
    {
        if (m_displaying)
            CloseContainer();
        else
            DisplayContainer();
    }
    public void DisplayContainer()
    {
        m_displaying = true;
        InventoryUIManager.CreateInventoryGUI(this);
    }

    public void CloseContainer()
    {
        m_displaying = false;
        InventoryUIManager.CloseGUIPanel(this);
    }

    private void InitializeSlots()
    {

    }
    public InventoryItemData GetItem(Vector2 pos)
    {
        if (items.ContainsKey(pos))
            return items[pos];
        return null;
    }
    public InventoryItemData GetItem(string slotName)
    {
        foreach(EquipmentSlot es in slotData)
        {
            if (es.SlotName == slotName)
                return GetItem(es.coordinate);
        }
        return null;
    }
    
    public Vector2 GetItemLocation(Item i)
    {
        foreach (Vector2 v in items.Keys)
        {
            InventoryItemData myI = items[v];
            if (myI.displayName == i.displayname)
            {
                return v;
            }
        }
        return new Vector2(-1, -1);
    }

    public void AddItem(Item i , Vector2 pos, bool autoStack = true)
    {
        GameObject instance = null;
        if (autoStack )
        {
            int stackLeft = i.CurrentStack;
            foreach (Vector2 v in items.Keys)
            {
                InventoryItemData myI = items[v];
                if (myI.displayName == i.displayname)
                {
                    Item itm = myI.EquipmentInstance;
                    int oldN = itm.CurrentStack;
                    itm.CurrentStack = Mathf.Min(itm.MaxStack, itm.CurrentStack + stackLeft);
                    stackLeft -= (itm.CurrentStack - oldN);
                    if (stackLeft <= 0)
                    {
                        Destroy(i.gameObject);
                        return;
                    }
                }
            }
            i.CurrentStack = stackLeft;
        }

        if (eqpSlotInfo.ContainsKey(pos))
        {
            instance = i.OnEnterInventory(this, eqpSlotInfo[pos]);
            Holder.GetComponent<AITaskManager>()?.triggerEvent(new AIEVItemGet(i,eqpSlotInfo[pos]));
        } else
        {
            instance = i.OnEnterInventory(this, null);
            Holder.GetComponent<AITaskManager>()?.triggerEvent(new AIEVItemGet(i));
        }
        if (items.ContainsKey(pos))
        {
            if (items[pos].displayName == i.displayname)
            {
                items[pos].EquipmentInstance.CurrentStack += i.CurrentStack;
                return;
            }
            items.Remove(pos);
        }
        i.transform.SetParent(transform);
        i.transform.localPosition = Vector3.zero;
        InventoryItemData iid = new InventoryItemData(i);
        if (instance != null)
            iid.EquipmentInstance = instance.GetComponent<Item>();
        items.Add(pos, iid);
        Dirty = true;

        m_freeSlots.Remove(pos);

        
    }
    public void DropItem(Vector2 v)
    {
        Debug.Log("Attempting to drop item at position: " + v);
        if (!items.ContainsKey(v))
            return;
        if (eqpSlotInfo.ContainsKey(v))
        {
            items[v].exitFunc(this, eqpSlotInfo[v]);
            Holder.GetComponent<AITaskManager>()?.OnItemLost(items[v].EquipmentInstance);
        }
        else
        {
            items[v].exitFunc(this, null);
            Holder.GetComponent<AITaskManager>()?.OnItemLost(items[v].EquipmentInstance);
        }
        items[v].EquipmentInstance.SetItemActive(true, false);
        items.Remove(v);
        m_freeSlots.Add(v);
        m_freeSlots.Sort((a, b) => (a.x + a.y * 10).CompareTo(b.x + b.y * 10));
        Dirty = true;

    }
    public int RemoveItem(Item i, int amount = 1)
    {
        int remainingToRemove = amount;
        int numRemoved = 0;
        List<Vector2> toRemove = new List<Vector2>();
        foreach (Vector2 v in items.Keys)
        {
            InventoryItemData myI = items[v];
            if (myI.displayName == i.displayname)
            {
                if (myI.EquipmentInstance.CurrentStack <= remainingToRemove)
                {
                    numRemoved += myI.EquipmentInstance.CurrentStack;
                    remainingToRemove -= myI.EquipmentInstance.CurrentStack;
                    toRemove.Add(v);
                    if (remainingToRemove <= 0)
                        return numRemoved;
                } else
                {
                    numRemoved += remainingToRemove;
                    myI.EquipmentInstance.CurrentStack -= remainingToRemove;  
                    return numRemoved;
                }
            }
        }
        foreach (Vector2 v in toRemove)
        {
            ClearItem(v);
            Dirty = true;
        }
            
        return numRemoved;
    }
    public int GetItemCount(Noun itemType, InventorySlotType slotType, Observer perspective)
    {
        int stack = 0;
        if (slotType == InventorySlotType.NORMAL)
        {
            foreach (InventoryItemData iid in items.Values)
                if (itemType.IsA(iid.EquipmentInstance.gameObject, perspective) > 0f)
                    stack += iid.EquipmentInstance.CurrentStack;
            return stack;
        }
        foreach (EquipmentSlot es in slotData)
        {
            if (es.SlotType == slotType)
            {
                InventoryItemData iid = GetItem(es.coordinate);
                if (itemType.IsA(iid.EquipmentInstance.gameObject, perspective) > 0f)
                    stack += iid.EquipmentInstance.CurrentStack;
            }
        }
        return stack;
    }

    public int GetItemCount( Item i )
    {
        int stack = 0;
        foreach (Vector2 v in items.Keys)
        {
            InventoryItemData myI = items[v];
            if (myI.displayName == i.displayname)
            {
                stack += myI.EquipmentInstance.CurrentStack;
            }
        }
        return stack;
    }
    public void ClearItem(Vector2 v)
    {
        Debug.Log("Attempting to clear item at position: " + v);
        if (!items.ContainsKey(v))
            return;
        if (eqpSlotInfo.ContainsKey(v))
        {
            items[v].exitFunc(this, eqpSlotInfo[v]);
            Holder.GetComponent<AITaskManager>()?.OnItemLost(items[v].EquipmentInstance);
        } else {
            items[v].exitFunc(this, null);
            Holder.GetComponent<AITaskManager>()?.OnItemLost(items[v].EquipmentInstance);
        }
        
        items.Remove(v);
        m_freeSlots.Add(v);
        m_freeSlots.Sort((a, b) => (a.x + a.y*10).CompareTo(b.x + b.y*10));
        Dirty = true;
    }
    
    public Vector2 findFreeSlot(Item i)
    {
        if (!canAcceptItem(i))
            return new Vector2(-1, -1);
        foreach (Vector2 v in items.Keys)
        {
            InventoryItemData myI = items[v];
            if (myI.displayName == i.displayname &&
                myI.EquipmentInstance.CurrentStack + i.CurrentStack <= myI.EquipmentInstance.MaxStack)
                return v;
        }
        foreach (Vector2 v in m_freeSlots)
        {
            if (CanFit(v, i))
                return v;
        }
        return new Vector2(-1,-1);
    }
    public Vector2 getSlot(string slotName)
    {
        foreach(Vector2 slot in eqpSlotInfo.Keys)
        {
            if (eqpSlotInfo[slot].SlotName == slotName)
                return slot;
        }
        return new Vector2(-1, 1);
    }
    private void InitInventory()
    {
        List<Vector2> vList = new List<Vector2>();
        foreach (Vector2 v in items.Keys){ vList.Add(v); }
        foreach (Vector2 v in vList) { ClearItem(v); }
        items.Clear();
        foreach (InitialItemData iid in initItemData)
        {
            GameObject prefab;
            if (iid.Item != null)
            {
                prefab = iid.Item;
                iid.ItemProperties.prefabPath = prefab.GetComponent<PersistentItem>().data.prefabPath;
            } else {
                //Debug.Log("Attempting to load: " + iid);
                if ((GameObject)Resources.Load(iid.ItemProperties.prefabPath) == null)
                    continue;
                prefab = Resources.Load(iid.ItemProperties.prefabPath) as GameObject;
            }
            GameObject go = Instantiate(prefab);
            go.GetComponent<Item>().ItemProperties = iid.ItemProperties;
            go.GetComponent<Item>().CurrentStack = Mathf.Max(1,Mathf.Min(go.GetComponent<Item>().MaxStack, iid.CurrentStack));
            go.GetComponent<Item>().LoadItems();
            AddItem(go.GetComponent<Item>(), iid.inventoryLocation);
        }
        //Debug.Log("InventoryInitialized");
        m_inventoryInitialized = true;
        Dirty = true;
    }

    public virtual bool canAcceptItem(Item i)
    {
        return true;
    }
    
    public virtual bool ItemNameUseUpdatePlayer(string itemName, InputPacket input)
    {
        int c = transform.childCount;
        for (int i = 0; i < c; i++)
        {
            Transform t = transform.GetChild(c);
            if (t.gameObject.GetComponent<Equipment>() && t.gameObject.GetComponent<Equipment>().displayname == itemName)
            {
                Equipment e = t.gameObject.GetComponent<Equipment>();
                if (input.movementInput.magnitude > 0.1f)
                {
                    e.OnSecondaryUse(input, gameObject);
                }
                else
                {
                    e.OnPrimaryUse(input, gameObject);
                }

                return true;
            }
        }
        return false;
    }
    public virtual bool EquipmentSlotUseUpdatePlayer(string slotOrItemName, InputPacket input)
    {
        Transform t = transform.Find(slotOrItemName);
        if (t == null)
        {
            return false;
        }
        Equipment e = t.gameObject.GetComponent<Equipment>();
        if (input.movementInput.magnitude > 0.1f)
        {
            e.OnSecondaryUse(input,gameObject);
        }
        else
        {
            e.OnPrimaryUse(input, gameObject);
        }
        return true;
    }
    public virtual bool EquipmentReload(string slotOrItemName, InputPacket input)
    {
        Transform t = transform.Find(slotOrItemName);
        if (t == null)
        {
            return false;
        }
        Equipment e = t.gameObject.GetComponent<Equipment>();
        var weapon = e as EqpWeapon;
        if (e == null)
            return false;
        if (!weapon.CanReload(gameObject))
            return false;
        weapon.AttemptReload(input, gameObject);
        return true;
    }
    private string convertToSaveList(Dictionary<Vector2, InventoryItemData> saveItems)
    {
        string saveList = "";
        foreach (Vector2 v in items.Keys)
        {
            InitialItemData newItem = new InitialItemData();
            newItem.inventoryLocation = v;
            newItem.CurrentStack = items[v].EquipmentInstance.CurrentStack;
            newItem.ItemProperties = new CharData();
            items[v].EquipmentInstance.SaveItems();
            newItem.ItemProperties = items[v].EquipmentInstance.ItemProperties;

            saveList += JsonUtility.ToJson(newItem) + "\n";
        }
        return saveList;
    }

    private void storeData(CharData d)
    {
        string s = convertToSaveList(items);
        d.SetString("initItemData", s);
    }

    private void loadData(CharData d)
    {
        string savedItems = d.GetString("initItemData");
        var arr = savedItems.Split('\n');
        initItemData.Clear();
        foreach (string s in arr)
        {
            if (s.Length > 0) {
                InitialItemData newItem = JsonUtility.FromJson<InitialItemData>(s);
                initItemData.Add(newItem);
            }
        }
        InitInventory();
    }
}
