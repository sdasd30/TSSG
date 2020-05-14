using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEquipment : Item
{
    public override bool CanEnterInventory(InventoryContainer i, InventorySlotUI s)
    {
        return (s.SlotType != InventorySlotType.EQUIPMENT);
    }

    public override GameObject OnEnterInventory(InventoryContainer s, EquipmentSlot es)
    {
        base.OnEnterInventory(s, es);
        
        if (es != null && es.SlotType == InventorySlotType.PASSIVE)
        {
            SetItemActive(false);
            name = es.SlotName;
            OnEquip(s, es);
        }
        return null;
    }

    public override void OnExitInventory(InventoryContainer s, EquipmentSlot es)
    {
        base.OnExitInventory(s, es);
        if (m_currentContainer.GetComponent<CharacterBase>() != null)
            m_currentContainer.GetComponent<CharacterBase>().SkipActionToEnd();

        if (es != null && es.SlotType == InventorySlotType.PASSIVE)
        {
            OnDeequip(s, es);
        }
    }

    public virtual void OnEquip(InventoryContainer i, EquipmentSlot es) {}
    public virtual void OnDeequip(InventoryContainer i, EquipmentSlot es) {}
}
