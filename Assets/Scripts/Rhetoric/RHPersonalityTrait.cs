using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHPersonalityTrait : MonoBehaviour
{
    public Sprite TraitIcon;
    public string GetHoverText()
    {
        return "";
    }

    public virtual void OnListenerStart(RHListener l, RHSpeaker speaker, RHConversation conversation)
    {

    }
    public virtual void OnSpeakerStart(RHSpeaker speaker, Dictionary<RHListener, float> listeners, RHConversation conversation)
    {

    }

    public virtual float OnStatementTaken(float baseValue,  RHStatement statement, RHListener l, RHSpeaker speaker, RHConversation conversation)
    {
        return baseValue;
    }
    public virtual float OnStatementUsed(float baseValue, RHStatement statement, RHListener l, RHSpeaker speaker, RHConversation conversation)
    {
        return baseValue;
    }
}
