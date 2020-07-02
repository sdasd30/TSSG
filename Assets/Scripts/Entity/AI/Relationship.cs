using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relationship
{
    private Observer m_observer;
    private bool m_isDirty;
    public virtual bool IsDirty { get { return m_isDirty; } private set { m_isDirty = value; } }
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
        if (!m_impressionEvaluation.ContainsKey(targetI))
            return 0f;
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
            Debug.Log("Adding: " + i.GetType());
            m_typeLookup[i.GetType()] = i;
            m_impressionEvaluation[i] = new List<ImpressionModifier>();
        }
        i = m_typeLookup[i.GetType()];
        Debug.Log("I is: " + i);
        updateImpressionModifiers();
        foreach (ImpressionModifier dm in m_impressionEvaluation[i])
        {
            if (dm.ID == newDM.ID)
            {
                m_isDirty = m_isDirty || dm.attempt_stack(newDM.CurrentStack);
                return;
            }
        }
        m_isDirty = true;
        m_impressionEvaluation[i].Add(newDM);
        Debug.Log("I is: " + i);

    }
    public void ClearModifier(Noun i, ImpressionModifier newDM)
    {
        if (!m_typeLookup.ContainsKey(i.GetType()))
        {
            return;
        }
        Debug.Log("Attempting to remove: " + i.GetType());
        if (m_typeLookup[i.GetType()] == null)
            m_typeLookup[i.GetType()] = i;
        i = m_typeLookup[i.GetType()];
        
        Debug.Log("Found: " + i);
        updateImpressionModifiers();
        
        List<ImpressionModifier> newL = new List<ImpressionModifier>();
        if (!m_impressionEvaluation.ContainsKey(i))
        {
            m_impressionEvaluation[i] = newL;
            return;
        }

        foreach (ImpressionModifier dm in m_impressionEvaluation[i])
        {
            if (dm.ID != newDM.ID)
                newL.Add(dm);
        }
        m_isDirty = true;
        m_impressionEvaluation[i] = newL;
    }

}
