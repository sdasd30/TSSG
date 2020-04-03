using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRTargetHealth : Transition
{
    public float HealthPercentageAbove = 0f;
    public float HealthPercentageBelow = 0.5f;
    public override void OnHit(HitInfo hb)
    {
        base.OnHit(hb);
        if (OriginTask.GetTargetObj() == null)
            return;
        float hp = OriginTask.GetTargetObj().GetComponent<Attackable>().Health;
        float maxHP = OriginTask.GetTargetObj().GetComponent<Attackable>().MaxHealth;
        float healthPercentage = hp / maxHP;
        if (healthPercentage > HealthPercentageAbove &&
            healthPercentage < HealthPercentageBelow)
            TriggerTransition();
    }
    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("HealthPercentageBelow", this))
            HealthPercentageBelow = int.Parse(g.GetVariable("HealthPercentageBelow", this));
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("HealthPercentageBelow", HealthPercentageBelow.ToString(), this);
    }
}
