using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqpAction : Equipment
{
    public ActionInfo PrimaryAction;
    public ActionInfo SecondaryAction;

    public override void OnPrimaryUse(InputPacket input,GameObject user) {
        if (PrimaryAction != null)
        {
            //OverrideCurrentEquipSprite(user);
            user.GetComponent<CharacterBase>().TryAction(PrimaryAction, OnRegisterHit);
        }
    }
    public override void OnSecondaryUse(InputPacket input, GameObject user)
    {
        if (SecondaryAction != null)
        {
            //OverrideCurrentEquipSprite(user);
            user.GetComponent<CharacterBase>().TryAction(SecondaryAction, OnRegisterHit);
        } 
        else
        {
            OnPrimaryUse(input, user);
        }
            
    }

}
