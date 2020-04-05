using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DALogical : DialogueAction
{
	private string condition = "";
	public override bool IsExecutionString(string actionString)
	{
		if ( MatchStart(actionString, "EQU"))
		{
			condition = "EQU";
			return true;
		} else if (MatchStart(actionString, "GRE"))
		{
			condition = "GRE";
			return true;
		} else if (MatchStart(actionString, "LES"))
		{
			condition = "LES";
			return true;
		} else if (MatchStart(actionString, "AND"))
		{
			condition = "AND";
			return true;
		} else if (MatchStart(actionString, "OR"))
		{
			condition = "OR";
			return true;
		} else if (MatchStart(actionString, "XOR"))
		{
			condition = "XOR";
			return true;
		}
		return false;
	}

	public override string PerformAction(string actionString, Textbox originTextbox)
	{
		List<string> args = ExtractArgs(actionString, condition);
		if (args.Count < 2)
			return "F";
		if (condition == "EQU")
		{
			return (Equal(args)) ? "T" : "F";
		} else if (condition == "GRE")
		{
			return (Greater(args,originTextbox)) ? "T" : "F";
		}
		else if (condition == "LES")
		{
			return (Less(args,originTextbox)) ? "T" : "F";
		}
		else if (condition == "AND")
		{
			return (andCond(args,originTextbox)) ? "T" : "F";
		}
		else if (condition == "OR")
		{
			return (orCond(args,originTextbox)) ? "T" : "F";
		}
		else if (condition == "XOR")
		{
			return (xorCond(args,originTextbox)) ? "T" : "F";
		}
		return "F";
	}

	private bool Equal(List<string> args)
	{
		return args[0].Equals(args[0]);
	}

	private bool Greater(List<string> args,Textbox originTextbox)
	{
		float n1 = 0f;
		float n2 = 0f;
		bool success = float.TryParse(originTextbox.ParseSection(args[0]), out n1) &&
			float.TryParse(originTextbox.ParseSection(args[1]), out n2);
		if (!success)
			return false;
		return n1 > n2;
	}
	private bool Less(List<string> args, Textbox originTextbox)
	{
		float n1 = 0f;
		float n2 = 0f;
		bool success = float.TryParse(originTextbox.ParseSection(args[0]), out n1) &&
			float.TryParse(originTextbox.ParseSection(args[1]), out n2);
		if (!success)
			return false;
		return n1 < n2;
	}
	private bool andCond(List<string> args, Textbox originTextbox)
	{
		foreach(string s in args)
		{
			if (s != "T")
				return false;
		}
		return true;
	}

	private bool orCond(List<string> args, Textbox originTextbox)
	{
		foreach (string s in args)
		{
			if (s == "T")
				return true;
		}
		return false;
	}
	private bool xorCond(List<string> args, Textbox originTextbox)
	{
		if (args[0] == "T" && args[1] == "T")
			return false;
		return (args[0] == "T" || args[1] == "T");
	}
}
