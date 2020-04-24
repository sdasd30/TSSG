using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetectDisguise : Goal
{
    private Dictionary<GameObject, float> disguiseEvaluation = new Dictionary<GameObject, float>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void triggerEvent(string eventName, List<Object> args)
    {
        Object o = args[0];
        var go = o as GameObject;
        if (go == null || go.GetComponent<DisguisedObj>() == null)
            return;
        go.GetComponent<DisguisedObj>()


    }
}
