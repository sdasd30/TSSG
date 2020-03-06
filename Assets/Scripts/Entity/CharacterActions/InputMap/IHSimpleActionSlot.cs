using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
public class IHSimpleActionSlot : InputHandler
{

    public ActionInfo OnLeftMouse;
    public string OnLeftMouseName;
    public ActionInfo OnRightMouse;
    public string OnRightMouseName;
    public ActionInfo OnInteractButton;
    public string OnInteractName;
    [System.Serializable] public class DictionaryOfInputKeyAndAction : SerializableDictionary<InputKey, ActionInfo> { }
    [System.Serializable] public class DictionaryOfInputKeyAndString : SerializableDictionary<InputKey, string> { }
    public DictionaryOfInputKeyAndAction ButtonActions = new DictionaryOfInputKeyAndAction();
    public DictionaryOfInputKeyAndString ButtonActionStrings = new DictionaryOfInputKeyAndString();

    CharacterBase m_charBase;
    private void Start()
    {
        m_charBase = GetComponent<CharacterBase>();
    }
    public override void HandleInput(InputPacket ip)
    {
        if (ip.leftMousePress)
        {
            m_charBase.TryAction(OnLeftMouse, null, InputKey.Fire);
            m_charBase.TryAction(OnLeftMouseName, null, InputKey.Fire);
        }
            
        if (ip.rightMousePress)
        {
            m_charBase.TryAction(OnRightMouse, null, InputKey.Fire2);
            m_charBase.TryAction(OnRightMouseName, null, InputKey.Fire);
        }
            
        if (ip.InputKeyPressed.ContainsKey(InputKey.Interact) && ip.InputKeyPressed[InputKey.Interact])
        {
            m_charBase.TryAction(OnInteractButton, null, InputKey.Interact);
            m_charBase.TryAction(OnInteractName, null, InputKey.Interact);
        }
            
        foreach (InputKey keyID in ButtonActions.Keys)
        {
            if (ip.InputKeyPressed.ContainsKey(keyID) && ip.InputKeyPressed[keyID])
            {
                m_charBase.TryAction(ButtonActions[keyID]);
            }
        }
        foreach (InputKey keyID in ButtonActionStrings.Keys)
        {
            if (ip.InputKeyPressed.ContainsKey(keyID) && ip.InputKeyPressed[keyID])
            {
                m_charBase.TryAction(ButtonActions[keyID]);
            }
        }
    }
}
