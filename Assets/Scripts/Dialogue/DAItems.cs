using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAItems : DialogueAction
{
	public string condition;
	public override bool IsExecutionString(string actionString)
	{
		if (MatchStart(actionString, "ITEMNUM"))
		{
			condition = "ITEMNUM";
			return true;
		}
		else if (MatchStart(actionString, "ITEMHAS"))
		{
			condition = "ITEMHAS";
			return true;
		}
		else if (MatchStart(actionString, "ITEMGIVE"))
		{
			condition = "ITEMGIVE";
			return true;
		}
		else if (MatchStart(actionString, "ITEMTAKE"))
		{
			condition = "ITEMTAKE";
			return true;
		}
		else if (MatchStart(actionString, "ITEMEQUIP"))
		{
			condition = "ITEMEQUIP";
			return true;
		}
		return false;
	}

	public override string PerformAction(string actionString, Textbox originTextbox)
	{
		List<string> args = ExtractArgs(actionString, condition);
		if (args.Count < 2)
		{
			if (condition == "ITEMEQUIP")
				Debug.Log("INVALID ITEM VARIABLE COMMAND, Need 2 or 3 Args <" + condition + " USER ITEMNAME SLOTNAME> got: " + args.Count);
			else
				Debug.Log("INVALID ITEM VARIABLE COMMAND, Need 2 or 3 Args <" + condition + " USER ITEMNAME STACKNUMBER> got: " + args.Count);
			return "";
		}
		GameObject user = GameObject.Find(originTextbox.ParseSection(args[0]));
		string itemName = originTextbox.ParseSection(args[1]);
		if (user == null || user.GetComponent<InventoryHolder>() == null || (GameObject)Resources.Load(itemName) == null)
			return "";
		GameObject item = (GameObject)Resources.Load(itemName);
		if (item.GetComponent<Item>() == null)
			return "ERROR Item not found";
		int stack = 1;
		string slot = "";
		if (condition == "ITEMEQUIP")
			slot = originTextbox.ParseSection(args[2]);
		else
			int.TryParse(originTextbox.ParseSection(args[2]), out stack);
		if (condition == "ITEMNUM")
		{
			return itemNum(user.GetComponent<InventoryHolder>(), item, stack);
		}
		else if (condition == "ITEMHAS")
		{
			return itemHas(user.GetComponent<InventoryHolder>(), item, stack);
		}
		else if (condition == "ITEMGIVE")
		{
			itemGive(user.GetComponent<InventoryHolder>(), item, stack);
		}
		else if (condition == "ITEMTAKE")
		{
			itemTake(user.GetComponent<InventoryHolder>(), item, stack);
		}
		else if (condition == "ITEMEQUIP")
		{
			itemEqp(user.GetComponent<InventoryHolder>(), item, slot);
		}
		return "";
	}

	private string itemNum(InventoryHolder user, GameObject item, int stack)
	{
		return user.GetItemCount(item.GetComponent<Item>()).ToString();
	}
	private string itemHas(InventoryHolder user, GameObject item, int stack)
	{
		int itemCount = user.GetItemCount(item.GetComponent<Item>());
		return (itemCount >= stack) ? "T" : "F";
	}

	private void itemGive(InventoryHolder user, GameObject item, int stack)
	{
		GameObject newI = GameObject.Instantiate(item);
		newI.GetComponent<Item>().CurrentStack = stack;
		user.AddItemIfFree(item.GetComponent<Item>());
	}

	private void itemTake(InventoryHolder user, GameObject item, int stack)
	{
		user.RemoveItem(item.GetComponent<Item>(), stack);
	}

	private void itemEqp(InventoryHolder user, GameObject item, string slotName)
	{
		int itemCount = user.GetItemCount(item.GetComponent<Item>());
		if (itemCount < 1)
			itemGive(user, item,1);
		user.ForceEquip(item.GetComponent<Item>(), slotName);
	}
}
