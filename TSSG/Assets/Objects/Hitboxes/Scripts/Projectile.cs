using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    public float ProjectileSpeed;
    public Vector2 AimPoint = new Vector2();
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
            orientToSpeed(new Vector2(m_velocity.x, m_velocity.y));
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
    protected override void OnHitObject(Collider2D other)
    {
        if (TravelThroughWalls)
            return;
        if (other.gameObject != Creator && !other.isTrigger && !JumpThruTag(other.gameObject)
            && other.GetComponent<Attackable>() == null)
        {
            m_remainingDuration = 0f;
        }
    }
    void orientToSpeed(Vector2 speed)
    {
        if (ProjectileSpeed != 0f)
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(speed.y, speed.x)));
    }

    private bool JumpThruTag(GameObject obj)
    {
        return (obj.CompareTag("JumpThru") || (obj.transform.parent != null &&
            obj.transform.parent.CompareTag("JumpThru")));
    }

    public override void SetHitboxActive(bool a)
    {
        base.SetHitboxActive(a);
        m_numPenetrated = 0;
    }

    public void SetAimPoint(Vector2 ap)
    {
        m_velocity = new Vector3(ProjectileSpeed * Time.fixedDeltaTime * ap.normalized.x,
            ProjectileSpeed * Time.fixedDeltaTime * ap.normalized.y, 0f);
        AimPoint = ap;
    }

    IEnumerator StartGravity(float seconds, float gravityScale)
    {
        GravityScale = 0f;
        yield return new WaitForSeconds(seconds);
        GravityScale = gravityScale;
    }
}