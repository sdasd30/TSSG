﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(InventoryContainer))]
public class IHEquipmentActionSlot : InputHandler
{
    public string LeftMouseItem = "primary";
    public string RightMouseItem = "secondary";
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
        if (m_container == null)
            return;
        if (ip.leftMousePress)
            m_container.EquipmentUseUpdatePlayer(LeftMouseItem, ip);
        if (ip.rightMousePress)
            m_container.EquipmentUseUpdatePlayer(RightMouseItem, ip);
        foreach (InputKey keyID in ButtonActions.Keys)
        {
            if (ip.InputKeyPressed.ContainsKey(keyID) && ip.InputKeyPressed[keyID])
            {
                m_container.EquipmentUseUpdatePlayer(ButtonActions[keyID], ip);
            }
        }
    }
}
