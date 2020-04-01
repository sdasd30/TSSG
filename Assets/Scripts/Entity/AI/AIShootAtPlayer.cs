using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShootAtPlayer : AIInputParentClass
{
    public Transform m_targetObj;
    public float StartShootingRange;
    public float ShootProbability = 0.0f;
    public float ShootCheckInterval = 1.0f;
    public float ShootHoldBurstTime = 0.1f;
    public Vector3 TargetOffset = new Vector3();
    public Vector2 RandomRangeX = new Vector2();
    public Vector2 RandomRangeZ = new Vector2();
    private float m_nextShootTime = 0.0f;
    private float m_nextRecheck = 0.0f;
    private float m_fireHoldTime;

    private const float TARGET_RESEEK_TIME = 0.25f;
    void Start()
    {
        //BasicMovement[] ListMovements = FindObjectsOfType<BasicMovement>();
        //foreach (BasicMovement bm in ListMovements)
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
            m_targetObj = GetComponent<FactionHolder>().GetNearestEnemy();
            m_nextRecheck = Time.timeSinceLevelLoad + TARGET_RESEEK_TIME;
        }
            
    }
    public override InputPacket AITemplate()
    {
        InputPacket ip = new InputPacket();
        float t = Time.timeSinceLevelLoad;
        if (m_targetObj != null)
        {
            float d = Vector3.Distance(m_targetObj.transform.position, transform.position);
            if (d < StartShootingRange)
            {
                Vector3 target =  m_targetObj.transform.position + TargetOffset;
                target += new Vector3(Random.Range(RandomRangeX.x, RandomRangeX.y),0f, Random.Range(RandomRangeZ.x, RandomRangeZ.y));
                target = target - transform.position;
                ip.MousePointWorld = target;
                if (t < m_fireHoldTime)
                    ip.leftMouse = true;
                if (ShootProbability > 0.0f && t > m_nextShootTime)
                {
                    if (Random.Range(0, 100) < ShootProbability)
                    {
                        m_nextShootTime = t + ShootCheckInterval;
                        ip.leftMousePress = true;
                        m_fireHoldTime = t + ShootHoldBurstTime;
                    }
                }
            }
        }
        return ip;
    }
}
