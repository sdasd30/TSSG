using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler
{
    private Image m_slotBackground;
    private bool Occupied = false;

    public InventoryContainer m_container;
    public Item OccupyingItem;
    public Vector2 Coordinate;
    public Vector3 ItemOffsetPos;
    public InventorySlotType SlotType = InventorySlotType.NORMAL;
    public string SlotName = "noname";


    // Start is called before the first frame update
    void Start()
    {
        m_slotBackground = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryUIManager.GetCurrentItem() == null)
        {
            m_slotBackground.color = new Color(0.5f, 0.5f, 0.5f);
        } else
        {
            ItemUIElement iui = InventoryUIManager.GetCurrentItem();
            if (CanFitItem(iui.ItemInfo))
            {
                m_slotBackground.color = new Color(0.5f, 1.0f, 0.5f);
            } else
            {
                m_slotBackground.color = new Color(1.0f, 0.5f, 0.5f);
            }
        }
        InventoryUIManager.SetHighlightedCell(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Occupied)
        {
            m_slotBackground.color = new Color(0.7f, 0.7f, 0.7f);
        } else
        {
            m_slotBackground.color = new Color(1.0f, 1.0f, 1.0f);
        }
        InventoryUIManager.ClearHighlightedCell(this);
    }

    public bool CanFitItem (Item i)
    {
        bool canFit = m_container.CanFit(Coordinate, i);
        return canFit;
    }
    public bool CanSwapItem (Item i)
    {
        Debug.Log("Testing swap item. Current item: " + OccupyingItem);
        if (OccupyingItem != null)
        {
            Debug.Log("mSize: " + OccupyingItem.baseSize + " other item: " + i.baseSize);
            return (OccupyingItem.baseSize == i.baseSize);
        }
        return false;
    }
    public void AddItem(Item i)
    {
        m_container.AddItem(i, Coordinate);
    }

    public void SetSlotName(string s)
    {
        Transform t = transform.Find("SlotName");
        SlotName = s;
        if (t != null)
        {
            t.gameObject.GetComponent<TextMeshProUGUI>().text = s;
        }
    }
}
