using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryHolder))]
public class IHOpenInventory : InputHandler
{
    public bool RequirePlayerControllable = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void HandleInput(InputPacket ip)
    {
        if (ip.InputKeyPressed.ContainsKey(InputKey.Inventory) && 
            (!RequirePlayerControllable || GetComponent<MovementBase>().IsPlayerControl))
        {
            GetComponent<InventoryHolder>().ToggleAllInventory();
        }
    }
}
