using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionDistanceCriteria { WITHIN_DISTANCE_OF, OUTSIDE_DISTANCE_OF}
public class TRSeeAttackableObject : Transition {

    public bool IgnoreNeutral = true;
    // Use this for initialization
    public string IfSeenInZone = "NONE";
    public bool InvertInZoneCondition = false;
    public TransitionDistanceCriteria DistanceCriteria;
    public float WithinDistance = 20f;

    public override void OnSight(Observable o)
    {
        Debug.Log("On sight of : " + o);
        if (o.GetComponent<FactionHolder>() == null ||
            (IgnoreNeutral || o.GetComponent<FactionHolder>().Faction == FactionType.NEUTRAL))
            return;
        if (MasterAI.GetComponent<FactionHolder>().CanAttack(o.GetComponent<FactionHolder>().Faction))
        {
            Debug.Log("Faction is: " + o.GetComponent<FactionHolder>().Faction);
            float d = Vector2.Distance(new Vector2(MasterAI.transform.position.x, MasterAI.transform.position.z),
                new Vector2(o.transform.position.x, o.transform.position.z));
            if (IfSeenInZone != "NONE" && 
                (DistanceCriteria == TransitionDistanceCriteria.WITHIN_DISTANCE_OF && d < WithinDistance ||
                DistanceCriteria == TransitionDistanceCriteria.OUTSIDE_DISTANCE_OF && d > WithinDistance))
            {
                bool inZone = ZoneManager.IsPointInZone(o.transform.position, IfSeenInZone);
                if (inZone && InvertInZoneCondition)
                {
                    return;
                }
                if (!inZone && InvertInZoneCondition)
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
        if (g.ContainsKey("IfSeenInZone", this))
            IfSeenInZone = g.GetVariable("IfSeenInZone", this);
        if (g.ContainsKey("InvertInZoneCondition", this))
            InvertInZoneCondition = (g.GetVariable("InvertInZoneCondition", this) == "TRUE");
        if (g.ContainsKey("IgnoreNeutral", this))
            IgnoreNeutral = bool.Parse(g.GetVariable("IgnoreNeutral", this));
        if (g.ContainsKey("WithinDistance", this))
            WithinDistance = float.Parse(g.GetVariable("WithinDistance", this));
    }

    public override void OnSave(Goal g)
    {
        g?.SetVariable("IfSeenInZone", IfSeenInZone, this);
        g?.SetVariable("InvertInZoneCondition", InvertInZoneCondition ? "TRUE" : "FALSE", this);
        g?.SetVariable("IgnoreNeutral", (IgnoreNeutral).ToString(), this);
        g?.SetVariable("WithinDistance", (WithinDistance).ToString(), this);
    }
}
