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
    public virtual void OnSpeakerStart(RHSpeaker speaker, List<RHListener> listeners, RHConversation conversation)
    {

    }

    public virtual float OnStatementTaken(float baseValue,  RHStatement statement, RHListener l, RHSpeaker speaker, RHConversation conversation)
    {
        return baseValue;
    }
    public virtual float OnStatementUsed(float baseValue, RHStatement statement, RHListener l, RHSpeaker speaker, RHConversation conversation, RHStat s = RHStat.CURRENT_PERSUASION_LEVEL)
    {
        return baseValue;
    }

    public virtual float OnGetAttribute(float baseValue, RHStatement statement, RHListener l, RHSpeaker speaker, RHConversation conversation)
    {
        return baseValue;
    }
    public virtual float ModifyEmotion(RHListener listener, float previousValue)
    {
        return previousValue;
    }
    public virtual float ModifyNoun(RHListener listener, float previousValue, RHSpeaker speaker, Noun n)
    {
        return previousValue;
    }
}
