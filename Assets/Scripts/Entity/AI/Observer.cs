using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof (Orientation))]
public class Observer : MonoBehaviour {

    private Dictionary<string, Relationship> m_relationshipsByID = new Dictionary<string, Relationship>();

    private class QueuedObservationCheck
    {
        private AIEvent m_event;
        private List<System.Object> m_eventArgs;
        private float m_triggerTime;
        private Observer.ObservationCheck m_evalFunction;
        private Observable m_observable;
        public QueuedObservationCheck(AIEvent aiEvent, float timeDelay, Observer.ObservationCheck evalFunction, Observable obs)
        {
            m_event = aiEvent;
            m_triggerTime = Time.timeSinceLevelLoad + timeDelay;
            m_evalFunction = evalFunction;
            m_observable = obs;
        }
        public bool IsTriggered()
        {
            return Time.timeSinceLevelLoad > m_triggerTime;
        }
        public void OnTrigger(Observer observer)
        {
            if (m_evalFunction(m_observable, observer))
                observer.GetComponent<AITaskManager>()?.triggerEvent(m_event);
        }
    }

    public delegate bool ObservationCheck(Observable me, Observer obs);

    public float detectionRange = 15.0f;
    public float detectionAngle = 40.0f;

	public List<Observable> VisibleObjs = new List<Observable>();
	private float nextScan;
	private float scanInterval = 0.5f;
	private float postLineVisibleTime = 3.0f;

    private List<QueuedObservationCheck> m_currentObservationChecks = new List<QueuedObservationCheck>();
    Orientation m_orient;

	Dictionary<Observable,float> m_lastTimeSeen;

    public void ListenToNoise(NoiseForTrigger na)
    {
        if (GetComponent<AITaskManager>() != null)
        {
            AIEVSound newSound = new AIEVSound(na);
            GetComponent<AITaskManager>().triggerEvent(newSound);
        }
    }
    public List<GameObject> ScanRayToPoint(Vector3 targetPoint)
    {
        List<GameObject> newHitList = new List<GameObject>();
        Vector3 myPos = transform.position;
        float cDist = Vector3.Distance(targetPoint, myPos);
        RaycastHit[] hits = Physics.RaycastAll(myPos, targetPoint - myPos, cDist);
        foreach (RaycastHit h in hits)
        {
            GameObject oObj = h.collider.gameObject;
            if (oObj != gameObject && !h.collider.isTrigger)
            {
                newHitList.Add(oObj);
            }
        }
        return newHitList;
    }
	// Use this for initialization
	void Start () {
		m_lastTimeSeen = new Dictionary<Observable,float> ();
        m_orient = GetComponent<Orientation>();
		nextScan = UnityEngine.Random.Range (Time.timeSinceLevelLoad,Time.timeSinceLevelLoad + scanInterval);
	}

	void Update() {
        DebugDrawLoS();
        float t = Time.timeSinceLevelLoad;
        if (Time.timeSinceLevelLoad > nextScan) {
            
            scanForEnemies ();
		}
	}

    private void ProcessQueuedChecks()
    {
        List<QueuedObservationCheck> newList = new List<QueuedObservationCheck>();
        foreach (QueuedObservationCheck quc in m_currentObservationChecks)
        {
            if (quc.IsTriggered())
                quc.OnTrigger(this);
            else
                newList.Add(quc);
        }
        m_currentObservationChecks = newList;
    }
    private void DebugDrawLoS()
    {
        Vector3 myPos = transform.position;
        float leftAngle = Mathf.Deg2Rad * (-transform.eulerAngles.y - detectionAngle / 2f);
        float rightAngle = Mathf.Deg2Rad * (-transform.eulerAngles.y + detectionAngle / 2f);
        Vector3 leftPoint = new Vector3(Mathf.Cos(leftAngle) * detectionRange, myPos.y, Mathf.Sin(leftAngle) * detectionRange);
        Vector3 rightPoint = new Vector3(Mathf.Cos(rightAngle) * detectionRange, myPos.y, Mathf.Sin(rightAngle) * detectionRange);
        Debug.DrawRay(myPos, leftPoint, Color.blue);
        Debug.DrawRay(myPos, rightPoint, Color.blue);
    }

	void scanForEnemies() {
        Observable[] allObs = FindObjectsOfType<Observable> ();
		float lts = Time.realtimeSinceStartup;
		foreach (Observable o in allObs) {
			Vector3 otherPos = o.transform.position;
			Vector3 myPos = transform.position;
            float cDist = Vector3.Distance(otherPos, myPos);
            if (o.gameObject != gameObject && InConeOfVision(otherPos)) {
				RaycastHit[] hits = Physics.RaycastAll (myPos, otherPos - myPos, cDist);
				
                
				float minDist = float.MaxValue;
				foreach (RaycastHit h in hits) {
					GameObject oObj = h.collider.gameObject;
					if (oObj != gameObject && !h.collider.isTrigger) {
						minDist = Mathf.Min(minDist,Vector3.Distance (transform.position,h.point));
					}
				}
				float diff = Mathf.Abs (cDist - minDist);
				if (diff < 0.4f) {
                    Debug.DrawRay(myPos, otherPos - myPos, Color.green);
                    m_lastTimeSeen[o] = lts;
                    if (!VisibleObjs.Contains (o)) {
                        //Debug.Log("On Sight!: " + o.gameObject + " minDist: " + minDist + " dist: " + cDist);
						OnSight (o);
                    }
				}
			}
		}
		if (VisibleObjs.Count > 0) {
			for (int i= VisibleObjs.Count - 1; i >= 0; i --) {
				Observable o = VisibleObjs [i];
				if (o == null) { // c.gameObject == null) {
					VisibleObjs.RemoveAt (i);
				} else if (m_lastTimeSeen.ContainsKey(o)) {
					
					if (lts - m_lastTimeSeen[o] > postLineVisibleTime) {
                        //Debug.Log ("Cannot see, lts = " + lts + " m_lasttime: " + m_lastTimeSeen [o] + " post: " + postLineVisibleTime);
						
						outOfSight (o);
                        VisibleObjs.RemoveAt(i);
                    } else if (Mathf.Abs(lts - m_lastTimeSeen[o]) > 0.05f){
						//Out of sight thing.
					}	
				}
			}
		}
		nextScan = Time.timeSinceLevelLoad + scanInterval;
    }
    
    internal void OnSight(Observable o) {
        //ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnSight (o));
	    GetComponent<AITaskManager> ()?.OnSight (o);
		o.addObserver (this);
		VisibleObjs.Add (o);
	}
    internal void outOfSight(Observable o)
    {
        GetComponent<AITaskManager>()?.OutOfSight(o);
        o.removeObserver(this);
    }
	public bool IsVisible(Observable o) {
		return VisibleObjs.Contains (o);
	}
	void OnDestroy() {
		foreach (Observable o in VisibleObjs) {
			o.removeObserver (this);	
		}
	}

    private bool InConeOfVision(Vector3 point)
    {
        Vector3 myPos = transform.position;
        float cDist = Vector3.Distance(point, myPos);
        return cDist < detectionRange && GetComponent<Orientation>().FacingPoint(point,detectionAngle/2f);          
    }
    /*private bool SeeThroughTag( GameObject obj ) {
		return (obj.CompareTag ("JumpThru") || (obj.transform.parent != null &&
			obj.transform.parent.CompareTag ("JumpThru")));
	}*/
    public void queueObservationCallback(AIEvent aievent, float timeDelay, ObservationCheck evalFunction, Observable obs)
    {
        QueuedObservationCheck qcheck = new QueuedObservationCheck(aievent, timeDelay, evalFunction, obs);
        m_currentObservationChecks.Add(qcheck);
    }

    public bool ContainsImpression(string targetID, Noun i)
    {
        if (!m_relationshipsByID.ContainsKey(targetID))
            return false;
        return m_relationshipsByID[targetID].containsImpression(i);
    }
    public float GetImpressionModifiers(string targetID, Noun i)
    {
        if (!m_relationshipsByID.ContainsKey(targetID))
            return 0.0f;
        return m_relationshipsByID[targetID].GetImpressionModifiers(i);
    }
    public void AddModifier(string targetID, Noun n, ImpressionModifier newDM)
    {
        if (!m_relationshipsByID.ContainsKey(targetID))
            m_relationshipsByID[targetID] = new Relationship(this);
        m_relationshipsByID[targetID].AddModifier(n, newDM);
    }
    public void ClearModifier(string targetID, Noun n, ImpressionModifier newDM)
    {
        if (!m_relationshipsByID.ContainsKey(targetID))
            return;
        m_relationshipsByID[targetID].ClearModifier(n, newDM);
    }

    public bool IsA(LogObjHolder target, LogicalConcept obj)
    {
        return true;
    }
}

