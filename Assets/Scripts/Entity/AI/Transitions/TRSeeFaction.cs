using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneCondition { DO_NOT_CHECK_FOR_ZONE, IF_TARGET_IN_ZONE, IF_TARGET_OUT_OF_ZONE}

public enum MyZoneCondition { DO_NOT_CHECK_FOR_ZONE, I_AM_IN_ZONE, I_AM_OUT_OF_ZONE }
public class TRSeeFaction : Transition
{
    public FactionType TriggeringFaction;
    
    public ZoneCondition TargetZoneCondition;
    public string ZoneName = "";

    public MyZoneCondition MyZoneCondition;
    public string MyZoneName = "";

    public TransitionDistanceCriteria DistanceCriteria;
    public float Distance = 20f;

    public override void OnSight(Observable o)
    {
        Debug.Log("On sight of : " + o);
        if (ValidObject(o))
        {
            Debug.Log("Faction is: " + TriggeringFaction);
            float d = Vector2.Distance(new Vector2(MasterAI.transform.position.x, MasterAI.transform.position.z),
                new Vector2(o.transform.position.x, o.transform.position.z));
            if (DistanceCriteria == TransitionDistanceCriteria.WITHIN_DISTANCE_OF && d < Distance ||
                DistanceCriteria == TransitionDistanceCriteria.OUTSIDE_DISTANCE_OF && d > Distance)
                return;

            if (TargetZoneCondition != ZoneCondition.DO_NOT_CHECK_FOR_ZONE)
            {
                bool inZone = ZoneManager.IsPointInZone(o.transform.position, ZoneName);
                if ((!inZone && TargetZoneCondition != ZoneCondition.IF_TARGET_IN_ZONE) ||
                    (inZone && TargetZoneCondition != ZoneCondition.IF_TARGET_OUT_OF_ZONE))
                {
                    return;
                }
            }
            if (MyZoneCondition != MyZoneCondition.DO_NOT_CHECK_FOR_ZONE)
            {
                bool inZone = ZoneManager.IsPointInZone(MasterAI.transform.position, MyZoneName);
                if ((!inZone && MyZoneCondition != MyZoneCondition.I_AM_IN_ZONE) ||
                    (inZone && MyZoneCondition != MyZoneCondition.I_AM_OUT_OF_ZONE))
                {
                    return;
                }
            }
            TargetTask.SetTargetObj(o.gameObject);
            TriggerTransition();
        }
    }

    public virtual bool ValidObject(Observable o)
    {
        if (o.GetComponent<FactionHolder>() == null)
            return false;
        return (o.GetComponent<FactionHolder>().Faction == TriggeringFaction);
    }
    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("IfSeenInZone", this))
            ZoneName = g.GetVariable("IfSeenInZone", this);
        if (g.ContainsKey("InvertInZoneCondition", this))
            TargetZoneCondition = (ZoneCondition)int.Parse(g.GetVariable("InvertInZoneCondition",this));
        if (g.ContainsKey("TriggeringFaction", this))
            TriggeringFaction = (FactionType)int.Parse(g.GetVariable("TriggeringFaction", this));
        if (g.ContainsKey("Distance", this))
            Distance = float.Parse(g.GetVariable("Distance", this));
    }

    public override void OnSave(Goal g)
    {
        g?.SetVariable("IfSeenInZone", ZoneName, this);
        g?.SetVariable("InvertInZoneCondition", TargetZoneCondition.ToString(), this);
        g?.SetVariable("TriggeringFaction", ((int)TriggeringFaction).ToString(), this);
        g?.SetVariable("Distance", (Distance).ToString(), this);
    }
}
