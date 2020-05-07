using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    public float ProjectileSpeed;
    public Vector3 AimPoint = new Vector3();
    public int PenetrativePower = 0;
    public bool TravelThroughWalls = false;
    public bool OrientToSpeed = true;
    public float GravityScale = 0.0f;
    public float timeToGravity = 0f;

    int m_numPenetrated = 0;

    private const float TERMINAL_VELOCITY = -10f;
    private Vector3 m_velocity;

    private void Start()
    {
        if (timeToGravity != 0f)
        {
            StartCoroutine(StartGravity(timeToGravity,GravityScale));
            
        }
    }
    new virtual internal void Update()
    {
        Tick();
    }

    protected override void Tick()
    {
        base.Tick();
    }

    protected void FixedUpdate()
    {
        if (GravityScale != 0.0f)
            processGravity();
        transform.Translate(m_velocity, Space.World);
        if (OrientToSpeed)
            orientToSpeed(m_velocity);
    }
    protected override HitResult OnAttackable(Attackable atkObj)
    {
        if (canAttack(atkObj))
            incrementPenetration();
        return base.OnAttackable(atkObj);
    }

    void incrementPenetration()
    {
        m_numPenetrated++;
        if (m_numPenetrated > PenetrativePower)
            m_remainingDuration = 0f;
    }

    private void processGravity()
    {
        if (m_velocity.y > TERMINAL_VELOCITY)
            m_velocity.y += -GravityScale * Time.fixedDeltaTime;
    }
    protected override void OnHitObject(Collider other)
    {
        if (TravelThroughWalls)
            return;
        if (other.gameObject != Creator && !other.isTrigger
            && other.GetComponent<Attackable>() == null)
        {
            m_remainingDuration = 0f;
        }
    }
    void orientToSpeed(Vector3 speed)
    {
        if (ProjectileSpeed != 0f)
            transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, Mathf.Rad2Deg * Mathf.Atan2(speed.z, speed.x)));
    }
    public override void SetHitboxActive(bool a)
    {
        base.SetHitboxActive(a);
        m_numPenetrated = 0;
    }

    public void SetAimPoint(Vector3 ap)
    {
        m_velocity = new Vector3(ProjectileSpeed * Time.fixedDeltaTime * ap.normalized.x,
            ProjectileSpeed * Time.fixedDeltaTime * ap.normalized.y, ProjectileSpeed * Time.fixedDeltaTime * ap.normalized.z);
        AimPoint = ap;
    }

    IEnumerator StartGravity(float seconds, float gravityScale)
    {
        GravityScale = 0f;
        yield return new WaitForSeconds(seconds);
        GravityScale = gravityScale;
    }

    public void InitFromProjectileInfo(ProjectileInfo pi, GameObject parent,FactionHolder fh = null)
    {
        float deg = Mathf.Deg2Rad * parent.transform.rotation.eulerAngles.y;
        if (pi.AimTowardsTarget)
        {

        } else
        {
            
            transform.localPosition = transform.position + Orientation.OrientToVectorZ(pi.ProjectileCreatePos,deg);
        }
        AimPoint = Orientation.OrientToVectorZ(pi.ProjectileAimDirection,deg);
        
        ProjectileSpeed = pi.ProjectileSpeed;
        SetAimPoint(AimPoint);
        PenetrativePower = pi.PenetrativePower;
        Damage = pi.Damage;
        Stun = pi.Stun;
        Duration = pi.HitboxDuration;
        AddElement(pi.Element);
        Knockback = Orientation.OrientToVectorZ(pi.Knockback, deg);
        hitCallback = pi.hitCallback;
        if (fh != null)
            fh.SetFaction(gameObject);
        Creator = parent;
        Init();
    }
}