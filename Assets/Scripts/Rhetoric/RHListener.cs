using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHListener : MonoBehaviour
{
    public float EmotionalIntensity;
    public delegate float ModifyValueFunction(float baseValue,  RHStatement statement, RHSpeaker speaker);
    public delegate float BaseValueFunction(float baseValue, RHConversation conversation, RHSpeaker speaker);
    private List<RHPersonalityTrait> m_traits = new List<RHPersonalityTrait>();
    public virtual List<RHPersonalityTrait> Traits { get { return m_traits; } }

    private Dictionary<RHPersonalityTrait, ModifyValueFunction> statementModifiers = new Dictionary<RHPersonalityTrait, ModifyValueFunction>();
    private Dictionary<RHPersonalityTrait, BaseValueFunction> baseValueModifiers = new Dictionary<RHPersonalityTrait, BaseValueFunction>();

    private void Start()
    {
        RHPersonalityTrait[] traits = GetComponentsInChildren<RHPersonalityTrait>();
        m_traits.AddRange(traits);
    }
    public void OnRhetoricStart(RHSpeaker speaker, RHConversation conversation)
    {
        foreach (RHPersonalityTrait t in m_traits)
        {
            t.OnListenerStart(this, speaker, conversation);
        }
    }
    public float ApplyStatementModifiers(float baseValue, RHSpeaker speaker, RHStatement statement, RHConversation conversation)
    {
        foreach (RHPersonalityTrait t in m_traits)
        {
            baseValue = t.OnStatementUsed(baseValue, statement, this, speaker, conversation);
        }
        return baseValue;
    }

    public float GetAuthority(RHSpeaker speaker)
    {
        return GetComponent<Observer>().GetImpressionModifiers(speaker.gameObject.name, new NOUNAuthority());
    }
    public float GetTrust(RHSpeaker speaker)
    {
        return GetComponent<Observer>().GetImpressionModifiers(speaker.gameObject.name, new NOUNTrustable());
    }
    public float GetFavor(RHSpeaker speaker)
    {
        return GetComponent<Observer>().GetImpressionModifiers(speaker.gameObject.name, new NOUNFavorable());
    }
    public virtual RHResponseString GetResponseString(RHListener listener, RHSpeaker speaker, float effectiveness)
    {
        return new RHResponseString("");
    }
}
