using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WUIAmmo : WUIBase
{
    [SerializeField]
    private Sprite EmptyIcon;
    [SerializeField]
    private Image m_primaryEquipmentIcon;
    [SerializeField]
    private Image m_secondaryEquipmentIcon;
    [SerializeField]
    private TextMeshProUGUI m_primaryCurrentClipText;
    [SerializeField]
    private TextMeshProUGUI m_primaryReserveAmmoText;
    [SerializeField]
    private TextMeshProUGUI m_secondaryCurrentClipText;
    [SerializeField]
    private TextMeshProUGUI m_secondaryReserveAmmoText;

    [SerializeField]
    private TextMeshProUGUI m_primaryName;
    [SerializeField]
    private TextMeshProUGUI m_secondaryName;

    private Item m_lastPrimary;
    private Item m_lastSecondary;
    private void Update()
    {
        if (Target == null)
            return;
        InventoryHolder ih = Target.GetComponent<InventoryHolder>();
        if (ih == null)
            return;
        UpdateAmmo(ih.GetItemInSlot("Primary"), m_lastPrimary, m_primaryEquipmentIcon, m_primaryName, m_primaryCurrentClipText, m_primaryReserveAmmoText);
        //UpdateAmmo(ih.GetItemInSlot("Secondary"), m_lastSecondary, m_secondaryEquipmentIcon, m_secondaryName, m_secondaryCurrentClipText, m_secondaryReserveAmmoText);
    }

    private void UpdateAmmo(Item i, Item lastItem, Image icon, TextMeshProUGUI name,  TextMeshProUGUI clip, TextMeshProUGUI reserve)
    {
        if (i == null)
        {
            icon.sprite = EmptyIcon;
            clip.text = "";
            reserve.text = "";
            name.text = "empty";
        }
        if (i != lastItem)
        {
            icon.sprite = i.InventoryIcon;
            lastItem = i;
            name.text = i.name;
        }
        EqpWeapon wep = i as EqpWeapon;
        if (wep == null)
        {
            clip.text = "";
            reserve.text = "";
        } else
        {
            clip.text = wep.CurrentAmmo + " / " + wep.ClipSize;
            reserve.text = Target.GetComponent<InventoryHolder>().GetItemCount(wep.AmmoType.GetComponent<Item>()).ToString();
        }
    }
}
