﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionType { TO_THIS_TASK, FROM_THIS_TASK}

public class Transition : MonoBehaviour {

	public TransitionType TypeOfTransition;

	[HideInInspector]
	public AITaskManager MasterAI;
    [HideInInspector]
    public Goal ParentGoal;

    public TaskType OtherTaskType;
    [HideInInspector]
    public Task OriginTask;
    [HideInInspector]
    public Task TargetTask;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Init() {
	}

	public virtual void OnTriggerEvent(AIEvent aie) { }

	public virtual void OnUpdate() {}

	public virtual void OnHit(HitInfo hb) {}

	public virtual void OnSight(Observable o) {}

    public virtual void OutOfSight(Observable o) { }

    public virtual void OnExitZone(Zone z) { }

    public virtual void OnEnterZone(Zone z) { }

    public virtual void OnTime() { }

    public virtual void OnStart() { }

    public virtual void OnItemGet(Item i) { }

    public virtual void OnItemLost(Item i) { }

    public void TriggerTransition() {
		if (TypeOfTransition == TransitionType.FROM_THIS_TASK) {
			if (TargetTask != null)
				MasterAI.TransitionToTask (TargetTask);
			else
				MasterAI.TransitionToTask (OtherTaskType);
		} else {
			MasterAI.TransitionToTask (GetComponent<Task>());
		}
	}
    public virtual void OnLoad(Goal g)
    {

    }

    public virtual void OnSave(Goal g)
    {

    }

}