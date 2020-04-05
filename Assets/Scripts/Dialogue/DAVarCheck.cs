using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAVarCheck : DialogueAction {

	public override bool IsExecutionString(string actionString) {
		return MatchStart (actionString, "VARCHK");
	}

	public override string PerformAction(string actionString, Textbox originTextbox) {
		List<string> args = ExtractArgs(actionString, "VARCHK");

		if (args.Count != 3) {
			Debug.Log ("INVALID SET VARIABLE COMMAND, Need 3 or 4 Args <IF VARIABLE EQUALTOVAL THENACTION ELSE> got: " + args.Count + "arguments.");
			return "";
		}
		string s = SaveObjManager.PublicVars ().GetString (args [0]);
		if (s.Length > 0 || s != args[1])
			return originTextbox.ParseSection(args[2]);
		else if (args.Count == 4)
			return originTextbox.ParseSection(args[3]);
		else
			return "";
	}
}
