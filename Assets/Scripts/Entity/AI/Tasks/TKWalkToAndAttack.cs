using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKWalkToAndAttack : Task
{
    public float startuptime = 1f;
    public float DesiredDistanceToTarget = 4f;
    public float ShootDistance = 5f;
    public float FirePauseTime = 1f;
    public float HoldTriggerTime = 0.2f;
    public string UseItemNameOrSlot = "primary";
    private string state = "startup";

    private float m_finishStartupTime = 0f;
    private float m_nextFireTime = 0f;
    public override void OnTransition()
    {
        base.OnTransition();
        m_finishStartupTime = Time.timeSinceLevelLoad + startuptime;
        m_nextFireTime = Time.timeSinceLevelLoad + FirePauseTime;
        state = "startup";
    }

    public override void OnActiveUpdate()
    {
        if (GetTargetObj() == null)
            return;

        if (Time.timeSinceLevelLoad > m_finishStartupTime)
        {
            Vector3 targetPos = GetTargetObj().transform.position;
            float d = MasterAI.get2DDistanceToPoint(targetPos);
            if (d > DesiredDistanceToTarget * 1.2)
            {
                state = "headingToPoint";
                MasterAI.GetComponent<AIBaseMovement>().SetTarget(targetPos, DesiredDistanceToTarget);
                m_nextFireTime = Time.timeSinceLevelLoad + m_nextFireTime;
            } else
            {
                MasterAI.GetComponent<Orientation>().FacePoint(GetTargetObj().transform.position);
            }
            if (d < ShootDistance)
            {
                state = "firing";
                if (Time.timeSinceLevelLoad > m_nextFireTime)
                {
                    MasterAI.GetComponent<AIBaseMovement>().UseItem(UseItemNameOrSlot, new Vector2(), HoldTriggerTime);
                    m_nextFireTime = Time.timeSinceLevelLoad + FirePauseTime;
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
            float t = m_nextFireTime - Time.timeSinceLevelLoad;
            return "st: " + state + " Tgt: " + GetTargetObj().name + " d: " + dist + " timeToShot: " + t + "\n";
        }
        return "No Target set";
    }
}
