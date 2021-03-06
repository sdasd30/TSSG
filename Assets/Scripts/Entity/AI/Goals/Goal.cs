﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Goal : MonoBehaviour
{
    protected AITaskManager m_masterAI;

    private float m_goalPriority;
    private Dictionary<System.Type, List<AIEventCallback>> m_events;

    public virtual float GoalPriority { get { return m_goalPriority; } private set { m_goalPriority = value; } }

    public virtual string GoalName { get { return "DefaultGoal"; } }

    public virtual void OnItemGet(Item i) { }
    public virtual void OnItemLost(InventoryItemData i) { }

    public virtual void OnHit(HitInfo hb) { }

    public virtual void OnSight(Observable o) { }

    public virtual void OutOfSight(Observable o) { }

    public virtual void OnStart() { }

    public virtual void OnTime() { }

    public virtual void OnEnterZone(Zone z) { }

    public virtual void OnExitZone(Zone z) { }

    protected int NumGoalVariables = 0;
    [SerializeField]
    protected StringDictionary GoalVariables;

    public void SetGoalPriority(float newP)
    {
        m_goalPriority = newP;
    }
    public void SetMasterAI(AITaskManager master)
    {
        m_masterAI = master;
    }
    
    public string ExportString()
    {
        string s = this.GetType().ToString();
        s = s + "|";
        s += name + "|";
        foreach (string k in GoalVariables.Keys)
        {
            s = s + k + ":" + GoalVariables[k] + "|";
        }
        return s;
    }

    public void InitializeVars(string[] initList)
    {
        if (GoalVariables == null)
            GoalVariables = new StringDictionary();
        name = initList[1];
        foreach (string s in initList)
        {
            var keyValueCombo = s.Split(':');
            if (keyValueCombo.Length > 1)
            {
                GoalVariables[keyValueCombo[0]] = keyValueCombo[1];
            }
        }
    }
    public string outputDebugString()
    {
        string s = "";
        foreach (string k in GoalVariables.Keys)
        {
            s = s + k + ":" + GoalVariables[k] + "\n";
        }
        return s;
    }
    void Start()
    {
        init();
    }

    protected virtual void initVariableDictionary() {
        int c = GoalVariables.Keys.Count;
        for (int i = c; i < NumGoalVariables; i++)
        {
            GoalVariables.Add(i.ToString(), "");
        }
    }

    protected virtual void init() {
        m_events = new Dictionary<System.Type, List<AIEventCallback>>();
        if (GoalVariables == null)
            GoalVariables = new StringDictionary();
        initVariableDictionary();
    }

    public void SetVariable(string key, string value, Task origin)
    {
        GoalVariables[origin.GetType() + "-" + origin.ParentGoal.name + "-" + key] = value;
    }
    public string GetVariable(string key, Task origin)
    {
        if (!ContainsKey(key,origin))
            return "";
        return GoalVariables[origin.GetType() + "-" + origin.ParentGoal.name + "-" + key];
    }
    public bool ContainsKey(string key, Task origin)
    {
        string realKey = origin.GetType() + "-" + origin.ParentGoal.name + "-" + key;
        return GoalVariables.ContainsKey(realKey);
    }
    public void SetVariable(string key, string value, Transition origin)
    {
        GoalVariables[origin.GetType() + "-" + origin.ParentGoal.name + "-" + key] = value;
    }

    public string GetVariable(string key, Transition origin)
    {
        if (!ContainsKey(key, origin))
            return "";
        return GoalVariables[origin.GetType() + "-" + origin.ParentGoal.name + "-" + key];
    }
    public bool ContainsKey(string key, Transition origin)
    {
        return GoalVariables.ContainsKey(origin.GetType() + "-" + origin.ParentGoal.name + "-" + key);
    }

    //public virtual void triggerEvent(string eventName, List<System.Object> args)
    //{
    //    if (!m_events.ContainsKey(eventName))
    //        return;
    //    foreach (AITaskCallback f in m_events[eventName])
    //    {
    //        f(args);
    //    }
    //}
    public virtual void triggerEvent(AIEvent aie)
    {
        if (!m_events.ContainsKey(aie.GetType()))
            return;
        foreach (AIEventCallback f in m_events[aie.GetType()])
        {
            f(aie);
        }
    }
    public void registerEvent(System.Type eventType, AIEventCallback callbackFunction)
    {
        if (!m_events.ContainsKey(eventType))
            m_events[eventType] = new List<AIEventCallback>();
        m_events[eventType].Add(callbackFunction);
    }
    public void deregisterEvent(System.Type eventType, AIEventCallback callbackFunction)
    {
        if (!m_events.ContainsKey(eventType))
            return;
        List<AIEventCallback> m_newList = new List<AIEventCallback>();
        foreach (AIEventCallback f in m_events[eventType])
        {
            if (f != callbackFunction)
            {
                m_newList.Add(f);
            }
        }
        m_events[eventType] = m_newList;
    }
}
