using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisguisedObj : LogicalObject
{
    public GameObject DisguisedAsObject;

    public float DisguiseEffectiveness = 1.0f;

    public virtual float IsA(GameObject logicalObj)
    {
        if (Parent == null)
        {

            return (gameObject.name == logicalObj.name) ? 1f : 0f;
        }
        else
        {
            return Parent.GetComponent<LogicalObject>().IsA(logicalObj);
        }
    }

    public float changeDisguiseValue(string eventName, List<Object> obj, GoalDetectDisguise disguiseGoal, float lastDisguiseValue)
    { 
        return 1.0f;
    }
    public float BaseDisguiseValue()
    {
        return 1.0f;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
