using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKAvoidObject : Task
{
    public float AvoidPrecision = 0.5f;
    public float AvoidDistance = 5f;
    // Start is called before the first frame update
    public override void OnActiveUpdate()
    {
        if (GetTargetObj() != null)
        {
            GameObject tgt = GetTargetObj();
            Vector3 d = tgt.transform.position - MasterAI.transform.position;
            float dist = Vector3.Magnitude(new Vector3(d.x, 0f, d.z));
            if (dist < AvoidDistance)
            {
                MasterAI.GetComponent<AIBaseMovement>().SetTarget(MasterAI.transform.position - d, AvoidPrecision);
            }
            
        }
    }

    public override string debugExtraInfo()
    {
        if (GetTargetObj() != null)
        {
            Vector3 d = GetTargetObj().transform.position - MasterAI.transform.position;
            float dist = Vector3.Magnitude(new Vector3(d.x, 0f, d.z));
            return "Avd: " + GetTargetObj().name + " dist: " + d + " m: " + dist + "\n";
        }
        return "No Target set";
    }
}
