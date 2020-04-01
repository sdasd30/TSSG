using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent (typeof (Orientation))]
public class Observer : MonoBehaviour {

	public float detectionRange = 15.0f;
    public float detectionAngle = 40.0f;

	public List<Observable> VisibleObjs = new List<Observable>();
	float nextScan;
	float scanInterval = 0.5f;
	float postLineVisibleTime = 3.0f;
    Orientation m_orient;

	Dictionary<Observable,float> m_lastTimeSeen;

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
        if (Time.timeSinceLevelLoad > nextScan) {
            
            scanForEnemies ();
		}
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
				if (cDist - 0.3f < minDist) {
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
	    GetComponent<AICharacter> ()?.OnSight (o);
		o.addObserver (this);
		VisibleObjs.Add (o);
	}
    internal void outOfSight(Observable o)
    {
        GetComponent<AICharacter>()?.outOfSight(o);
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
}