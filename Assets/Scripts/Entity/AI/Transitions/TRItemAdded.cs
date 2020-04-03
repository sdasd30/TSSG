using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRItemAdded : Transition
{
    public Item PrefabOfItem;
    public override void OnItemGet(Item i)
    {
        base.OnItemGet(i);
        if (i.name == PrefabOfItem.name)
        {
            TargetTask.SetTargetObj(i.gameObject);
            TriggerTransition();
        }
            
    }
}
