using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAction  {

	public virtual bool IsExecutionString(string actionString) {return false;}

	public virtual string PerformAction(string remainder, Textbox originTextbox) {
		return "";
	}

	public virtual string SkipAction(string remainder, Textbox originTextbox)
	{
		return PerformAction(remainder, originTextbox);
	}
	protected bool MatchStart(string actionString, string key) {
		if (key.Length > actionString.Length)
			return false;
		for (int i = 0; i < key.Length; i++) {
			if (actionString.ToCharArray () [i] != key.ToCharArray () [i])
				return false;
		}
		return true;
	}

	protected List<string> ExtractArgs(string actionString, string key) {
		List<string> rawArgs = new List<string>();
		if (key.Length > actionString.Length)
			return rawArgs;
		string lastArg = "";
		int numSpecials = 0;
		for (int i = key.Length; i < actionString.Length; i++) {
			char nextChar = actionString.ToCharArray () [i];
			if (nextChar == '>') {
				numSpecials--;
				lastArg += actionString.ToCharArray()[i];
			} else if (nextChar == '<') {
				numSpecials++;
				lastArg += actionString.ToCharArray()[i];
			} else if (nextChar != ' ' || numSpecials > 0) {
				lastArg += actionString.ToCharArray () [i];
			} else if (lastArg.Length > 0) {
				rawArgs.Add (lastArg);
				lastArg = "";
			}
		}
		rawArgs.Add (lastArg);
		return rawArgs;
	}
}
