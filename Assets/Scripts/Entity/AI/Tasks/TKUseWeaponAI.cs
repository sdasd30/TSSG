using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKUseWeaponAI : Task
{
    public string UseItemSlot = "Item1";

    private List<GameObject> m_addedTasks = new List<GameObject>();
    public override void OnTransition()
    {
        base.OnTransition();
        Transform t = transform.Find(UseItemSlot);
        if (t != null && t.GetComponent<Item>() != null && t.GetComponent<Item>().AIBehaviour != null)
        {
            GetComponent<AITaskManager>().AddBehaviour(t.GetComponent<Item>().AIBehaviour, ParentGoal);
        }
    }

    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("ItemSlot", this))
            UseItemSlot = g.GetVariable("ItemSlot", this);
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("ItemSlot", UseItemSlot, this);
    }
}
