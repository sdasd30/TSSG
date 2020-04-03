using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalExecuteOnStart : Goal
{
    public GameObject Behaviour;
    public float Priority = 10000f;
    AIBehaviour b;
    void Awake()
    {
        init();
    }

    protected override void init()
    {
        base.init();
        GameObject g;
        if (Behaviour == null)
        {
            g = GameObject.Find((GoalVariables["ExecutePrefab"]));
            if (g == null)
                g = (GameObject)Resources.Load(GoalVariables["ExecutePrefab"]);
        } else
        {
            g = Behaviour;
            GoalVariables["ExecutePrefab"] = Behaviour.name;
        }

          
        
        if (g != null)
        {
            b = new AIBehaviour("AutoSet", g, this, Priority);
        } else if (GoalVariables["ExecutePrefab"] == "ExecuteThisBehaviour")
        {
            b = new AIBehaviour("AutoSet", gameObject, this, Priority);
        }
    }
    public override void OnStart()
    {
        if (b != null)
        {
            m_masterAI.AddNewBehaviour(b);
        }
    }
    public override void OnEnterZone(Zone z)
    {
    }
    protected override void initVariableDictionary() {
        base.initVariableDictionary();
        if (!GoalVariables.ContainsKey("ExecutePrefab"))
        {
            GoalVariables["ExecutePrefab"] = "NoPrefabSet";
        }

    }
}
