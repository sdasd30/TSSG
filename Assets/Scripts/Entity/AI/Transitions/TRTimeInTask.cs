using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRTimeInTask : Transition
{

    public float TimeTriggerThreashold = 5f;

    private GameObject targetObj;
    private float nextTimeTrigger = 0f;
    public override void OnStart()
    {
        base.OnStart();
        targetObj = OriginTask.GetTargetObj();

        nextTimeTrigger = Time.timeSinceLevelLoad + nextTimeTrigger;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.timeSinceLevelLoad > nextTimeTrigger)
            TriggerTransition();
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
