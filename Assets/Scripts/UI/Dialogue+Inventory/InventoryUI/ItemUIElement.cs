﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class ItemUIElement : MonoBehaviour, IDragHandler, IEndDragHandler, 
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Item ItemInfo;
    public bool CanMove = true;

    private Vector3 mouseoffset = new Vector3();
    private Image m_image;
    public TextMeshProUGUI uitext;

    //public InventorySlotUI CurrentSlot;
    private string m_extraString;
    private Vector3 m_returnPos;
    public void Start()
    {
        UpdateReturnPos(GetComponent<RectTransform>().localPosition);
        m_image = GetComponent<Image>();
        //ItemInfo = GetComponent<Item>();
        m_image.color = Color.gray;
        m_extraString = ItemInfo.UIString();
        uitext.text = m_extraString;
        uitext.raycastTarget = false;
    }
    
    public void UpdateReturnPos(Vector3 pos)
    {
        m_returnPos = pos;
    }
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //if (pointerEventData.button.ToString() == "Right")
        //{
        //    DialogueSelectionInitializer doi = new DialogueSelectionInitializer("Item");
        //    doi.AddDialogueOption("Drop Item", dropItem);
        //    doi.AddDialogueOption("Text2", "you selection something else");
        //    doi.AddDialogueOption("Last 3", "Final item selected");
        //    Debug.Log("open menu item: " + ItemInfo);
        //    TextboxManager.StartDialogueOptions(doi);
        //}
    }

    //private void dropItem(DialogueOption dop)
    //{
    //    //Debug.Log("Attempting drop to: " + ItemInfo.CurrentSlot.m_container.gameObject.transform.position );
    //    GameObject go = Instantiate((GameObject)Resources.Load(ItemInfo.ItemProperties.prefabPath), 
    //        ItemInfo.CurrentSlot.m_container.gameObject.transform.position + new Vector3(0f, 0.25f, 0f), Quaternion.identity);

    //    ItemInfo.CurrentSlot.m_container.ClearItem(ItemInfo.CurrentSlot.Coordinate);
    //    Debug.Log("Saving item: " + ItemInfo);
    //    ItemInfo.SaveItems();
    //    go.GetComponent<Item>().ItemProperties = ItemInfo.ItemProperties;
    //    go.GetComponent<PersistentItem>().recreated = true;

    //    go.GetComponent<BasicPhysics>().Floating = false;
    //    GameObject.Destroy(dop.MasterBox.gameObject);
    //    //Debug.Assert(false);
    //    GameObject.Destroy(gameObject);
        
    //}
    public void OnBeginDrag(PointerEventData eventData)
    {
        /*Debug.Log("On begin drag");
        Debug.Log("Button: " + eventData.button);
        mouseoffset = Input.mousePosition - transform.position;
        maxY = transform.GetComponent<RectTransform>().rect.height / 2f;
        m_image.color = Color.yellow;
        m_image.raycastTarget = false;
        InventoryUIManager.SetHeldItem(this);*/
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("button down: " + eventData.button);
        if (eventData.button.ToString() == "Right")
        {
            InventoryUIManager.CreateItemMenu(this);
        } else
        {
            Vector2 newPos = Input.mousePosition - mouseoffset;
            float newX = Mathf.Min(Mathf.Max(0f, newPos.x), Screen.width);
            float newY = Mathf.Min(Mathf.Max(0f, newPos.y), Screen.height);
            transform.position = new Vector3(newX, newY, 0f);
            m_image.raycastTarget = false;
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button.ToString() == "Left")
        {
            m_image.color = Color.gray;
            
            if (InventoryUIManager.GetCurrentCell() != null &&
                InventoryUIManager.AttemptMoveItem(this))
            {
                UpdateReturnPos(GetComponent<RectTransform>().localPosition);
            }
            else if (InventoryUIManager.GetHighlightedItem() != null)
            {
                InventoryUIManager.AttemptSwap(InventoryUIManager.GetHighlightedItem(), this);
            }
            else
            {
                ReturnPos();
            }
            InventoryUIManager.SetHeldItem(null);
            m_image.raycastTarget = true;
        }        
    }

    public void MatchItemProperties(Item i)
    {

    }

    public void ReturnPos()
    {
        transform.localPosition = m_returnPos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanMove)
        {
            InventoryUIManager.SetHighlightedItem(this);
            m_image.color = Color.yellow;
        }
        else
        {
            m_image.color = Color.red;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUIManager.ClearHighlightedItem(this);
        m_image.color = Color.gray;
    }
}
