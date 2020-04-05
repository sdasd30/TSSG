using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAIf : DialogueAction
{
	public override bool IsExecutionString(string actionString)
	{
		return MatchStart(actionString, "IF");
	}

	public override string PerformAction(string actionString, Textbox originTextbox)
	{
		List<string> args = ExtractArgs(actionString, "IF");

		if (args.Count < 2 || args.Count > 3)
		{
			Debug.Log("INVALID SET VARIABLE COMMAND, Need 2-3 Args <IF CONDITION THENACTION OPTIONALELSE> got: " + args.Count + "arguments.");
			return "";
		}
		string cond = originTextbox.ParseSection(args[0]);
		if (cond == "T")
			return originTextbox.ParseSection(args[1]);
		else if (args.Count == 3)
			return originTextbox.ParseSection(args[2]);
		else
			return "";
	}
}