﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOUNCivilian : Noun
{
    private ImpressionModifier m_weaponEquipped;
    public override void ReactToEvent(AIEvent newEvent, Observer observer)
    {
        base.ReactToEvent(newEvent, observer);
        Debug.Log("Reacting to event: " + newEvent.GetType().ToString());
        generalDangerDisguiseFactors(newEvent, observer);
    }
    protected void generalDangerDisguiseFactors(AIEvent newEvent, Observer observer)
    {
        System.Type t = newEvent.GetType();
        if (t == typeof(AIEVItemGet))
        {
            OnItemGet((AIEVItemGet)newEvent,observer);
        }
        else if (t == typeof(AIEVSeenByObserver))
        {
            OnSight((AIEVSeenByObserver)newEvent, observer);
        }
    }

    protected void OnSight(AIEVSeenByObserver ev, Observer obs)
    {
        Debug.Log("On Sight");
        GameObject observedObj = ev.ObservedObj;
        if (observedObj.GetComponent<InventoryContainer>() != null)
        {
            Debug.Log("Has an inventory");
        }
    }

    protected void OnItemGet(AIEVItemGet ev,Observer obs)
    {
        if (ev.Slot != null && ev.Slot.SlotType == InventorySlotType.EQUIPMENT &&
                LogicManager.IsA(ev.NewItem.gameObject, typeof(NOUNWeapon), obs))
        {
            ImpressionModifier m_weaponEquipped = new ImpressionModifier("sawHoldingWeapon", -0.2f, 20,5,3);
            obs.AddModifier(ev.ObservedObj.name, this, m_weaponEquipped);
        }
    }

    protected void OnAttackAction(AIEVObservedAction ev, Observer obs)
    {
        if (LogicManager.IsA(ev.ObservedAction, typeof(VBAttack), obs))
        {
            ImpressionModifier m_weaponEquipped = new ImpressionModifier("sawAttacking", -0.25f, 10,3,2);
            obs.AddModifier(ev.ObservedObj.name, this, m_weaponEquipped);
        } else if (LogicManager.IsA(ev.ObservedAction, typeof(VBGunshot), obs))
        {
            ImpressionModifier m_weaponEquipped = new ImpressionModifier("sawGunshot", -1f, 120);
            obs.AddModifier(ev.ObservedObj.name, this, m_weaponEquipped);
        }
    }
    protected void OnAttackPerson(AIEVHitConfirm ev, Observer obs)
    {
        if (LogicManager.IsA(ev.ObjectHit, typeof(VBAttack), obs))
        {
            float severity = Mathf.Max(-0.2f, ev.MyHitInfo.Damage * 0.05f);
            ImpressionModifier m_weaponEquipped = new ImpressionModifier("sawAttacking", severity, 10, 3, 2);
            obs.AddModifier(ev.ObservedObj.name, this, m_weaponEquipped);
        }
    }
}
