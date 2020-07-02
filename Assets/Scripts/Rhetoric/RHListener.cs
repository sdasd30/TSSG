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
    private Dictionary<RHStat, List<ImpressionModifier>> m_temporaryModifiers = new Dictionary<RHStat, List<ImpressionModifier>>();
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
    public void OnReceiveStatement(RHRegisteredStatement rs)
    {
        RHStatement statement = rs.statement;
        RHSpeaker speaker = rs.speaker;
        RHConversation c = rs.conversation;

        GetComponent<AITaskManager>()?.triggerEvent(new AIEVStatementReceived(c, statement, speaker));
        Dictionary<RHStat, float> results = new Dictionary<RHStat, float>();
        for (int i = 0; i < 5; i++)
        {
            RHStat s = (RHStat)i;
            float power = statement.GetPower(speaker, this, c, s);
            float diff = applyStatementModifiers(power, rs, s);
            if (diff == 0)
                continue;

            results[s] = diff;
            ModifyStat(speaker, c, s, diff);
        }
        statement.OnStatementReceived(speaker, this,c,  results);
        RHManager.AddHistoryText(GetResponseString(statement, speaker, results));
    }
    private float applyStatementModifiers(float baseValue, RHRegisteredStatement rs, RHStat s)
    {
        foreach (RHPersonalityTrait t in m_traits)
        {
            baseValue = t.OnStatementUsed(baseValue, rs.statement, this, rs.speaker, rs.conversation,s);
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
    public virtual RHResponseString GetResponseString(RHStatement statement, RHSpeaker speaker, Dictionary<RHStat,float> results)
    {
        return null;
    }
    public void ModifyStat(RHSpeaker s, RHConversation c, RHStat stat, float value, bool permanent = false)
    {
        if (stat == RHStat.CURRENT_PERSUASION_LEVEL)
        {
            c.ModifyListenerPersuasion(this, value);
            return;
        }
            
        if (stat == RHStat.EMOTIONS)
        {
            m_emotionalIntensity += value;
            return;
        }
            
        string label = getModifierString(s,c,permanent);
        ImpressionModifier i = new ImpressionModifier(label, value);
        if (!permanent)
        {
            if (!m_temporaryModifiers.ContainsKey(stat))
                m_temporaryModifiers[stat] = new List<ImpressionModifier>();
            m_temporaryModifiers[stat].Add(i);
        }
        
        GetComponent<Observer>().AddModifier(s.gameObject.name, RHStatToNoun(stat), i);
    }
    public Dictionary<RHStat, float> GetDifferenceStats(RHSpeaker speaker)
    {
        Dictionary<RHStat, float> diffs = new Dictionary<RHStat, float>();
        foreach (RHStat s in m_temporaryModifiers.Keys)
        {
            diffs[s] = 0f;
            foreach (ImpressionModifier im in m_temporaryModifiers[s])
                diffs[s] += im.getModValue();
        }
        return diffs;
    }

    public void ClearTempStats(RHSpeaker speaker)
    {
        foreach (RHStat s in m_temporaryModifiers.Keys)
        {
            foreach (ImpressionModifier im in m_temporaryModifiers[s])
                GetComponent<Observer>().ClearModifier(speaker.gameObject.name, RHStatToNoun(s), im);
        }
    }
    public string getModifierString(RHSpeaker s, RHConversation c, bool permanent = false)
    {
        return "RHConv" + s.gameObject.name + "::conv::" + c.gameObject.name + "Time:" + ScaledTime.UITimeElapsed.ToString() + "::perm::" + permanent.ToString();
    }
    private Noun RHStatToNoun(RHStat s)
    {
        switch (s)
        {
            case RHStat.AUTHORITY:
                return new NOUNAuthority();
            case RHStat.FAVOR:
                return new NOUNFavorable();
            case RHStat.TRUST:
                return new NOUNTrustable();
            default:
                return new NOUNAuthority();
        }
    }
    private float ApplyPersonalityTraits(RHSpeaker speaker, Noun n, bool applyPersonalityTraits = false)
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
}
