using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKWanderZone : Task
{

    public Vector2 WaitTimeRange;
    public Vector2 WanderDistanceRange;
    public string ZoneName = "";

    private bool FoundZone = false;
    private Vector3 nextSpot;
    private float nextMoveTime = 0;
    private const int MAX_TRIES = 10;
    // Start is called before the first frame update
    public void SetZone()
    {
        if (ZoneName != "" && ZoneManager.GetZone(ZoneName) != null)
        {
            SetTargetObj(ZoneManager.GetZone(ZoneName).gameObject);
        }
    }
    // Update is called once per frame
    public override void OnActiveUpdate()
    {
        if (GetTargetObj() == null)
        {
            SetZone();
        } else
        {
            FoundZone = true;
            if (Time.timeSinceLevelLoad > nextMoveTime)
                pickNewDestination();
            if (nextSpot != null)
                MasterAI.GetComponent<AIBaseMovement>().SetTarget(nextSpot);
        }
    }

    private void pickNewDestination()
    {
        int numTries = 0;
        nextSpot = transform.position;
        while (numTries < MAX_TRIES )
        {
            nextSpot = RandomPointFromPoint(MasterAI.transform.position,
                Random.Range(WanderDistanceRange.x, WanderDistanceRange.y));
            //Debug.Log("In zone?: " + Target.GetComponent<Zone>().IsInZone(nextSpot));
            if (GetTargetObj().GetComponent<Zone>().IsInZone(nextSpot))
            {
                //Debug.Log("Next Point: " + nextSpot );
                MasterAI.GetComponent<AIBaseMovement>().SetTarget(nextSpot);
                break;
            }
            numTries++;
        }
        nextMoveTime = Time.timeSinceLevelLoad + Random.Range(WaitTimeRange.x, WaitTimeRange.y);
    }

    private Vector3 RandomPointFromPoint(Vector3 start, float dist)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        //Debug.Log("Transform?: " + start);
        Vector3 newPoint = start + new Vector3(Mathf.Cos(angle) * dist, 0f, Mathf.Sin(angle) * dist);
        //Debug.Log("Next Point: " + newPoint + " angle: " + angle + " dist: " + dist + " ds: " + Mathf.Sin(angle) * dist);
        return newPoint;
    }

    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("WaitTimeRange", this))
        {
            string[] vars = g.GetVariable("WaitTimeRange",this).Split(',');
            WaitTimeRange = new Vector2(float.Parse(vars[0]), float.Parse(vars[1]));
        }
        if (g.ContainsKey("WanderDistanceRange", this))
        {
            string[] vars = g.GetVariable("WanderDistanceRange", this).Split(',');
            WanderDistanceRange = new Vector2(float.Parse(vars[0]), float.Parse(vars[1]));
        }
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("WaitTimeRange", WaitTimeRange.x + "," + WaitTimeRange.y, this);
        g.SetVariable("TriggerWhenInZone", WanderDistanceRange.x + "," + WanderDistanceRange.y, this);
    }

    public override string debugExtraInfo()
    {
        string F = (FoundZone) ? "T" : "F";
        string s = "Fouund: " + F + " Tgt: " + nextSpot + "\n";
        float tim = nextMoveTime - Time.timeSinceLevelLoad;
        s += "Time until move: " + tim;
        return s;
    }
}
