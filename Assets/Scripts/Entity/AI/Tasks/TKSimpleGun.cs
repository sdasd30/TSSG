using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKSimpleGun : Task
{
    public float startuptime = 1f;
    public float targetWalkPoint = 4f;
    public float shootDistance = 5f;
    public float firePauseTime = 1f;
    public float fireHoldTime = 0.2f;
    public string UseItemNameOrSlot = "primary";
    private string state = "startup";

    private float m_finishStartupTime = 0f;
    private float m_nextFireTime = 0f;
    public override void OnTransition()
    {
        base.OnTransition();
        m_finishStartupTime = Time.timeSinceLevelLoad + startuptime;
        m_nextFireTime = Time.timeSinceLevelLoad + firePauseTime;
        state = "startup";
    }

    public override void OnActiveUpdate()
    {
        if (GetTargetObj() == null)
            return;

        if (Time.timeSinceLevelLoad > m_finishStartupTime)
        {
            Vector3 targetPos = GetTargetObj().transform.position;
            float d = Vector2.Distance(new Vector2(MasterAI.transform.position.x, MasterAI.transform.position.z),
                new Vector2(targetPos.x, targetPos.z));
            if (d > targetWalkPoint * 1.2)
            {
                state = "headingToPoint";
                MasterAI.GetComponent<AIBaseMovement>().SetTarget(targetPos, targetWalkPoint);
                m_nextFireTime = Time.timeSinceLevelLoad + m_nextFireTime;
            } else
            {
                MasterAI.GetComponent<Orientation>().FacePoint(GetTargetObj().transform.position);
            }
            if (d < shootDistance)
            {
                if (Time.timeSinceLevelLoad > m_nextFireTime)
                {
                    state = "firing";
                    
                    MasterAI.GetComponent<AIBaseMovement>().UseItem(UseItemNameOrSlot, new Vector2(), fireHoldTime);
                    m_nextFireTime = Time.timeSinceLevelLoad + firePauseTime;
                }
            }
        }
    }

    public override string debugExtraInfo()
    {
        if (GetTargetObj() != null)
        {
            Vector3 d = GetTargetObj().transform.position - MasterAI.transform.position;
            float dist = Vector3.Magnitude(new Vector3(d.x, 0f, d.z));
            return "st: " + state + " Tgt: " + GetTargetObj().name + " dist: " + d + " m: " + dist + "\n";
        }
        return "No Target set";
    }
}
