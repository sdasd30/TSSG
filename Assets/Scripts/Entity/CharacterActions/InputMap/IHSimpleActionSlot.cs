﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
public class IHSimpleActionSlot : InputHandler
{

    public ActionInfo OnLeftMouse;
    public ActionInfo OnRightMouse;
    public SerializableDictionary<InputKey, ActionInfo> ButtonActions = new SerializableDictionary<InputKey, ActionInfo>();

    CharacterBase m_charBase;
    private void Start()
    {
        m_charBase = GetComponent<CharacterBase>();
    }
    public override void HandleInput(InputPacket ip)
    {
        if (ip.leftMousePress && OnLeftMouse != null)
            m_charBase.TryAction(OnLeftMouse,null,InputKey.Fire);
        if (ip.leftMousePress && OnLeftMouse != null)
            m_charBase.TryAction(OnRightMouse,null,InputKey.Fire2);
        foreach (InputKey keyID in ButtonActions.Keys)
        {
            if (ip.InputKeyPressed.ContainsKey(keyID) && ip.InputKeyPressed[keyID])
            {
                m_charBase.TryAction(ButtonActions[keyID]);
            }
        }
    }
}