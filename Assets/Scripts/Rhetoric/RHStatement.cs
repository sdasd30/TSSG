using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RHType { LOGOS, PATHOS,ETHOS, NONE}

public enum RHResourceType { POSITIVE, NEGATIVE, QUESTION, IDEA, OPINION   }

public class RHStatement : MonoBehaviour
{
    [SerializeField]
    private string m_statementName = "Generic Statement";

    [SerializeField]
    private RHType m_RhetoricType = RHType.LOGOS;

    [SerializeField]
    private float m_Time = 1.0f;

    [SerializeField]
    private float m_basePower = 1.0f;
    [SerializeField]
    private List<RHModifier> m_modPower = new List<RHModifier>();
    [SerializeField]
    private float m_baseFavor = 1.0f;
    [SerializeField]
    private List<RHModifier> m_modFavor = new List<RHModifier>();
    [SerializeField]
    private float m_baseTrust = 1.0f;
    [SerializeField]
    private List<RHModifier> m_modTrust = new List<RHModifier>();
    [SerializeField]
    private float m_baseAuthority = 1.0f;
    [SerializeField]
    private List<RHModifier> m_modAuthority = new List<RHModifier>();
    [SerializeField]
    private float m_baseEmotions = 1.0f;
    [SerializeField]
    private List<RHModifier> m_modEmotions = new List<RHModifier>();
    [SerializeField]
    private SerializableDictionary<RHResourceType, int> m_requirements = new SerializableDictionary<RHResourceType, int>();
    [SerializeField]
    private string m_hoverText = "Generic Description";

    [SerializeField]
    private RHResponseString m_responseString;

    [SerializeField]
    private bool apply_decay = true;
    

    [SerializeField]
    private bool displayModifierOnHover = true;

    public virtual string StatementName { get { return m_statementName; } }
    public virtual RHType RhetoricType { get  { return m_RhetoricType; }}
    public virtual float Time { get { return m_Time; } }

    private List<RHEvent> m_eventList = new List<RHEvent>();
    public List<RHEvent> RHEvents { get { return m_eventList; } }
    private void Start()
    {
        init();
    }

    protected void init()
    {
        m_eventList = new List<RHEvent>(FindObjectsOfType<RHEvent>());

        if (m_modPower == null)
            m_modPower = new List<RHModifier>();

        if (StatementName == null)
            m_statementName = name;

        if (!apply_decay)
            return;

        RHModifier m = new RHModifier("Decay", 0f, 0.85f, RHScaleModifier.INSTANCES_OF_SELF, 0f, RHScaleType.MULTIPLICATION);
        m.statement = this;
        m_modPower.Add(m);
    }

    public virtual void OnStatementQueued(RHSpeaker speaker)
    {
        speaker.ModifyResources(m_requirements, true);
    }
    public virtual void OnStatementCancelled(RHSpeaker speaker)
    {
        speaker.ModifyResources(m_requirements);
    }
    public virtual void OnStatementExecuted(RHSpeaker speaker)
    {

    }
    public virtual float GetPower(RHSpeaker speaker, RHListener listener, RHConversation c)
    {
        float value = m_basePower;
        foreach (RHModifier m in m_modPower)
        {
            value = m.ApplyModifier(value, c, listener);
        }
        return value;
    }

    public virtual float GetPower(RHSpeaker speaker, RHListener listener, RHConversation c, RHStat stateType = RHStat.CURRENT_PERSUASION_LEVEL)
    {
        float value = 0;
        List<RHModifier> modList = new List<RHModifier>();
        switch (stateType)
        {
            case RHStat.AUTHORITY:
                modList = m_modAuthority;
                value = m_baseAuthority;
                break;
            case RHStat.EMOTIONS:
                modList = m_modEmotions;
                value = m_baseEmotions;
                break;
            case RHStat.FAVOR:
                modList = m_modFavor;
                value = m_baseFavor;
                break;
            case RHStat.TRUST:
                modList = m_modTrust;
                value = m_baseTrust;
                break;
            case RHStat.CURRENT_PERSUASION_LEVEL:
                modList = m_modPower;
                value = m_basePower;
                break;
        }
        foreach (RHModifier m in modList)
        {
            value = m.ApplyModifier(value, c, listener);
        }
        return value;
    }
    public virtual bool IsEnabled(RHSpeaker speaker, RHListener listener, RHConversation c)
    {
        return speaker.meetsRequirements(m_requirements);
    }
    public virtual string GetHoverText(RHConversation conv)
    {
        string t = m_hoverText + "\n";
        float value = m_basePower;
        if (m_requirements.Count > 0)
        {
            t += "COST: ";
            foreach (RHResourceType r in m_requirements.Keys)
            {
                int c = m_requirements[r];
                t += c.ToString() + " " + r.ToString() + " ";
            }
        }
        if (displayModifierOnHover)
        {
            foreach (RHListener l in conv.Listeners.Keys)
            {
                t += l.gameObject.name;
                t += " Base Power: " + GetPower(conv.Speakers[0],l,conv);
                foreach (RHModifier m in m_modPower)
                {
                    t += " " + m.getHoverString(value, conv,l);
                    value = m.ApplyModifier(value, conv, l);
                }
                t += "\n";
            }
        }
        return m_hoverText;
    }

    public virtual RHStatement GenerateResponse( RHListener l)
    {
        RHStatement st = GetEmptyGenerateResponseStatement();
        return st;
    }
    public RHStatement GetEmptyGenerateResponseStatement()
    {
        GameObject go = Instantiate(RHManager.GenerateResourcesPrefab);
        RHStatement st = go.GetComponent<RHStatement>();
        Destroy(go);
        return st;
    }
    public virtual RHResponseString GetResponseString(RHListener listener, RHSpeaker speaker, float effectiveness)
    {
        return m_responseString;
    }

    public virtual RHStatement GenerateStandardResponse(RHSpeaker speaker, RHListener listener)
    {
        if (RandomChanceRange(1, listener.GetAuthority(speaker), true))
            return null;
        RHStatement response = GetEmptyGenerateResponseStatement();
        float severityEmotionScaling;
        float authorityResponseScaling;
        float favorPositiveScaling;
        float favorNegativeScaling;
        return response;
    }
    private bool RandomChanceRange(float max, float value,bool invert)
    {
        return (invert)?Random.Range(0, max) >= value : Random.Range(0,max) < value;
    }
    private float StandardDamageModifiers(float baseValue, RHListener l, RHType rhtype)
    {
        if (rhtype == RHType.LOGOS)
        {
            return baseValue;
        } else if (rhtype == RHType.ETHOS)
        {
            return baseValue;
        }
        else if (rhtype == RHType.PATHOS)
        {
            return baseValue;
        }
        return baseValue;
    }
    private string standardDamageBreakdown()
    {
        return "";
    }
}