using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TROutOfSight : Transition
{
    public float TimeTriggerThreashold = 5f;

    private GameObject targetObj;
    private float nextTimeTrigger = 0f;
    private bool inSight = true;
    public override void OnStart()
    {
        base.OnStart();
        targetObj = OriginTask.GetTargetObj();
        inSight = true;
        nextTimeTrigger = Time.timeSinceLevelLoad + 10000f;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!inSight && Time.timeSinceLevelLoad > nextTimeTrigger)
            TriggerTransition();
    }
    public override void OutOfSight(Observable o)
    {
        base.OutOfSight(o);
        inSight = false;
        if (o == targetObj)
            nextTimeTrigger = Time.timeSinceLevelLoad + TimeTriggerThreashold;
    }
    public override void OnSight(Observable o)
    {
        base.OutOfSight(o);
        inSight = true;
    }
    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("TimeTriggerThreashold", this))
            TimeTriggerThreashold = float.Parse(g.GetVariable("TimeTriggerThreashold", this));
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("TimeTriggerThreashold", TimeTriggerThreashold.ToString(), this);
    }
}
