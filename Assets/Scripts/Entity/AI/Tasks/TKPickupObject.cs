using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKPickupObject : Task
{
    public override void OnActiveUpdate()
    {
        if (GetTargetObj() != null)
            MasterAI.GetComponent<AIBaseMovement>().SetTarget(GetTargetObj().transform.position);
    }
}
