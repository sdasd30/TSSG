﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {NEUTRAL,AGGRESSIVE,ATTACK};

public class Task : MonoBehaviour {
    [HideInInspector]
    public AITaskManager MasterAI;

	public TaskType MyTaskType;
	public bool IsInitialTask = false;

	private bool m_active = false;

    [HideInInspector]
    public List<Transition> TransitionsTo;
    [HideInInspector]
    public List<Transition> TransitionsFrom;

    [HideInInspector]
    public Goal ParentGoal;
    [HideInInspector]
    public string ParentBehaviour;
	[HideInInspector]
	public float TaskPriority;

	public void Init() {
		TransitionsTo = new List<Transition> ();
		TransitionsFrom = new List<Transition> ();

		foreach (Transition t in GetComponents<Transition>()) {
			if (t.TypeOfTransition == TransitionType.FROM_THIS_TASK) {
				t.OriginTask = this;
				TransitionsFrom.Add (t);
			} else {
				t.TargetTask = this;
				TransitionsTo.Add (t);
			}
		}
	}
	
	// Update is called once per frame
	public void OnUpdate () {
		foreach (Transition t in TransitionsFrom) {
			if (t.isActiveAndEnabled)
				t.OnUpdate ();
		}
		if (m_active) {
			OnActiveUpdate ();
		}
	}

	public void OnSight(Observable o) { 
		foreach (Transition t in TransitionsFrom) {
			if (t.isActiveAndEnabled)
				t.OnSight (o);
		}
	}

    public void OutOfSight(Observable o)
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OutOfSight(o);
        }
    }

    public void OnHit(HitInfo hb) { 
		foreach (Transition t in TransitionsFrom) {
			if (t.isActiveAndEnabled)
				t.OnHit (hb);
		}
	}

    public void OnItemGet(Item i)
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnItemGet(i);
        }
    }
    public void OnItemLost(Item i)
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnItemLost(i);
        }
    }
    public void OnStart()
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnStart();
        }
    }

    public void OnTime()
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnTime();
        }
    }
    public void OnEnterZone(Zone z)
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnEnterZone(z);
        }
    }
    public void OnExitZone(Zone z)
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnExitZone(z);
        }
    }

    public virtual void TriggerEvent(AIEvent aie)
    {
        foreach (Transition t in TransitionsFrom)
        {
            if (t.isActiveAndEnabled)
                t.OnTriggerEvent(aie);
        }
    }
    public void RequestTransition(Task t) {
	}

	public void RequestTransition(TaskType tt) {
	}

	public void SetActive(bool act) {
		m_active = act;
	}

	public virtual void OnTransition() {}

	public virtual void OnActiveUpdate() {
	}

    public virtual void OnLoad(Goal g) {}

    public virtual void OnSave(Goal g) {}

    public void SetTargetObj(GameObject go)
    {
        if (go == null)
            return;
        ParentGoal.SetVariable("Target", go.name,this);
    }
    public GameObject GetTargetObj()
    {
        if (ParentGoal == null || !ParentGoal.ContainsKey("Target", this))
            return null;
        return GameObject.Find(ParentGoal.GetVariable("Target", this));
    }

    public virtual string debugExtraInfo()
    {
        return "";
    }
}
