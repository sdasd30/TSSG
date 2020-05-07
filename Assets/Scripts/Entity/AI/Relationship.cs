using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationship
{
    private Observer m_observer;
    public Relationship(Observer observer)
    {
        m_observer = observer;
    }
    private Dictionary<Noun, List<ImpressionModifier>> m_impressionEvaluation = new Dictionary<Noun, List<ImpressionModifier>>();

    private Dictionary<System.Type, Noun> m_typeLookup = new Dictionary<System.Type, Noun>();
    // Start is called before the first frame update
    private void updateImpressionModifiers()
    {
        Dictionary<Noun, List<ImpressionModifier>> ndict = new Dictionary<Noun, List<ImpressionModifier>>();
        foreach (Noun gm in m_impressionEvaluation.Keys)
        {
            if (gm == null)
                continue;
            List<ImpressionModifier> dml = m_impressionEvaluation[gm];
            List<ImpressionModifier> nl = new List<ImpressionModifier>();
            foreach (ImpressionModifier dm in dml)
            {
                if (!dm.is_expired())
                    nl.Add(dm);
            }
            ndict[gm] = nl;
        }
    }
    public bool containsImpression(Noun i)
    {
        return m_typeLookup.ContainsKey(i.GetType());
    }
    public float GetImpressionModifiers(Noun imp)
    {
        Noun targetI;
        if (!m_typeLookup.ContainsKey(imp.GetType()))
        {
            foreach(Noun i in m_impressionEvaluation.Keys)
            {
                if (LogicManager.IsA(i,imp,m_observer))
                {
                    targetI = i;
                    continue;
                }
            }
            return 0.0f;
        } else
        {
            targetI = m_typeLookup[imp.GetType()];
        }
        updateImpressionModifiers();
        float value = 0;

        foreach (ImpressionModifier dm in m_impressionEvaluation[targetI])
        {
            value += dm.getModValue();
        }
        return value;
    }
    public void AddModifier(Noun i, ImpressionModifier newDM)
    {
        if (!m_typeLookup.ContainsKey(i.GetType()))
        {
            m_impressionEvaluation[i] = new List<ImpressionModifier>();
        }
            
        updateImpressionModifiers();
        foreach (ImpressionModifier dm in m_impressionEvaluation[i])
        {
            if (dm.ID == newDM.ID)
            {
                dm.attempt_stack(newDM.CurrentStack);
                return;
            }
        }
        m_impressionEvaluation[i].Add(newDM);
        m_typeLookup[i.GetType()] = i;
    }
    public void ClearModifier(Noun i, ImpressionModifier newDM)
    {
        if (!m_typeLookup.ContainsKey(i.GetType()))
            return;
        updateImpressionModifiers();
        List<ImpressionModifier> newL = new List<ImpressionModifier>();
        foreach (ImpressionModifier dm in m_impressionEvaluation[i])
        {
            if (dm.ID != newDM.ID)
                newL.Add(dm);
        }
        m_impressionEvaluation[i] = newL;
        m_typeLookup[i.GetType()] = null;
    }

}
