using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRItemRemoved : Transition
{
    public Item PrefabOfItem;

    public override void OnItemLost(InventoryItemData i)
    {
        base.OnItemLost(i);
        if (i.itemName == PrefabOfItem.name)
            TriggerTransition();
    }
}
