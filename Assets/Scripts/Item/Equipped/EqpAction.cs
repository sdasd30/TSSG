using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqpWeapon : Equipment
{
    public ActionInfo PrimaryAction;
    public ActionInfo SecondaryAction;
    public ActionInfo ReloadAction;

    public GameObject AmmoType;
    public int ClipSize = 8;
    public int CurrentAmmo = 0;


    public override void OnPrimaryUse(InputPacket input,GameObject user) {
        if (PrimaryAction != null)
        {
            int i = ItemProperties.PersistentInt["CurrentAmmo"];
            if (PrimaryAction.GetComponent<ActionGunshot>() != null)
            {
                int ammoConsumed = PrimaryAction.GetComponent<ActionGunshot>().m_weaponStats.AmmoConsumedPerShot;
                if (ammoConsumed > CurrentAmmo)
                {
                    AttemptReload(input, user);
                } 
            }
            //OverrideCurrentEquipSprite(user);
            PrimaryAction.SourceEqp = this;
            user.GetComponent<CharacterBase>().TryAction(PrimaryAction, OnRegisterHit);
        }
    }
    public override void OnSecondaryUse(InputPacket input, GameObject user)
    {
        if (SecondaryAction != null)
        {
            //OverrideCurrentEquipSprite(user);
            SecondaryAction.SourceEqp = this;
            user.GetComponent<CharacterBase>().TryAction(SecondaryAction, OnRegisterHit);
        } 
        else if (ReloadAction != null)
        {
            AttemptReload(input,user);
        } else
        {
            OnPrimaryUse(input, user);
        }
            
    }

    private void AttemptReload(InputPacket input, GameObject user)
    {
        ReloadAction.SourceEqp = this;
        user.GetComponent<CharacterBase>().TryAction(ReloadAction, OnRegisterHit);
    }

    public override void onItemLoad(CharData d)
    {
        base.onItemLoad(d);
        if (d.PersistentInt.ContainsKey("CurrentAmmo"))
        {
            CurrentAmmo = d.PersistentInt["CurrentAmmo"];
        }
    }

    public override void onItemSave(CharData d)
    {
        base.onItemSave(d);
        d.PersistentInt["CurrentAmmo"] = CurrentAmmo;
    }

}
