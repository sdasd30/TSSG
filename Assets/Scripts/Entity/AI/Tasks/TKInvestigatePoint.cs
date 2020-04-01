using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKInvestigatePoint : Task
{
    public float startuptime = 1f;
    public float tolerance = 0.75f;
    public float searchRadius = 2f;
    public float pauseTime = 2f;
    // Start is called before the first frame update

    private string state = "startup";

    private float m_finishStartupTime = 0f;
    private float m_nextSearchTime = 0f;
    private Vector3 m_nextSearchpoint = new Vector3();

    private const int MAX_SEARCH_TRIES = 5;
    private Vector3 m_targetPoint;
    public override void OnTransition()
    {
        base.OnTransition();
        m_finishStartupTime = Time.timeSinceLevelLoad + startuptime;
        m_nextSearchTime = Time.timeSinceLevelLoad + pauseTime;
        state = "startup";
        m_targetPoint = GetTargetObj().transform.position;
        m_nextSearchpoint = GetTargetObj().transform.position;
    }
    public override void OnActiveUpdate()
    {
        if (GetTargetObj() != null)
            return;

        if (Time.timeSinceLevelLoad > m_finishStartupTime)
        {
            float d = Vector2.Distance(new Vector2(MasterAI.transform.position.x, MasterAI.transform.position.z),
                new Vector2(m_nextSearchpoint.x, m_nextSearchpoint.z));
            if (d > tolerance * 1.2)
            {
                state = "headingToPoint";
                MasterAI.GetComponent<AIBaseMovement>().SetTarget(m_nextSearchpoint, tolerance);
                m_nextSearchTime = -1f;
            } else
            {
                if (m_nextSearchTime == -1f)
                {
                    m_nextSearchTime = Time.timeSinceLevelLoad + pauseTime;
                    state = "pausing";
                } else if (Time.timeSinceLevelLoad > m_nextSearchTime)
                {
                    m_nextSearchpoint = nextSearchPos(m_targetPoint, searchRadius);
                    m_nextSearchTime = -1f;
                }
            }
        }
    }
    private Vector3 nextSearchPos(Vector3 centerPoint, float searchRadius)
    {
        Vector3 nextPos = MasterAI.transform.position;
        bool foundValidPoint = false;
        int numTries = 0;

        while (!foundValidPoint)
        {

            if (numTries > MAX_SEARCH_TRIES)
                break;
            float angle = (MasterAI.GetComponent<Orientation>().LastZ + Random.Range(90f, 270f)) * Mathf.Deg2Rad;
            float dist = Random.Range(0f, searchRadius);
            Vector3 candidatePos = new Vector3(Mathf.Cos(angle) * dist, 0f, Mathf.Sin(angle) * dist) + centerPoint;
            if (MasterAI.GetComponent<Observer>().ScanRayToPoint(candidatePos).Count < 1)
            {
                nextPos = candidatePos;
                break;
            } else
            {
                //Debug.Log("Blocked");  
            }
            numTries++;
        }
        return nextPos;
    }

    public override string debugExtraInfo()
    {
        if (GetTargetObj() != null)
        {
            Vector3 d = GetTargetObj().transform.position - MasterAI.transform.position;
            float dist = Vector3.Magnitude(new Vector3(d.x, 0f, d.z));
            return "St: " + state + " tgt: " + m_targetPoint + " dist: " + d + " m: " + dist + "\n";
        }
        return "No Target set";
    }
}
