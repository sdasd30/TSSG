using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    // Start is called before the first frame update
    private Dictionary<Attackable, HitInfo> m_hitTargets;
    public Dictionary<Attackable, HitInfo> AttackHistory { get { return m_hitTargets; } private set { m_hitTargets = value; } }
    void Start()
    {
        m_hitTargets = new Dictionary<Attackable, HitInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void RegisterHit(GameObject otherObj, HitInfo hi, HitResult hr)
    {
        //Debug.Log ("Collision: " + this + " " + otherObj);
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnHitConfirm(hi, otherObj, hr));
        //Debug.Log ("Registering hit with: " + otherObj);
        if (otherObj.GetComponent<Attackable>() != null)
        {
            m_hitTargets[otherObj.GetComponent<Attackable>()] = hi;
        }
        //if (m_currentAction != null)
        //    m_currentAction.OnHitConfirm(otherObj, hi, hr);
    }
}
