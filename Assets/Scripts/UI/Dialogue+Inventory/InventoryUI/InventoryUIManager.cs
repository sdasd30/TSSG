﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    private static InventoryUIManager m_instance;

    public GameObject InvPrefab;
    public GameObject SlotPrefab;
    public GameObject ItemIconPrefab;
    public GameObject ItemMenuPrefab;
    public Canvas MainInventoryCanvas;

    private InventorySlotUI m_highlightedSlot;
    private ItemUIElement m_highlightedItem;
    private ItemUIElement m_currentItem;
    private Dictionary<InventoryContainer,GameObject> m_containerPrefabs;
    private Dictionary<InventoryHolder,int> m_openHolder;
    private InventoryContainer m_Container;
    

    public static InventoryUIManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }
    public void Start()
    {
        m_containerPrefabs = new Dictionary<InventoryContainer, GameObject>();
        m_openHolder = new Dictionary<InventoryHolder, int>();
        m_Container = GetComponent<InventoryContainer>();
    }
    public static bool IsMenuOpen()
    {
        return (m_instance.m_containerPrefabs.Count != 0);
    }
    public static void CreateItemMenu(ItemUIElement iue)
    {
        if (iue.transform.Find("ItemMenu(Clone)") != null)
            return;
        GameObject newMenu = Instantiate(m_instance.ItemMenuPrefab, iue.transform);
        newMenu.GetComponent<ItemMenu>().HighlightedItem = iue.ItemInfo;
    }
    public static GameObject CreateInventoryGUI(InventoryContainer ic)
    {
        GameObject go = Instantiate(m_instance.InvPrefab, m_instance.MainInventoryCanvas.transform);
        go.GetComponent<InventoryUIMenu>().Container = ic;
        m_instance.m_containerPrefabs.Add(ic, go);
        if (m_instance.m_openHolder.ContainsKey(ic.Holder))
            m_instance.m_openHolder[ic.Holder]++;
        else
            m_instance.m_openHolder[ic.Holder] = 1;
        int invX = 1;
        foreach (InventoryHolder h in m_instance.m_openHolder.Keys)
        {
            if (h == ic.Holder)
                break;
            invX++;
        }
        int invY = m_instance.m_openHolder[ic.Holder];
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300f + (300f * (invX - 1)),  - (200f * (invY - 1)));
        go.GetComponent<RectTransform>().sizeDelta = new Vector2(150f + ((ic.size.x - 1 )* 50f), 150f + ((ic.size.y - 1) * 50f));

        go.transform.Find("DragBar").Find("OwnerName").GetComponent<TextMeshProUGUI>().text = ic.Holder.name;
        go.transform.Find("DragBar").Find("OwnerName").GetComponent<TextMeshProUGUI>().text = ic.name;
        Transform slotParent = go.transform.Find("Inv").Find("Slots");
        Dictionary<Vector2, InventorySlotUI> slots = new Dictionary<Vector2, InventorySlotUI>();
        for (int i = 0; i < ic.size.x * ic.size.y; i++)
        {
            GameObject newSlot = Instantiate(m_instance.SlotPrefab, slotParent);
            newSlot.GetComponent<InventorySlotUI>().m_container = ic;
           
            newSlot.GetComponent<InventorySlotUI>().m_container = ic;
            Vector2 coordinates = new Vector2(i % ic.size.x,Mathf.FloorToInt(i / ic.size.x));
            if (ic.eqpSlotInfo.ContainsKey(coordinates))
            {
                newSlot.GetComponent<InventorySlotUI>().SlotType = ic.eqpSlotInfo[coordinates].SlotType;
                newSlot.GetComponent<InventorySlotUI>().SetSlotName(ic.eqpSlotInfo[coordinates].SlotName);
            } else
            {
                newSlot.GetComponent<InventorySlotUI>().SlotType = InventorySlotType.NORMAL;
            }
            newSlot.GetComponent<InventorySlotUI>().ItemOffsetPos = new Vector2((coordinates.x) * 50f, (-coordinates.y) * 50f);
            newSlot.GetComponent<InventorySlotUI>().Coordinate = coordinates;
            slots.Add(coordinates, newSlot.GetComponent<InventorySlotUI>());
        }
        Transform itemParent = go.transform.Find("Inv").Find("Items");
        foreach (Vector2 v in ic.items.Keys)
        {
            m_instance.addItemIcon(ic.items[v],v,slots,itemParent,ic);
        }
        return go;
    }
    public static void CloseGUIPanel(InventoryContainer ic)
    {
        if (m_instance.m_containerPrefabs.ContainsKey(ic))
        {
            Destroy(m_instance.m_containerPrefabs[ic]);
            m_instance.m_containerPrefabs.Remove(ic);

            m_instance.m_openHolder[ic.Holder]--;
            if (m_instance.m_openHolder[ic.Holder] == 0)
                m_instance.m_openHolder.Remove(ic.Holder);
        }
    }

    public static bool AttemptMoveItem(ItemUIElement iue)
    {
        if (m_instance.m_highlightedSlot.CanFitItem(iue.ItemInfo))
        {
            if (!iue.ItemInfo.CanEnterInventory(iue.ItemInfo.CurrentSlot.m_container, m_instance.m_highlightedSlot))
                return false;
            iue.UpdateReturnPos(m_instance.m_highlightedSlot.ItemOffsetPos);
            iue.transform.SetParent( m_instance.m_highlightedSlot.transform.parent.parent.Find("Items") );
            iue.ReturnPos();
            iue.ItemInfo.CurrentSlot.m_container.ClearItem(iue.ItemInfo.CurrentSlot.Coordinate);
            m_instance.m_highlightedSlot.AddItem(iue.ItemInfo);
            iue.ItemInfo.CurrentSlot = m_instance.m_highlightedSlot;
            iue.ItemInfo.CurrentSlot = m_instance.m_highlightedSlot;
            return true;
        }
        iue.ReturnPos();
        return false;
    }
    public static void DropItem(ItemUIElement item)
    {
        item.ItemInfo.CurrentSlot.m_container.DropItem(item.ItemInfo.CurrentSlot.Coordinate);
    }
    public static bool AttemptSwap(ItemUIElement item1, ItemUIElement item2)
    {
        InventorySlotUI slot1 = item1.ItemInfo.CurrentSlot;
        InventorySlotUI slot2 = item2.ItemInfo.CurrentSlot;
        if (slot1.CanFitItem(item2.ItemInfo) && slot2.CanFitItem(item1.ItemInfo) &&
            item1.ItemInfo.CanEnterInventory(slot2.m_container,slot2) &&
            item2.ItemInfo.CanEnterInventory(slot1.m_container,slot1))
        {
            Debug.Log("Moving item 1");
            GameObject newSlot = Instantiate(m_instance.SlotPrefab, m_instance.transform);
            InventorySlotUI tempSlot = newSlot.GetComponent<InventorySlotUI>();
            tempSlot.m_container = m_instance.m_Container;
            m_instance.MoveItemTo(item1, tempSlot);
            
            Debug.Log("Moving item 2");
            m_instance.MoveItemTo(item2, slot1);
            m_instance.MoveItemTo(item1, slot2);
            Destroy(newSlot);
            return true;
        }
        item1.ReturnPos();
        item2.ReturnPos();
        return false;
    }
    public static void AttemptSwap(Item item1, Item item2)
    {
        InventoryContainer container1 = item1.GetContainer();
        InventoryContainer container2 = item2.GetContainer();

        GameObject newSlot = Instantiate(m_instance.SlotPrefab, m_instance.transform);
        InventorySlotUI tempSlot = newSlot.GetComponent<InventorySlotUI>();
        tempSlot.m_container = m_instance.m_Container;

        container1.ClearItem(item1.GetSlot());
        tempSlot.AddItem(item1);
        item1.CurrentSlot = tempSlot;

        InventoryUIManager.MoveItemTo(item2, container1,item1.GetSlot());
        InventoryUIManager.MoveItemTo(item1, container2, item2.GetSlot());
        Destroy(newSlot);
    }
    public static void MoveItemTo(Item i, InventoryContainer c,Vector2 slot, bool swapAllowed = true)
    {
        if (c.GetItem(slot) != null && swapAllowed)
            AttemptSwap(i, c.GetItem(slot).EquipmentInstance);
        c.ClearItem(slot);
        c.AddItem(i, slot);
    }
    private void MoveItemTo(ItemUIElement iue, InventorySlotUI slot)
    {
        iue.UpdateReturnPos(slot.ItemOffsetPos);
        if (slot.transform.parent.parent != null)
        {
            iue.transform.SetParent(slot.transform.parent.parent.Find("Items"));
            iue.ReturnPos();
        }
        
        iue.ItemInfo.CurrentSlot.m_container.ClearItem(iue.ItemInfo.CurrentSlot.Coordinate);
        slot.AddItem(iue.ItemInfo);
        iue.ItemInfo.CurrentSlot = slot;
    }
    private void addItemIcon(InventoryItemData i, Vector2 loc, Dictionary<Vector2,InventorySlotUI> slots, Transform parent,InventoryContainer c)
    {
        //if ((GameObject)Resources.Load(i.EquipmentInstance.ItemProperties.prefabPath) == null)
        //    return;
        //GameObject tempItem = Instantiate((GameObject)Resources.Load(i.EquipmentInstance.ItemProperties.prefabPath)); 
        GameObject go = Instantiate(ItemIconPrefab, parent);
        
        go.transform.localPosition = new Vector3((loc.x) * 50f, -50 - (loc.y - 1) * 50f
                , 3f);

        go.GetComponent<ItemUIElement>().ItemInfo = i.EquipmentInstance;
        
        go.GetComponent<ItemUIElement>().ItemInfo.CurrentSlot = slots[loc];
        if (i.InvIcon != null)
        {
            go.GetComponent<Image>().sprite = i.InvIcon;
        }
            
        //Destroy(tempItem);
    }

    public static List<Vector2> GetOccupiedSlots(Item i)
    {
        return new List<Vector2>();
    }
    private Vector2 GetDrawLocation()
    {
        return new Vector2();
    }
    public static void SetHighlightedCell(InventorySlotUI slot)
    {
        m_instance.m_highlightedSlot = slot;
    }
    public static void SetHighlightedItem(ItemUIElement item)
    {
        m_instance.m_highlightedItem = item;
    }
    public static void ClearHighlightedItem(ItemUIElement item)
    {
        if (m_instance.m_highlightedItem == item)
            m_instance.m_highlightedItem = null;
    }
    public static void SetHeldItem(ItemUIElement item)
    {
        m_instance.m_currentItem = item;
    }
    public static void ClearHighlightedCell(InventorySlotUI slot)
    {
        if (m_instance.m_highlightedSlot == slot)
            m_instance.m_highlightedSlot = null;
    }
    public static InventorySlotUI GetCurrentCell()
    {
        return m_instance.m_highlightedSlot;
    }

    public static ItemUIElement GetCurrentItem()
    {
        return m_instance.m_currentItem;
    }
    public static ItemUIElement GetHighlightedItem()
    {
        return m_instance.m_highlightedItem;
    }
}
