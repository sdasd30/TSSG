using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void AITaskCallback(List<System.Object> arguments);
public delegate void AIEventCallback(AIEvent newEvent);

public class AITaskManager : MonoBehaviour {

    public List<Goal> GoalList;

    public Goal m_currentGoal;
    public Task m_currentTask;


    public string debugLastEvent = "";
    public string debugLastTransition = "initial";

    private Dictionary<TaskType, List<Task>> MyTasks;
	private Dictionary<TaskType, List<Transition>> GenericTransitions;

    private List<string> m_GoalObjectNames;

    private float m_currentPriority;
    private string m_currentBehaviourName;
    private Dictionary<System.Type, List<AIEventCallback>> m_events;

    public void ProposeNewBehaviour(AIBehaviour b)
    {
        if (b.BehaviourPrefab == null)
            return;
        if (m_currentGoal == null)
        {
            SetBehaviour(b.BehaviourPrefab, b.ParentGoal, b.PriorityScore);
            return;
        }
        if (b.PriorityScore * b.ParentGoal.GoalPriority >
            m_currentPriority * m_currentGoal.GoalPriority)
        {
            SetBehaviour(b.BehaviourPrefab, b.ParentGoal, b.PriorityScore);
        }
    }
    public void AddNewBehaviour(AIBehaviour b)
    {
        if (b.BehaviourPrefab == null)
            return;
        if (m_currentGoal == null)
        {
            SetBehaviour(b.BehaviourPrefab, b.ParentGoal, b.PriorityScore);
            return;
        }
        SetBehaviour(b.BehaviourPrefab, b.ParentGoal, b.PriorityScore);
    }
    public void SetBehaviour(GameObject g, Goal originGoal, float priorityScore)
    {
        AddBehaviour(g, originGoal, priorityScore);
        m_currentPriority = priorityScore;
        m_currentBehaviourName = g.name;
        m_currentGoal = originGoal;
    }


    // Use this for initialization
    void Awake () {
        m_events = new Dictionary<System.Type, List<AIEventCallback>>();
        reloadGoals();
        GetComponent<PersistentItem>()?.InitializeSaveLoadFuncs(storeData, loadData);
        Goal[] gList = GetComponentsInChildren<Goal>();
        foreach (Goal g in gList)
            GoalList.Add(g);
        gList = GetComponents<Goal>();
        foreach (Goal g in gList)
            GoalList.Add(g);

        GenericTransitions = new Dictionary<TaskType, List<Transition>> ();
		reloadTasks ();
	}

    void Start()
    {
        OnStart();
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

    public void triggerEvent(AIEvent newAIEvent)
    {
        
        foreach (Goal g in GoalList)
        {
            g.triggerEvent(newAIEvent);
        }
        if (newAIEvent.ToBroadCastSawEvent && !newAIEvent.IsObservationEvent)
        {
            newAIEvent.IsObservationEvent = true;
            GetComponent<Observable>()?.BroadcastToObserver(newAIEvent);
        }
        if (!m_events.ContainsKey(newAIEvent.GetType()))
            return;
        foreach (AIEventCallback f in m_events[newAIEvent.GetType()])
        {
            f(newAIEvent);
        }
    }
    public void registerEvent(System.Type eventType, AIEventCallback callbackFunction)
    {
        if (!m_events.ContainsKey(eventType))
            m_events[eventType] = new List<AIEventCallback>();
        m_events[eventType].Add(callbackFunction);
    }
    public void deregisterEvent(System.Type eventType, AIEventCallback callbackFunction)
    {
        if (!m_events.ContainsKey(eventType))
            return;
        List<AIEventCallback> m_newList = new List<AIEventCallback>();
        foreach (AIEventCallback f in m_events[eventType])
        {
            if (f != callbackFunction)
            {
                m_newList.Add(f);
            }
        }
        m_events[eventType] = m_newList;
    }

    public List<Task> AddBehaviour(GameObject g, Goal originGoal, float priority = 1.0f)
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
			t.TaskPriority = priority;
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

    public void OnItemLost(Item i)
    {
        debugLastEvent = "item lost: " + i.name;
        if (m_currentTask != null)
        {
            m_currentTask.OnItemLost(i);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnItemLost(i);
            }
        }
        //GetComponent<Observable>()?.broadcasttoObserver("saw_OnItemLost", gameObject, i.gameObject);
    }

    public void OnHit(HitInfo hb) {
        debugLastEvent = "hit by: " + hb.Creator.name;
        foreach (Goal g in GoalList)
        {
            g.OnHit(hb);
        }

        if (m_currentTask != null) {
			m_currentTask.OnHit (hb);
			foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType]) {
				t.OnHit (hb);
			}
		}
        List<System.Object> args = new List<System.Object>();
        args.Add((Object)gameObject);
        args.Add(hb);
        //GetComponent<Observable>()?.broadcasttoObserver("saw_OnHit", args);
    }
	public void OnSight(Observable o) {
        o.GetComponent<Observable>()?.processImpressionChange(new AIEVSeenByObserver(o),GetComponent<Observer>());

        debugLastEvent = "saw: " + o.gameObject.name;
        foreach (Goal g in GoalList)
        {
            g.OnSight(o);
        }

        if (m_currentTask != null) {
			m_currentTask.OnSight (o);
			foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType]) {
				t.OnSight (o);
			}
		}
        //GetComponent<Observable>()?.broadcasttoObserver("saw_OnSight", gameObject, o.gameObject);
    }
    public void OutOfSight(Observable o)
    {
        debugLastEvent = "lost sight: " + o.gameObject.name;
        foreach (Goal g in GoalList)
        {
            g.OutOfSight(o);
        }

        if (m_currentTask != null)
        {
            m_currentTask.OutOfSight(o);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OutOfSight(o);
            }
        }
        //GetComponent<Observable>()?.broadcasttoObserver("saw_OutOfSight", gameObject, o.gameObject);
    }

    public void OnStart()
    {
        foreach (Goal g in GoalList)
        {
            g.OnStart();
        }

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
        foreach (Goal g in GoalList)
        {
            g.OnTime();
        }

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
        foreach (Goal g in GoalList)
        {
            g.OnEnterZone(z);
        }

        if (m_currentTask != null)
        {
            m_currentTask.OnEnterZone(z);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnEnterZone(z);
            }
        }
        //GetComponent<Observable>()?.broadcasttoObserver("saw_OnEnterZone", gameObject, z.gameObject);
    }
    public void OnExitZone(Zone z)
    {
        debugLastEvent = "exit zone: " + z.gameObject.name;
        foreach (Goal g in GoalList)
        {
            g.OnExitZone(z);
        }

        if (m_currentTask != null)
        {
            m_currentTask.OnExitZone(z);
            foreach (Transition t in GenericTransitions[m_currentTask.MyTaskType])
            {
                t.OnExitZone(z);
            }
        }
        //GetComponent<Observable>()?.broadcasttoObserver("saw_OnExitZone", gameObject, z.gameObject);
    }
    public void TransitionToTask(Task t) {
		if (!shouldTransitionToTask(t))
			return;
		
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
	
	private bool shouldTransitionToTask(Task t) {
		if (GetComponent<AITaskManager>().m_currentGoal == null || t.ParentGoal == GetComponent<AITaskManager>().m_currentGoal)
			return true;
		return t.ParentGoal.GoalPriority * t.TaskPriority > GetComponent<AITaskManager>().m_currentGoal.GoalPriority * m_currentTask.TaskPriority;
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

    private void storeData(CharData d)
    {
        d.SetString("CurrentBehaviour", m_currentBehaviourName);
        if (m_currentGoal != null)
            d.SetString("CurrentGoal", m_currentGoal.gameObject.name);
        else
            d.SetString("CurrentGoal", "none");
        d.SetFloat("CurrentBehaviourPriority", m_currentPriority);
        string goalList = "";
        foreach (Goal g in GoalList)
        {
            goalList += g.ExportString();
        }
        d.SetString("GoalList", goalList);
        //Debug.Log("Saving item: " + d.PersistentStrings["GoalList"]);
    }

    private void loadData(CharData d)
    {
        //Debug.Log("Loading a new Character: last goal: " + d.PersistentStrings["CurrentGoal"]);
        string savedItems = d.GetString("GoalList");
        var arr = savedItems.Split('\n');
        foreach (string s in arr)
        {
            if (s.Length > 0)
            {
                var goalArr = s.Split('|');
                if (System.Type.GetType(goalArr[0]) != null)
                {
                    GameObject newGoal = Instantiate(ListAIObjects.Instance.GenericGoal, transform);
                    System.Type goalType = System.Type.GetType(goalArr[0]);
                    newGoal.AddComponent(goalType);
                    ((Goal)newGoal.GetComponent(goalType)).InitializeVars(goalArr);
                }
            }
        }
        if (d.GetString("CurrentGoal") != "none")
        {
            GameObject g = (GameObject)Resources.Load(d.GetString("CurrentBehaviour"));
            Transform t = transform.Find(d.GetString("CurrentGoal"));
            if (g != null && t != null)
                SetBehaviour(g, t.gameObject.GetComponent<Goal>(), d.GetFloat("CurrentBehaviourPriority"));
        }
        reloadGoals();
        OnStart();
    }

    private void reloadGoals()
    {
        Goal[] gList = GetComponentsInChildren<Goal>();
        GoalList = new List<Goal>();

        foreach (Goal g in gList)
        {
            g.SetMasterAI(this);
            GoalList.Add(g);
        }
    }

    private GameObject findBehaviour(string name)
    {
        foreach (Goal g in GoalList)
        {
            Transform t = g.gameObject.transform.Find(name);
            if (t != null)
                return t.gameObject;
        }
        return null;
    }
}
