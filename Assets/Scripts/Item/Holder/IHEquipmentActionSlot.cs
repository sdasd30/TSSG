using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(InventoryContainer))]
public class IHEquipmentActionSlot : InputHandler
{
    public string LeftMouseItem = "Primary";
    public string RightMouseItem = "Secondary";
    [System.Serializable] public class DictionaryOfInputKeyAndString : SerializableDictionary<InputKey, string> { }
    public DictionaryOfInputKeyAndString ButtonActions;
    private InventoryContainer m_container;
    // Start is called before the first frame update
    void Start()
    {
        m_container = GetComponent<InventoryContainer>();
    }

    public override void HandleInput(InputPacket ip)
    {
        if (m_container == null || InventoryUIManager.IsMenuOpen()) 
            return;
        if (ip.leftMousePress)
            m_container.EquipmentSlotUseUpdatePlayer(LeftMouseItem, ip);
        if (ip.rightMousePress)
            m_container.EquipmentSlotUseUpdatePlayer(RightMouseItem, ip);
        if (ip.InputKeyPressed.ContainsKey(InputKey.Reload) && ip.InputKeyPressed[InputKey.Reload])
        {
            bool reloaded = m_container.EquipmentReload(LeftMouseItem, ip);
            if (!reloaded)
                m_container.EquipmentReload(RightMouseItem, ip);
        }
        foreach (InputKey keyID in ButtonActions.Keys)
        {
            if (ip.InputKeyPressed.ContainsKey(keyID) && ip.InputKeyPressed[keyID])
            {
                m_container.EquipmentSlotUseUpdatePlayer(ButtonActions[keyID], ip);
            }
        }
        foreach (string s in ip.itemSlotUse)
        {
            if (!m_container.EquipmentSlotUseUpdatePlayer(s, ip))
            {
                m_container.ItemNameUseUpdatePlayer(s, ip);
            }
        }
    }
}
