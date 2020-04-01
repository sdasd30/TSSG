using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKGotoObject : Task
{
    public float tolerance = 1f;
    public override void OnActiveUpdate()
    {
        if (GetTargetObj() != null)
            MasterAI.GetComponent<AIBaseMovement>().SetTarget(GetTargetObj().transform.position, tolerance);
    }

    public override string debugExtraInfo()
    {
        if (GetTargetObj() != null)
        {
            Vector3 d = GetTargetObj().transform.position - MasterAI.transform.position;
            float dist = Vector3.Magnitude(new Vector3(d.x, 0f, d.z));
            return "Tgt: " + GetTargetObj().name + " dist: " + d + " m: " + dist + "\n";
        }
        return "No Target set";
    }
}
