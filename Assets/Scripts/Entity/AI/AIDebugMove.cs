using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDebugMove : AIBase
{
    Camera cam;
    private NavMeshAgent m_agent;
    private float m_tolerance = 5f;
    // Start is called before the first frame update
    private void Start()
    {
        cam = FindObjectOfType<Camera>();
        m_agent = GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    public override InputPacket AITemplate()
    {
        InputPacket ip = new InputPacket();
        m_agent.updateRotation = false;
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

        if (Vector3.Distance(transform.position, m_agent.destination) > m_tolerance)
        {
            ip.movementInput = m_agent.desiredVelocity.normalized;
            //m_agent.velocity = m_charControl.velocity;
        }
        return ip;
    }
}
