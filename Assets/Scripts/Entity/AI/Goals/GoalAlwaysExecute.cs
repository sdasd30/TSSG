using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAlwaysExecute : Goal
{
    AIBehaviour b;
    void Awake()
    {
        init();
    }

    protected override void init()
    {
        base.init();


          
        GameObject g = (GameObject)Resources.Load(GoalVariables["ExecutePrefab"]);
        if (g != null)
        {
            b = new AIBehaviour("AutoSet", g, this, 10000.0f);
        } else if (GoalVariables["ExecutePrefab"] == "ExecuteThisBehaviour")
        {
            b = new AIBehaviour("AutoSet", gameObject, this, 10000.0f);
        }
    }
    public override void OnStart()
    {
        if (b != null)
        {
            m_masterAI.ProposeNewBehaviour(b);
        }
    }
    public override void OnEnterZone(Zone z)
    {
    }
    protected override void initVariableDictionary() {
        base.initVariableDictionary();
        if (!GoalVariables.ContainsKey("ExecutePrefab"))
        {
            GoalVariables["ExecutePrefab"] = "ExecuteThisBehaviour";
        }

    }
}
