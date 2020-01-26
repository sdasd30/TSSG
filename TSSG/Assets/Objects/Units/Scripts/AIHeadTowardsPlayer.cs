using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHeadTowardsPlayer : AIBase
{
    public float ChaseDistance = 10f;
    public float MaxYDistanceChase = -1f;
    public float AbandonDistance = 15f;
    public float DelayAfterAttack = 1.0f;
    public float JumpProbabilityPercent = 0.0f;
    public float JumpCheckInterval = 1.0f;
    public float JumpWhenInProximityOf = 999f;
    public bool YAccessMove = false;
    public Vector2 TargetOffset = new Vector2();
    public float TargetTolerance = 0.2f;

    public Transform m_targetObj;
    private BasicMovement m_playerChar;
    private float m_nextMoveOKTime = 0.0f;
    private float m_nextJumpTime = 0.0f;
    private float m_jumpingHoldTime = 0.0f;

    private float m_nextRecheck = 0.0f;

    private const float TARGET_RESEEK_TIME = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        //BasicMovement[] ListMovements = FindObjectsOfType<BasicMovement>();
        //foreach ( BasicMovement bm in ListMovements)
        //{
        //    if (bm.IsCurrentPlayer)
        //    {
        //        m_targetObj = bm.transform;
        //    }
        //}
        m_nextRecheck = Time.timeSinceLevelLoad + Random.Range(0f, TARGET_RESEEK_TIME);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_targetObj == null || Time.timeSinceLevelLoad > m_nextRecheck)
        {
            m_targetObj = GetComponent<FactionHolder>().GetNearestEnemy(MaxYDistanceChase);
            m_nextRecheck = Time.timeSinceLevelLoad + TARGET_RESEEK_TIME;
        }
    }
    public override InputPacket AITemplate()
    {
        InputPacket ip = new InputPacket();
        float t = Time.timeSinceLevelLoad;
        if (m_targetObj != null && t > m_nextMoveOKTime)
        {
            float moddedOffX = (m_targetObj.transform.position.x > transform.position.x) ? TargetOffset.x : -TargetOffset.x ;
            Vector2 target = new Vector2(m_targetObj.transform.position.x + moddedOffX, m_targetObj.transform.position.y + TargetOffset.y);
            float d = Vector2.Distance(target, transform.position);
            if (d > TargetTolerance)
            {
                ip.movementInput = new Vector2((target.x > transform.position.x) ? 1f : -1f,
                (target.y > transform.position.y) ? 1f : -1f);
                if (!YAccessMove)
                    ip.movementInput.y = 0f;
            }
            if (t < m_jumpingHoldTime)
                ip.jump = true;
                
            if (JumpProbabilityPercent > 0.0f && t > m_nextJumpTime && d < JumpWhenInProximityOf)
            {
                m_nextJumpTime = t + JumpCheckInterval;
                if (Random.Range(0, 100) < JumpProbabilityPercent)
                {
                    m_jumpingHoldTime = t + 0.5f;
                    //Debug.Log("Trying to Jump");
                }
                    
            }
        }
        
        return ip;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Attackable>() != null && GetComponent<FactionHolder>().CanAttack(other.gameObject.GetComponent<Attackable>()))
            m_nextMoveOKTime = Time.timeSinceLevelLoad + DelayAfterAttack;
    }


    //void oldMovement()
    //{
    //    if (targetSet)
    //    {
    //        if (targetObj)
    //        {
    //            if (followObj == null)
    //            {
    //                endTarget();
    //                return;
    //            }
    //            targetPoint = followObj.transform.position;
    //        }
    //        moveToPoint(targetPoint);
    //    }
    //}


    //internal void baseMovement(InputPacket ip)
    //{
    //    if (m_physics.onGround) { canDoubleJump = true; }
    //    inputX = 0.0f;
    //    inputY = 0.0f;
    //    if (!autonomy && m_physics.canMove && targetSet)
    //    {
    //        if (targetObj)
    //        {
    //            if (followObj == null)
    //            {
    //                endTarget();
    //                return;
    //            }
    //            targetPoint = followObj.transform.position;
    //        }
    //        moveToPoint(targetPoint);
    //    }
    //    else if (m_physics.canMove && autonomy)
    //    {
    //        inputY = ip.movementInput.y;
    //        inputX = ip.movementInput.x;
    //        if (ip.jump)
    //        {
    //            if (inputY < -0.9f)
    //            {
    //                GetComponent<PhysicsSS>().setDropTime(1.0f);
    //            }
    //            else if (m_physics.collisions.below)
    //            {
    //                m_physics.addSelfForce(m_jumpVector, 0f);
    //                jumpPersist = 0.2f;
    //            }
    //            else if (canDoubleJump)
    //            {
    //                velocity.y = m_jumpVelocity;
    //                m_physics.addSelfForce(m_jumpVector, 0f);
    //                canDoubleJump = false;
    //            }
    //        }
    //    }
    //    //m_physics logic
    //    float targetVelocityX = inputX * MoveSpeed;
    //    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref m_velocityXSmoothing, (m_physics.collisions.below) ? m_accelerationTimeGrounded : m_accelerationTimeAirborne);
    //    Vector2 input = new Vector2(inputX, inputY);
    //    m_physics.Move(velocity, input);
    //    m_physics.AttemptingMovement = (inputX != 0.0f);
    //}

}
