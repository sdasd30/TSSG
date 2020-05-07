using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;

public delegate float NounIsAFunction(GameObject go, Observer perspective);
public class LogicManager : MonoBehaviour
{
    private static LogicManager m_instance;

    private Dictionary<string,Verb> m_Verbs = new Dictionary<string, Verb>();
    private Dictionary<string,Noun> m_Nouns = new Dictionary<string, Noun>();

    public static LogicManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        foreach (System.Type type in
            Assembly.GetAssembly(typeof(Verb)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Verb))))
        {
            Verb v = (Verb)Activator.CreateInstance(type);
            m_Verbs.Add(v.GetType().ToString(),v);
        }
        foreach (System.Type type in
            Assembly.GetAssembly(typeof(Noun)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Noun))))
        {
            Noun n = (Noun)Activator.CreateInstance(type);
            Debug.Log(n.GetType().ToString());
            m_Nouns.Add(n.GetType().ToString(),n);
        }
    }

    public static bool IsA(Noun n1, Noun n2, Observer perspective)
    {
        return n1.IsA(n2);
    }
    public static bool IsA(ActionInfo action, string verbName, Observer perspective, float threashold = 0f)
    {
        if (m_instance.m_Verbs.ContainsKey(verbName))
        {
            return IsA(action, m_instance.m_Verbs[verbName], perspective, threashold);
        }
        return false;
    }
    public static bool IsA(ActionInfo action, System.Type verbType, Observer perspective, float threashold = 0f)
    {
        if (m_instance.m_Verbs.ContainsKey(verbType.ToString())) {
            return IsA(action, m_instance.m_Verbs[verbType.ToString()], perspective,threashold);
        }
        return false;
    }
    public static bool IsA(ActionInfo action, Verb actionCategory, Observer perspective, float threashold = 0f)
    {
        return (getIsAConfidence(action, actionCategory, perspective) > threashold);
    }
    public static float getIsAConfidence(ActionInfo action, Verb actionCategory, Observer perspective, float threashold = 0f)
    {
        return actionCategory.IsA(action,perspective);
    }


    public static bool IsA(GameObject action, string nounName, Observer perspective, float threashold = 0f)
    {
        if (m_instance.m_Verbs.ContainsKey(nounName))
        {
            return IsA(action, m_instance.m_Nouns[nounName], perspective);
        }
        return false;
    }
    public static bool IsA(GameObject target, System.Type nounType, Observer perspective,float threashold = 0f)
    {
        if (m_instance.m_Nouns.ContainsKey(nounType.ToString())) {
            return IsA(target, m_instance.m_Nouns[nounType.ToString()], perspective,threashold);
        }
        return false;
    }
    public static float getIsAConfidence(GameObject go, Noun n, Observer perspective)
    {
        return n.IsA(go, perspective);
    }
    public static bool IsA(GameObject go, Noun n, Observer perspective,float threashold = 0f)
    {
        return (getIsAConfidence(go, n, perspective) > threashold);
    }

    public static Noun GetNoun(string nounName)
    {
        if (!m_instance.m_Nouns.ContainsKey(nounName))
            return null;
        return m_instance.m_Nouns[nounName];
    }
    public static Noun GetNewNoun(string nounName)
    {
        System.Type type = Type.GetType(nounName);
        if (type == null)
        {
            Debug.LogError("Could not find Noun of type: " + nounName);
            return null;
        }
        Noun n = (Noun)Activator.CreateInstance(type);
        return n;
    }
    public static Verb GetVerb(string verbName)
    {
        if (!m_instance.m_Verbs.ContainsKey(verbName))
            return null;
        return m_instance.m_Verbs[verbName];
    }

    public static Noun GetNoun(System.Type nounType)
    {
        string s = nounType.ToString();
        return GetNoun(s);
    }

    public static Verb GetVerb(System.Type verbType)
    {
        string s = verbType.ToString();
        return GetVerb(s);
    }

}
