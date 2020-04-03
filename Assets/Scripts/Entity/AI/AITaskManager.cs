using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITaskManager : MonoBehaviour {

	public Task m_currentTask;

	Dictionary<TaskType, List<Task>> MyTasks;

	Dictionary<TaskType, List<Transition>> GenericTransitions;


    public string debugLastEvent = "";
    public string debugLastTransition = "initial";
    // Use this for initialization
    void Awake () {
		GenericTransitions = new Dictionary<TaskType, List<Transition>> ();
		reloadTasks ();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_currentTask != null)
        {
            m_currentTask.OnUpdate();

            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnUpdate();
            }
        }
    }

	public float get2DDistanceToPoint(Vector3 tgt)
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                new Vector2(tgt.x, tgt.z));
    }

    public void MoveToPoint(Vector3 target, float tolerance = 0.5f, float speedProportion = 1.0f)
    {
        GetComponent<AIBaseMovement>().SetTarget(target, tolerance, speedProportion);
    }
    public void FacePoint(Vector3 tgt)
    {
        GetComponent<Orientation>().FacePoint(tgt);
    }
    public float getDistanceToPoint(Vector3 tgt)
    {
        return Vector3.Distance(transform.position, tgt);
    }

    public List<Task> AddBehaviour(GameObject g, Goal originGoal)
    {
        GameObject newG = Instantiate(g);
        
        Task[] tList = newG.GetComponentsInChildren<Task>();
        Transition[] trList = newG.GetComponentsInChildren<Transition>();
        List<Task> addedTasks = new List<Task>();
        foreach (Task t in tList)
        {
            t.ParentGoal = originGoal;
            t.transform.parent = transform;
            t.ParentBehaviour = newG.name;
            t.OnLoad(originGoal);
            addedTasks.Add(t);
        }

        foreach (Transition t in trList)
        {
            t.ParentGoal = originGoal;
            t.transform.parent = transform;
            t.OnLoad(originGoal);
        }

        reloadTasks();
        Destroy(newG);
        return addedTasks;
    }

    public void RemoveBehaviour(GameObject g, Goal originGoal)
    {
        Task[] tList = GetComponentsInChildren<Task>();

        foreach (Task t in tList)
        {
            if (t.ParentBehaviour == g.name)
                Destroy(t.gameObject);
        }
    }
    public void OnItemGet(Item i)
    {
        debugLastEvent = "item received: " + i.name;
        if (m_currentTask != null)
        {
            m_currentTask.OnItemGet(i);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnItemGet(i);
            }
        }
    }
    public void OnItemLost(InventoryItemData i)
    {
        debugLastEvent = "item lost: " + i.itemName;
        if (m_currentTask != null)
        {
            m_currentTask.OnItemLost(i);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnItemLost(i);
            }
        }
    }

    public void OnHit(HitInfo hb) {
        debugLastEvent = "hit by: " + hb.Creator.name;
		if (m_currentTask != null) {
			m_currentTask.OnHit (hb);
			foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType]) {
				t.OnHit (hb);
			}
		}
	}
	public void OnSight(Observable o) {
        debugLastEvent = "saw: " + o.gameObject.name;
        if (m_currentTask != null) {
			m_currentTask.OnSight (o);
			foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType]) {
				t.OnSight (o);
			}
		}
	}
    public void OutOfSight(Observable o)
    {
        debugLastEvent = "lost sight: " + o.gameObject.name;
        if (m_currentTask != null)
        {
            m_currentTask.OutOfSight(o);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OutOfSight(o);
            }
        }
    }

    public void OnStart()
    {
        if (m_currentTask != null)
        {
            m_currentTask.OnStart();
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnStart();
            }
        }
    }
    public void OnTime()
    {
        debugLastEvent = "on time: ";
        if (m_currentTask != null)
        {
            m_currentTask.OnTime();
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnTime();
            }
        }
    }
    public void OnEnterZone(Zone z)
    {
        debugLastEvent = "enter zone: " + z.gameObject.name;
        if (m_currentTask != null)
        {
            m_currentTask.OnEnterZone(z);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnEnterZone(z);
            }
        }
    }
    public void OnExitZone(Zone z)
    {
        debugLastEvent = "exit zone: " + z.gameObject.name;
        if (m_currentTask != null)
        {
            m_currentTask.OnExitZone(z);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnExitZone(z);
            }
        }
    }
    public void TransitionToTask(Task t) {
        debugLastTransition = debugLastEvent;
        if (m_currentTask != null)
			m_currentTask.SetActive (false);
		m_currentTask = t;
		m_currentTask.SetActive (true);
		m_currentTask.OnTransition ();
		if (!GenericTransitions.ContainsKey (m_currentTask.MyTaskType))
			GenericTransitions [m_currentTask.MyTaskType] = new List<Transition> ();
	}

	public void TransitionToTask(TaskType tt) {
		if (MyTasks.ContainsKey(tt))
			TransitionToTask (MyTasks [tt] [0]);
	}

	private void AddTask(Task t) {
		t.Init ();
		if (!MyTasks.ContainsKey(t.MyTaskType))
			MyTasks[t.MyTaskType] = new List<Task>();
		
		if (!MyTasks [t.MyTaskType].Contains (t)) {
			MyTasks [t.MyTaskType].Add (t);
			addTransitions (t.TransitionsTo);
			foreach(Transition tt in t.TransitionsFrom) {
				tt.MasterAI = this;
			}
		}
	}

	private void RemoveTask(Task t) {
		if (!MyTasks.ContainsKey(t.MyTaskType))
			MyTasks[t.MyTaskType] = new List<Task>();
		if (MyTasks [t.MyTaskType].Contains (t))
			MyTasks [t.MyTaskType].Remove (t);
		removeTransitions (t.TransitionsTo);
	}

    private void addTransitions(List<Transition> lt) {
		foreach (Transition t in lt) {
			t.MasterAI = this;
            if (t.TypeOfTransition == TransitionType.TO_THIS_TASK && !GenericTransitions.ContainsKey(t.OtherTaskType))
                GenericTransitions[t.OtherTaskType] = new List<Transition>();
			if (!GenericTransitions [t.OtherTaskType].Contains (t))
				GenericTransitions [t.OtherTaskType].Add (t);
		}
	}

    private void removeTransitions (List<Transition> lt) {
		foreach (Transition t in lt) {
            if (t.TypeOfTransition == TransitionType.TO_THIS_TASK && !GenericTransitions.ContainsKey(t.OtherTaskType))
                GenericTransitions[t.OtherTaskType] = new List<Transition>();
            if (GenericTransitions [t.OtherTaskType].Contains (t))
				GenericTransitions [t.OtherTaskType].Remove (t);
		}
	}
    private void reloadTasks()
    {
        Task[] tList = GetComponentsInChildren<Task>();
        MyTasks = new Dictionary<TaskType, List<Task>>();
        foreach (Task t in tList)
        {
            t.MasterAI = this;
            if (m_currentTask == null || t.IsInitialTask)
                TransitionToTask(t);
            AddTask(t);
            t.OnSave(t.ParentGoal);
        }
        Transition[] trList = GetComponentsInChildren<Transition>();
        foreach (Transition t in trList)
        {
            t.MasterAI = this;
            t.OnSave(t.ParentGoal);
        }
    }
}
