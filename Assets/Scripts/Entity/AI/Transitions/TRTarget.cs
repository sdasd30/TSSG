﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRTarget : Transition {

	// Use this for initialization
	void Start () {}
	
	public override void OnSight(Observable o) {
		if (o.GetComponent<Attackable> () &&
		    MasterAI.GetComponent<FactionHolder> ().CanAttack (
			    o.GetComponent<Attackable> ().Faction)) {
			TargetTask.SetTargetObj( o.gameObject );
			//Debug.Log ("Triggering Transition");
			TriggerTransition ();
		}
	}
}