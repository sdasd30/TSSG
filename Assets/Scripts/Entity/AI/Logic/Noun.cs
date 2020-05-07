using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noun 
{
    protected List<System.Type> m_aliasTypes = new List<System.Type>();

    private float m_baseImpressionValue = 0.0f;

    public virtual float IsA(GameObject targetObject, Observer perspective)
    {
        return calculateObserverImpression(targetObject.name,perspective);
    }

    public virtual bool IsA(Noun b)
    {
        foreach (System.Type t in m_aliasTypes)
        {
            if (LogicManager.GetNoun(t) == b || LogicManager.GetNoun(t).IsA(b))
                return true;
        }
        return false;
    }

    public virtual void ReactToEvent(AIEvent newEvent, Observer observer) { }

    private float calculateObserverImpression(string targetName, Observer perspective)
    {
        if (perspective.ContainsImpression(targetName, this))
        {
            return m_baseImpressionValue + perspective.GetImpressionModifiers(targetName, this);
        }
        return 0.0f;
    }
}
