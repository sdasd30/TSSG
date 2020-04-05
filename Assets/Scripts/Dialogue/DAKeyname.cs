using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAKeyname : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "&");
	}

	public override string PerformAction(string actionString, Textbox originTextbox) {
		return TextboxManager.GetKeyString (ExtractArgs(actionString,"&")[0]);
	}
}