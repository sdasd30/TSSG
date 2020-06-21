using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHListener : MonoBehaviour
{
    private float m_emotionalIntensity;
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
    public float GetEmotionalIntensity(bool applyPersonalityTraits = false)
    {
        float f = m_emotionalIntensity;
        foreach (RHPersonalityTrait t in m_traits)
        {
            f = t.ModifyEmotion(this, f);
        }
        return f;
    }
    public void SetEmotionalIntensity(float value)
    {
        m_emotionalIntensity = value;
    }
    public float GetAuthority(RHSpeaker speaker, bool applyPersonalityTraits = false)
    {
        NOUNAuthority n = new NOUNAuthority();
        return ApplyPersonalityTraits(speaker, n, applyPersonalityTraits);
    }
    public float GetTrust(RHSpeaker speaker, bool applyPersonalityTraits = false)
    {
        NOUNTrustable n = new NOUNTrustable();
        return ApplyPersonalityTraits(speaker, n, applyPersonalityTraits);
    }
    public float GetFavor(RHSpeaker speaker, bool applyPersonalityTraits = false)
    {
        NOUNFavorable n = new NOUNFavorable();
        
        return ApplyPersonalityTraits(speaker,n, applyPersonalityTraits);
    }
    private float ApplyPersonalityTraits(RHSpeaker speaker, Noun n,bool applyPersonalityTraits = false)
    {
        float f = GetComponent<Observer>().GetImpressionModifiers(speaker.gameObject.name, n);
        if (!applyPersonalityTraits)
            return f;
        foreach (RHPersonalityTrait t in m_traits)
        {
            f = t.ModifyNoun(this, f, speaker, n);
        }
        return f;
    }
    public virtual RHResponseString GetResponseString(RHListener listener, RHSpeaker speaker, float effectiveness)
    {
        return new RHResponseString("");
    }
}
