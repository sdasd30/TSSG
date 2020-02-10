using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitScanInfo
{

    public Vector3 CreatePos = new Vector3(1.0f, 0f);
    public bool AimTowardsTarget = false;
    public bool FollowCharacter = true;
    public Vector3 AimDirection = new Vector3(1.0f, 0f);
    public float MaxRange = 10.0f;
    public int PenetrativePower = 1;
    public bool CanPenetrateWall = false;
    public float Damage = 10.0f;
    public float Stun = 0.3f;
    public float HitboxDuration = 0.5f;
    public Vector3 Knockback = new Vector3(10.0f, 10.0f);
    public ElementType Element = ElementType.PHYSICAL;
}


public class ActionHitScan : ActionInfo
{
    [SerializeField]
    private List<HitScanInfo> m_HitScan;
    // Start is called before the first frame update
    protected override void OnAttack()
    {
        base.OnAttack();
        if (m_HitScan.Count != 0)
        {
            createHitScan();
        }
    }

    protected void createHitScan()
    {
        foreach (HitScanInfo pi in m_HitScan)
        {
            m_hitboxMaker.createLineHB(pi);
        }
    }
}
