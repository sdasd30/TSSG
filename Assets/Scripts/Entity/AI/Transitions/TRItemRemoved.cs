using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRItemRemoved : Transition
{
    public Item PrefabOfItem;

    public override void OnItemLost(Item i)
    {
        base.OnItemLost(i);
        if (i.displayname == PrefabOfItem.name)
            TriggerTransition();
    }
}
