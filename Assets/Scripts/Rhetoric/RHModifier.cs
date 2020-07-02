using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RHScaleType { ADDITION_FLAT, ADDITION_PROPORTIONAL, MULTIPLICATION }
public enum RHScaleModifier { FAVOR, AUTHORITY, EMOTIONS, TRUST, CURRENT_PERSUASION_LEVEL, INSTANCES_OF_ETHOS, INSTANCES_OF_LOGOS, INSTANCES_OF_PATHOS, INSTANCES_OF_SELF };
public enum RHStat { FAVOR, AUTHORITY, EMOTIONS, TRUST, CURRENT_PERSUASION_LEVEL };

[System.Serializable]
public class RHModifier
{

    public string m_nameOfModifier;
    public RHScaleType m_scaleType;
    public RHScaleModifier m_scaleModifier;

    public float initialBonusValue = 0.0f;
    public float scaleZeroPoint = 0f;
    public float scaleFactor = 0.1f;
    public bool invertIfNegative = true;
    public float MaxDiff = 1.0f;
    public float MinDiff = -1.0f;
    public RHStatement statement;

    public RHModifier(string name, float initValue, float scaleFactor, RHScaleModifier modifierType, float zeroPoint, RHScaleType scaleType = RHScaleType.ADDITION_FLAT)
    {
        m_nameOfModifier = name;
        m_scaleModifier = modifierType;
        initialBonusValue = initValue;
        scaleZeroPoint = zeroPoint;
        this.scaleFactor = scaleFactor;
        m_scaleType = scaleType;
    }
    public virtual float ApplyModifier(float initialDiff, RHConversation c, RHListener l)
    {
        float scaleFactor = calculateScaleFactor(m_scaleModifier, c, l);
        return calculateScaleValue(initialDiff, scaleFactor);
    }
    public float getDiff(float initialDiff, RHConversation c, RHListener l)
    {
        return initialDiff - ApplyModifier(initialDiff, c, l);
    }

    public string getHoverString(float initialDiff, RHConversation c, RHListener l)
    {
        float d = getDiff(initialDiff, c, l);
        string hoverStr = "";
        if (d > 0)
            hoverStr += "+";
        else
            hoverStr += "-";
        hoverStr += d.ToString() + " (" + m_nameOfModifier + ")";
        return hoverStr;
    }

    public void SetDifferenceLimits(float min, float max)
    {
        MaxDiff = max;
        MinDiff = min;
    }

    protected virtual float calculateScaleFactor(RHScaleModifier mod, RHConversation c, RHListener l)
    {
        switch (mod)
        {
            case RHScaleModifier.FAVOR:
                return l.GetFavor(c.Speakers[0]);
            case RHScaleModifier.AUTHORITY:
                return l.GetAuthority(c.Speakers[0]);
            case RHScaleModifier.EMOTIONS:
                return l.GetEmotionalIntensity();
            case RHScaleModifier.TRUST:
                return l.GetTrust(c.Speakers[0]); ;
            case RHScaleModifier.CURRENT_PERSUASION_LEVEL:
                return c.Listeners[l];
            case RHScaleModifier.INSTANCES_OF_ETHOS:
                return countOfType(RHType.ETHOS, c.PreviousStatements);
            case RHScaleModifier.INSTANCES_OF_LOGOS:
                return countOfType(RHType.LOGOS, c.PreviousStatements);
            case RHScaleModifier.INSTANCES_OF_PATHOS:
                return countOfType(RHType.PATHOS, c.PreviousStatements);
            case RHScaleModifier.INSTANCES_OF_SELF:
                return countType(statement.StatementName, c.PreviousStatements);
        }
        return 0f;
    }

    protected int countOfType(RHType t, List<RHStatement> oldStatements)
    {
        int numberInstances = 0;
        foreach (RHStatement s in oldStatements)
        {
            if (s.RhetoricType == t)
            {
                numberInstances++;
            }
        }
        return numberInstances;
    }
    protected int countType(string n, List<RHStatement> oldStatements)
    {
        int numberInstances = 0;
        foreach (RHStatement s in oldStatements)
        {
            if (s.StatementName == n)
            {
                numberInstances++;
            }
        }
        return numberInstances;
    }
    private float calculateScaleValue(float initial, float scalevalue)
    {
        float diff = scalevalue - scaleZeroPoint;
        if (diff < 0 && !invertIfNegative)
            return initial;
        if (m_scaleType == RHScaleType.ADDITION_FLAT)
            return initial + Mathf.Min(MaxDiff, Mathf.Max(MinDiff, (initialBonusValue + diff * scaleFactor)));
        if (m_scaleType == RHScaleType.ADDITION_PROPORTIONAL)
            return initial + (initial * Mathf.Min(MaxDiff, Mathf.Max(MinDiff, (initialBonusValue + diff * scaleFactor))));
        else
        {
            return initial * (initialBonusValue + ( diff * scaleFactor));
        }
    }
}