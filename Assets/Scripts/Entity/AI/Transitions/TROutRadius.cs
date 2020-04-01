using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TROutRadius : Transition {

	public float OutSideRadiusOf = 10f;

	public override void OnUpdate() {
        if (OriginTask == null)
            return;
		if (OriginTask.GetTargetObj() == null)
            TriggerTransition();
        if (Vector3.Distance(transform.position, OriginTask.GetTargetObj().transform.position) > OutSideRadiusOf)
            TriggerTransition();
    }

    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("OutSideRadiusOf", this))
            OutSideRadiusOf = float.Parse(g.GetVariable("OutSideRadiusOf", this));
    }

    public override void OnSave(Goal g)
    {
        g?.SetVariable("OutSideRadiusOf", OutSideRadiusOf.ToString(), this);
    }

}
