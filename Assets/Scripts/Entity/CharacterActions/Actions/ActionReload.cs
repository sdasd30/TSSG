using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionReload : ActionInfo
{
    // Start is called before the first frame update
    protected override void OnAttack()
    {
        base.OnAttack();
        if (SourceEqp != null)
        {
            Reload((EqpWeapon)SourceEqp, m_charBase.GetComponent<InventoryHolder>());
        }
    }

    protected void Reload(EqpWeapon wep, InventoryHolder ih)
    {
        int bulletsToReload = wep.ClipSize;
        int rem = ih.RemoveItem(wep.AmmoType.GetComponent<Item>(), bulletsToReload);
        wep.CurrentAmmo += wep.ClipSize;
    }
}

 