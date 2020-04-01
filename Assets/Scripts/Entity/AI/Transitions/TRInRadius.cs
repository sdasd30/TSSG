using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRInRadius : Transition
{

    public float InRadiusOf = 10f;

    public override void OnUpdate()
    {
        if (OriginTask == null)
            return;
        if (OriginTask.GetTargetObj() == null)
            TriggerTransition();
        if (Vector3.Distance(transform.position, OriginTask.GetTargetObj().transform.position) < InRadiusOf)
            TriggerTransition();
    }

    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("InRadiusOf", this))
            InRadiusOf = float.Parse(g.GetVariable("InRadiusOf", this));
    }

    public override void OnSave(Goal g)
    {
        g?.SetVariable("InRadiusOf", InRadiusOf.ToString(), this);
    }

}
