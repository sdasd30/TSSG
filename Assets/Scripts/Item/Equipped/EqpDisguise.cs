using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqpDisguise : PassiveEquipment
{
    public string DisguiseName;
    private Noun m_disguise;

    public override void OnEquip(InventoryContainer i, EquipmentSlot es) {
        m_disguise = LogicManager.GetNewNoun(DisguiseName);
        i.GetComponent<Observable>()?.AddImpression(m_disguise);
    }
    public override void OnDeequip(InventoryContainer i, EquipmentSlot es) {
        i.GetComponent<Observable>()?.RemoveImpression(m_disguise);
    }
}
