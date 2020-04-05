using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAVarGet : DialogueAction
{
	public override bool IsExecutionString(string actionString)
	{
		return MatchStart(actionString, "VARGET");
	}

	public override string PerformAction(string actionString, Textbox originTextbox)
	{
		List<string> args = ExtractArgs(actionString, "VARGET");

		if (args.Count != 1)
		{
			Debug.Log("INVALID VARGET VARIABLE COMMAND, Need 1 Args <VARGET VARIABLENAME> got: " + args.Count + "arguments.");
			return "";
		}
		string s = SaveObjManager.PublicVars().GetString(args[0]);
		return s;
	}
}
