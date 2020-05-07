using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observable : MonoBehaviour {

	List<Observer> observers = new List<Observer>();
	List<Noun> m_impressions = new List<Noun>();

	// Use this for initialization
	void Start () {
		//movt = GetComponent<PhysicsSS>();
	}
	
	// Update is called once per frame
	void Update () {}

	//Observers
	public void addObserver(Observer obs) {
		if (!observers.Contains (obs)) {
			observers.Add (obs);
		}
	}
	public void removeObserver(Observer obs) {
		if (observers.Contains(obs)) {
			observers.Remove (obs);
		}
	}

	public void BroadcastToObserver(AIEvent newEvent)
	{
		foreach (Observer o in observers)
		{
			processImpressionChange(newEvent,o);
			newEvent.IsObservationEvent = true;
			newEvent.ToBroadCastSawEvent = false;
			newEvent.ObservedObj = gameObject;
			o.GetComponent<AITaskManager>()?.triggerEvent(newEvent);
		}
	}

	public void AddImpression(Noun n)
	{
		if (n == null)
		{
			Debug.LogError("Attempted to add null Impression to : " + gameObject.name + " could not find Impression");
			return;
		}
		if (m_impressions.Contains(n))
			RemoveImpression(n);
		m_impressions.Add(n);
	}
	public void RemoveImpression(Noun n)
	{
		if (n == null)
		{
			Debug.LogError("Attempted to remove null Impression to : " + gameObject.name);
			return;
		}
		if (m_impressions.Contains(n))
			m_impressions.Remove(n);
	}
	private void processImpressionChange(AIEvent ev, Observer perspective)
	{
		foreach(Noun n in m_impressions)
			n?.ReactToEvent(ev, perspective);
	}
}
