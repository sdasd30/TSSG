using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBaseMovement : AIInputParentClass
{
    [HideInInspector]
    public Vector3 Destination;
    private NavMeshAgent m_agent;

    private Vector3 m_targetPoint;
    private GameObject m_followTarget;
    private bool m_isFollow = false;
    private float m_tolerance = 5f;
    private bool primaryPressed = false;
    private bool secondaryPressed = false;
    private float primaryHoldUntil = 0f;
    private float secondaryHoldUntil = 0f;
    private Vector3 UseItemInput = new Vector3();

    private List<string> slotUse = new List<string>();
    // Start is called before the first frame update
    private void Awake()
    {
        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_agent.updatePosition = false;
        m_agent.updateRotation = false;
    }
    void Start()
    {
        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_targetPoint = transform.position;
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
        Vector3 d = m_targetPoint - transform.position;
        float dist = Vector3.Magnitude(new Vector3(d.x, 0f, d.z));
        if (dist > m_tolerance)
        {
            ip.movementInput = m_agent.desiredVelocity.normalized;
            //m_agent.velocity = m_charControl.velocity;
            Destination = m_agent.destination;
        }
        float t = Time.timeSinceLevelLoad;
        if (t < primaryHoldUntil || primaryPressed)
        {
            ip.leftMouse = true;
            ip.movementInput += UseItemInput;
        }
        if (t < secondaryHoldUntil || secondaryPressed)
        {
            ip.rightMouse = true;
            ip.movementInput += UseItemInput;
        }
            
        if (primaryPressed)
        {
            ip.leftMousePress = true;
            primaryPressed = false;
        }
            
        if (secondaryPressed)
        {
            ip.rightMousePress = true;
            secondaryPressed = false;
        }
        if (slotUse.Count > 0)
        {
            ip.itemSlotUse.AddRange(slotUse);
            slotUse.Clear();
        }
        return ip;
    }

    public void SetTarget(Vector3 target, float tolerance = 0.5f,float moveSpeedMod = 1.0f)
    {
        //Debug.Log("Setting destination to : " + t);
        m_agent.SetDestination(target);
        m_tolerance = tolerance;
        m_targetPoint = target;
        if (moveSpeedMod != 1.0f)
        {
            GetComponent<MovementBase>().AddModifier("AITarget", moveSpeedMod, Time.fixedDeltaTime);
        }
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

    public void UseItem(string slot, Vector3 input, float timeHold = 0f)
    {
        UseItemInput = input;
        if (slot == "primary")
        {
            primaryPressed = true;
        } else if (slot == "secondary")
        {
            secondaryPressed = true;
        } else
        {
            slotUse.Add(slot);
        }
    }
}
