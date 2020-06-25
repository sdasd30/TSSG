﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RHType { LOGOS, PATHOS,ETHOS, NONE}

public enum RHResourceType { NONE, POSITIVE, NEGATIVE, QUESTION, IDEA, OPINION  }

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
    private float m_listRankingPriority = 1.0f;

    [SerializeField]
    private RHResponseString m_responseString;

    [SerializeField]
    private bool apply_decay = true;
    

    [SerializeField]
    private bool displayModifierOnHover = true;

    public virtual string StatementName { get { return m_statementName; } }
    public virtual RHType RhetoricType { get  { return m_RhetoricType; }}
    public virtual float Time { get { return m_Time; } }

    protected List<RHEvent> m_eventList = new List<RHEvent>();
    public List<RHEvent> RHEvents { get { return m_eventList; } }
    private void Start()
    {
        init();
    }

    protected void init()
    {
        RHEvent[] attachedEvents = FindObjectsOfType<RHEvent>();
        foreach(RHEvent ev in attachedEvents)
        {
            if (!m_eventList.Contains(ev))
                m_eventList.Add(ev);
        }

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
    public virtual bool IsEnabled(RHSpeaker speaker, RHConversation c)
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
        RHStatement st = GetEmptyGenerateResourcesStatement();
        return st;
    }
    
    public virtual float GetListRankingPriority(RHSpeaker speaker, RHConversation c)
    {
        if (IsEnabled(speaker, c))
            return m_listRankingPriority + 100f;
        return m_listRankingPriority;
    }
    public virtual RHResponseString GetResponseString(RHListener listener, RHSpeaker speaker, float effectiveness)
    {
        return m_responseString;
    }

    public virtual RHStatement GenerateStandardResponse(RHSpeaker originalSpeaker, RHListener originalListener)
    {
        if (RandomChanceRange(200, 100 + originalListener.GetAuthority(originalSpeaker, true), true))
            return null;
        RHResource r;

        RHSGenerateResources response = GetEmptyGenerateResourcesStatement();
        Dictionary<float, RHResource> roller = new Dictionary<float, RHResource>();

        int intensity = Mathf.RoundToInt(Mathf.Max(1, originalListener.GetEmotionalIntensity() / 30f));
        float favorNonNegative = originalListener.GetFavor(originalSpeaker) + 100;
        float positiveScore = Mathf.RoundToInt(Mathf.Max(0, favorNonNegative));
        if (favorNonNegative > 120)
            positiveScore += (favorNonNegative - 100) * 5f;
        float negativeScore = Mathf.RoundToInt(Mathf.Max(0, 200 - favorNonNegative));
        if (favorNonNegative < 80)
            negativeScore -= (favorNonNegative - 100) * 5f;
        float neutralScore = 100f - Mathf.Abs(favorNonNegative - 100f);
        if (Mathf.Abs(favorNonNegative - 100f) < 20)
            neutralScore *= 2f;

        float trustNonNegative = originalListener.GetTrust(originalSpeaker);
        float ideaScore = (Mathf.Max(0f, trustNonNegative) - (originalListener.GetAuthority(originalSpeaker) * 0.5f)) * 0.5f;
        float opinionScore = (Mathf.Max(0f, trustNonNegative) - (originalListener.GetAuthority(originalSpeaker) * 0.5f)) * 0.4f;
        float questionScore = (Mathf.Max(0f, trustNonNegative) - (originalListener.GetAuthority(originalSpeaker) * 0.5f)) * 0.25f;

        roller = RegisterChance(roller, new RHResource(RHResourceType.POSITIVE, intensity), positiveScore);
        roller = RegisterChance(roller, new RHResource(RHResourceType.NEGATIVE, intensity), negativeScore);
        //roller = RegisterChance(roller, new RHResource(RHResourceType.NONE, intensity), neutralScore);
        roller = RegisterChance(roller, new RHResource(RHResourceType.OPINION, intensity), ideaScore);
        roller = RegisterChance(roller, new RHResource(RHResourceType.OPINION, intensity), opinionScore);
        roller = RegisterChance(roller, new RHResource(RHResourceType.QUESTION, intensity), questionScore);

        r = RollResource(roller);
        if (r != null)
        {
            response.addResource(r);
            response.distributeResources();
        }
        return response;
    }
    private RHSGenerateResources GetEmptyGenerateResourcesStatement()
    {
        GameObject go = Instantiate(RHManager.GenerateResourcesPrefab);
        RHSGenerateResources st = go.GetComponent<RHSGenerateResources>();
        Destroy(go);
        return st;
    }
    private bool RandomChanceRange(float max, float value,bool invert)
    {
        return (invert)?Random.Range(0, max) >= value : Random.Range(0,max) < value;
    }

    private Dictionary<float, RHResource> RegisterChance(Dictionary<float, RHResource> oldDict, RHResource newStatement, float weight)
    {
        float sum = 0;
        foreach (float f in oldDict.Keys)
            sum = f;
        oldDict.Add(sum + Mathf.Max(0.1f, weight), newStatement);
        return oldDict;
    }
    private RHResource RollResource(Dictionary<float, RHResource> oldDict)
    {
        float sum = 0;
        foreach (float f in oldDict.Keys)
            sum = f;
        float roll = Random.RandomRange(0f, sum);
        RHResource rolledStatement = null;
        foreach (float f in oldDict.Keys)
        {
            if (roll <= f)
            {
                rolledStatement = oldDict[f];
                break;
            }
        }
        return rolledStatement;
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