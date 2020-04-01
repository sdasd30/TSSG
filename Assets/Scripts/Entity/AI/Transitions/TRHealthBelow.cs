using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRHealthBelow : Transition
{

    public float HealthPercentage = 0.5f;


    public override void OnHit(HitInfo hb)
    {
        base.OnHit(hb);
        float hp = MasterAI.GetComponent<Attackable>().Health;
        float maxHP = MasterAI.GetComponent<Attackable>().MaxHealth;
        if (hp / maxHP < HealthPercentage)
            TriggerTransition();
    }
    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("HealthPercentage", this))
            HealthPercentage = int.Parse(g.GetVariable("HealthPercentage", this));
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("HealthPercentage", HealthPercentage.ToString(), this);
    }
}
