using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRAttackedBy : Transition
{

    public float DamageThreashould = 5f;

    public override void OnHit(HitInfo hb)
    {
        base.OnHit(hb);
        if (hb.Damage > DamageThreashould)
        {
            TargetTask.SetTargetObj(hb.mHitbox.gameObject);
            TriggerTransition();
        }
    }
    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("HealthPercentage", this))
            DamageThreashould = float.Parse(g.GetVariable("HealthPercentage", this));
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("HealthPercentage", DamageThreashould.ToString(), this);
    }
}
