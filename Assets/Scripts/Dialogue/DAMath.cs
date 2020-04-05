using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAMath : DialogueAction
{
	public string condition;
	public override bool IsExecutionString(string actionString)
	{
		if (MatchStart(actionString, "+"))
		{
			condition = "+";
			return true;
		}
		else if (MatchStart(actionString, "-"))
		{
			condition = "-";
			return true;
		}
		else if (MatchStart(actionString, "/"))
		{
			condition = "/";
			return true;
		}
		else if (MatchStart(actionString, "*"))
		{
			condition = "*";
			return true;
		}
		else if (MatchStart(actionString, "%"))
		{
			condition = "%";
			return true;
		}
		return false;
	}

	public override string PerformAction(string actionString, Textbox originTextbox)
	{
		List<string> args = ExtractArgs(actionString, condition);
		if (args.Count != 2)
		{
			Debug.Log("INVALID ADD VARIABLE COMMAND, Need 2 Args <ADD FLOAT FLOAT> got: " + args.Count);
			return "";
		}
		float n1 = 0f;
		float n2 = 0f;
		bool success = float.TryParse(originTextbox.ParseSection(args[0]), out n1) &&
			float.TryParse(originTextbox.ParseSection(args[1]), out n2);
		if (!success)
			return "ERROR";
		if (condition == "+")
		{
			return (add(n1, n2)).ToString();
		}
		else if (condition == "-")
		{
			return (subtract(n1, n2)).ToString();
		}
		else if (condition == "*")
		{
			return (multiply(n1, n2)).ToString();
		}
		else if (condition == "/")
		{
			return (divide(n1, n2)).ToString();
		}
		else if (condition == "%")
		{
			return (modulo(n1, n2)).ToString();
		}
		return "";
	}

	private float add(float n1, float n2)
	{
		return n1 + n2;
	}

	private float subtract(float n1, float n2)
	{
		return n1 - n2;
	}

	private float multiply(float n1, float n2)
	{
		return n1 * n2;
	}

	private float divide(float n1, float n2)
	{
		return n1 / n2;
	}
	private float modulo(float n1, float n2)
	{
		return n1 % n2;
	}
}