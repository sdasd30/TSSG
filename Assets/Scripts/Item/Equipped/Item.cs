 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BasicPhysics))]
[RequireComponent(typeof(CharacterController))]
public class Item : Interactable
{
    public bool Equipabble;
    public int MaxStack = 1;
    public int CurrentStack = 1;

    public string displayname;
    public Sprite InventoryIcon;
    public delegate void LoadFunction(CharData d);
    public LoadFunction m_onSave;

    public CharData ItemProperties;

    [HideInInspector]
    public bool Rotated = false;
    [HideInInspector]
    public Vector2 baseSize = new Vector2(1, 1);
    [HideInInspector]
    public InventorySlotUI CurrentSlot;
    [HideInInspector]
    public string prefabName;

    protected InventoryContainer m_currentContainer;
    protected Vector2 m_slotPosition;


    // Start is called before the first frame update
    void Awake()
    {
        if (GetComponent<PersistentItem>() != null)
            ItemProperties = GetComponent<PersistentItem>().data;
        if (InventoryIcon == null && GetComponent<SpriteRenderer>() != null)
            InventoryIcon = GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void AddItemOptions(ItemMenu itemMenu) { }
    public virtual GameObject OnEnterInventory(InventoryContainer s, EquipmentSlot es) { return null; }

    public virtual void OnExitInventory(InventoryContainer s, EquipmentSlot es) { }

    public virtual bool CanEnterInventory(InventoryContainer i, InventorySlotUI s) {
        return (s.SlotType == InventorySlotType.NORMAL);
    }

    public virtual string UIString()
    {
        string s = "";
        if (CurrentStack > 1)
            s = CurrentStack + " / " + MaxStack;
        return s;
    }
    public void DestroyItem()
    {
        m_currentContainer.ClearItem(m_slotPosition);
    }
    public void SetSlotData(InventoryContainer container, Vector2 slotPos)
    {
        //Debug.Log("Setting slot to: " + container + " slot: " + slotPos);
        m_currentContainer = container;
        m_slotPosition = slotPos;
    }
    public Vector2 GetSlot()
    {
        return m_slotPosition;
    }
    public InventoryContainer GetContainer()
    {
        return m_currentContainer;
    }
    protected override void onTrigger(GameObject interactor) {
        if (interactor.GetComponent<InventoryHolder>())
        {
            
            bool added = interactor.GetComponent<InventoryHolder>().AddItemIfFree(this);
            
        }
    }

    public override bool ShouldDisplayPrompt()
    {
        return (m_currentContainer == null);
    }
    public void SaveItems()
    {
        if (ItemProperties == null)
        {
            gameObject.AddComponent<PersistentItem>();
            ItemProperties = GetComponent<PersistentItem>().data;
        }
        ItemProperties.PersistentInt["CurrentStack"] = CurrentStack;
        if (m_onSave != null)
            m_onSave(ItemProperties);
        else
            onItemSave(ItemProperties);
    }
    public void LoadItems()
    {
        if (ItemProperties.PersistentInt.ContainsKey("CurrentStack"))
            CurrentStack = ItemProperties.PersistentInt["CurrentStack"];
        onItemLoad(ItemProperties);
    }
    public virtual void onItemSave(CharData d) { }

    public virtual void onItemLoad(CharData d) { }

    public void SetItemActive(bool active, bool held = true)
    {
        //Debug.Log("Assigning current slot to: " + es.coordinate);
        GetComponent<BasicPhysics>().SetEnabled(active);
        GetComponent<CharacterController>().enabled = active;

        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().enabled = !held;
        if (!held)
            transform.SetParent(null);
        if (GetComponent<BasicPhysics>())
        {
            GetComponent<BasicPhysics>().GravityForce = (active) ? -1.0f : 0.0f;
            GetComponent<SpriteRenderer>().enabled = active;
        }
    }
}
