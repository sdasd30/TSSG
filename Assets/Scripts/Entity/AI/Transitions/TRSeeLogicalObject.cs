using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRSeeLogicalObject : Transition
{
    public string ObjectSeen = "none";
    public string IfSeenInZone = "NONE";
    public bool TriggerIfInZone = true;
    public TransitionDistanceCriteria DistanceCriteria;
    public float WithinDistance = 20f;

    public override void OnSight(Observable o)
    {
        if (o.GetComponent<LogicalObject>() && o.GetComponent<LogicalObject>().LogicalName == ObjectSeen)
        {
            if (IfSeenInZone != "NONE")
            {
                bool inZone = ZoneManager.IsPointInZone(o.transform.position, IfSeenInZone);
                float d = Vector2.Distance(new Vector2(MasterAI.transform.position.x, MasterAI.transform.position.z),
                     new Vector2(o.transform.position.x, o.transform.position.z));
                if (DistanceCriteria == TransitionDistanceCriteria.WITHIN_DISTANCE_OF && d < WithinDistance ||
                    DistanceCriteria == TransitionDistanceCriteria.OUTSIDE_DISTANCE_OF && d > WithinDistance)
                    return;
                if (inZone && !TriggerIfInZone)
                {
                    return;
                }
                if (!inZone && TriggerIfInZone)
                {
                    return;
                }
            }
            TargetTask.SetTargetObj(o.gameObject);
            TriggerTransition();
        }
    }
    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("ObjectSeen", this))
            ObjectSeen = g.GetVariable("ObjectSeen", this);
        if (g.ContainsKey("IfSeenInZone", this))
            IfSeenInZone = g.GetVariable("IfSeenInZone", this);
        if (g.ContainsKey("TriggerIfInZone", this))
        {
            string s = g.GetVariable("TriggerIfInZone", this);
            TriggerIfInZone = (s == "TRUE");
        }
        if (g.ContainsKey("WithinDistance", this))
            WithinDistance = float.Parse(g.GetVariable("WithinDistance", this));
    }

    public override void OnSave(Goal g)
    {
        g?.SetVariable("ObjectSeen", ObjectSeen, this);
        g?.SetVariable("IfSeenInZone", IfSeenInZone, this);
        g?.SetVariable("TriggerIfInZone", (TriggerIfInZone)?"TRUE":"FALSE", this);
        g?.SetVariable("WithinDistance", (WithinDistance).ToString(), this);
    }

}
