using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDebugMove : AIInputParentClass
{
    Camera cam;
    private NavMeshAgent m_agent;
    private float m_tolerance = 0.5f;
    // Start is called before the first frame update
    private void Awake()
    {
        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_agent.updatePosition = false;
        m_agent.updateRotation = false;
        m_agent.nextPosition = transform.position;
        cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    public override InputPacket AITemplate()
    {
        InputPacket ip = new InputPacket();
        m_agent.updateRotation = false;
        m_agent.nextPosition = transform.position;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                m_agent.SetDestination(hit.point);
            }
        }
        m_agent.updatePosition = false;
        m_agent.updateRotation = false;
        Vector2 target = new Vector2(m_agent.destination.x, m_agent.destination.z);
        Vector2 me = new Vector2(transform.position.x, transform.position.z);
        if (Vector2.Distance(me, target) > m_tolerance)
        {
            ip.movementInput = m_agent.desiredVelocity.normalized;
            //m_agent.velocity = m_charControl.velocity;
        } else
        {
            ip.movementInput = Vector3.zero;
        }
        return ip;
    }
}
