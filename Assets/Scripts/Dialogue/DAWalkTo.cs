using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAWalkTo : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "MOV");
	}

	public override string PerformAction(string actionString, Textbox originTextbox) {
		List<string> chars = ExtractArgs(actionString,"MOV");
		if (chars.Count < 2)
			return "";
		GameObject character = GameObject.Find (chars[0]);
		GameObject target = GameObject.Find (chars[1]);
		if (character != null && character.GetComponent<MovementBase>()) {
			character.GetComponent<AIBaseMovement> ().SetTarget (target.transform.position);
		}
		return "";
	}
}