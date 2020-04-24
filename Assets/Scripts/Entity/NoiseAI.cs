using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoiseType {NONE, GUNSHOT, MUSIC, VOICE_FRIENDLY, VOICE_HOSTILE}

public class NoiseForTrigger {
    public NoiseType noiseType;
    public GameObject NoiseOrigin;
    public Transform originPosition;
    public float distanceFromSource;
    public float rangeLeft;
}
public class NoiseAI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlaySound(NoiseType type, float range)
    {
        Observer[] allObs = FindObjectsOfType<Observer>();
        foreach (Observer o in allObs)
        {
            Vector3 otherPos = o.transform.position;
            Vector3 myPos = transform.position;
            float cDist = Vector3.Distance(otherPos, myPos);
            if (o.gameObject != gameObject && cDist < range )
            {
                RaycastHit[] hits = Physics.RaycastAll(myPos, otherPos - myPos, cDist);


                float minDist = float.MaxValue;
                foreach (RaycastHit h in hits)
                {
                    GameObject oObj = h.collider.gameObject;
                    if (oObj != gameObject && oObj.GetComponent<NoiseProofing>() != null)
                    {
                        minDist = Mathf.Min(minDist, Vector3.Distance(transform.position, h.point));
                    }
                }
                float diff = Mathf.Abs(cDist - minDist);
                if (cDist - 0.3f < minDist)
                {
                    Debug.DrawRay(myPos, otherPos - myPos, Color.blue);
                    o.ListenToNoise(toNoise(type,range - cDist,cDist));
                }
            }
        }
    }

    public NoiseForTrigger toNoise(NoiseType nt, float rangeLeft, float distanceFromSource)
    {
        NoiseForTrigger newNoise = new NoiseForTrigger();
        newNoise.noiseType = nt;
        newNoise.NoiseOrigin = gameObject;
        newNoise.originPosition = transform;
        newNoise.distanceFromSource = distanceFromSource;
        newNoise.rangeLeft = rangeLeft;
        return newNoise;
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
}
