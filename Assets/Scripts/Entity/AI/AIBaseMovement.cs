using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBaseMovement : AIBase
{
    private NavMeshAgent m_agent;

    public Vector3 Destination;

    private Vector3 m_targetPoint;
    private GameObject m_followTarget;
    private bool m_isFollow = false;
    private float m_tolerance = 5f;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_targetPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override InputPacket AITemplate()
    {
        InputPacket ip = new InputPacket();
        m_agent.updateRotation = false;
        if (m_isFollow)
        {
            if (m_followTarget != null)
            {
                m_agent.SetDestination(m_followTarget.transform.position);
            }
            else
            {
                m_isFollow = false;
            }
        }
        m_agent.updatePosition = false;
        m_agent.updateRotation = false;

        if (Vector3.Distance(transform.position, m_targetPoint) > m_tolerance)
        {
            ip.movementInput = m_agent.desiredVelocity.normalized;
            //m_agent.velocity = m_charControl.velocity;
            Destination = m_agent.destination;
        }
        return ip;
    }

    public void SetTarget(Vector3 target, float tolerance = 0.5f)
    {
        //Debug.Log("Setting destination to : " + t);
        m_agent.SetDestination(target);
        m_tolerance = tolerance;
        m_targetPoint = target;

    }
    public void SetFollowGameObject(GameObject follow, bool keepFollowing = true)
    {
        m_agent.SetDestination(follow.transform.position);
        if (keepFollowing)
        {
            m_followTarget = follow;
            m_isFollow = true;
        }
    }
}
