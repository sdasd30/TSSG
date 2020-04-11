using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIMenu : MonoBehaviour
{
    public InventoryContainer Container;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Container.Dirty)
        {
            UpdateInventory();
            Container.Dirty = false;
        }
    }

    public void CloseInventoryMenu()
    {
        Container.CloseContainer();
    }

    public void UpdateInventory()
    {
        CloseInventoryMenu();
        InventoryUIManager.CreateInventoryGUI(Container);
    }
    
}
