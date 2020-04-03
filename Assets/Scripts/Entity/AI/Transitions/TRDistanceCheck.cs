using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TRDistanceCheck : Transition
{
    public float DistanceGreaterThen = 0f;
    public float DistanceLessThen = 10f;
    
    public override void OnUpdate()
    {
        if (OriginTask == null)
            return;
        if (OriginTask.GetTargetObj() == null)
            return;
        Vector3 tgt = OriginTask.GetTargetObj().transform.position;
        float d = Vector2.Distance(new Vector2(MasterAI.transform.position.x, MasterAI.transform.position.z),
                new Vector2(tgt.x,tgt.z));
        if (d < DistanceLessThen && d > DistanceGreaterThen)
            TriggerTransition();
    }

    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("DistanceLessThen", this))
            DistanceLessThen = float.Parse(g.GetVariable("DistanceLessThen", this));
    }

    public override void OnSave(Goal g)
    {
        g?.SetVariable("Radius", DistanceLessThen.ToString(), this);
    }

}
