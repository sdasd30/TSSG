using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRMyHealthCheck : Transition
{

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (OriginTask.GetTargetObj() == null)
            TriggerTransition();
    }
}
