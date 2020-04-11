using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStats
{
    public GameObject projectile;
    public float spread; //In degrees.
    public float firedelay; //In seconds between shots.
    public float randomDelay;
    public float damage = 1;
    public float speed = 10; //Projectile Speed.
    public float duration = 5; //How long the bullet lasts in seconds.
    public int shots = 1; //How many shots does the gun shoot at once?
    public int pierce = 0; //How many enemies can this gun pierce;
    public float stun = 0.2f;
    public float hitScanRange = 10f;
    public float knockbackMult = 1; //Multiplier of knockback. 1 is mean does base knockback.
    public Vector3 Offset; //How much offset does it have?
    public List<DeathDropItem> OnDeathCreate;
    [HideInInspector]
    public float GravityScale = 0f;
    [HideInInspector]
    public float timeToGravity = 0f; //How many seconds of flat travel before it begins to fall to gravity?

    public float RecoilDamage = 0f; //How much damage does it do to the player per shot
    public bool auto = false; //Is full auto?
    [HideInInspector]
    public bool deflector = false;
    public List<AudioClip> attackSound; //What sounds should play when a projectile is created?
    public GameObject AmmoType;
    public int AmmoConsumedPerShot = 1;
}


public class ActionGunshot : ActionInfo
{
    [SerializeField]    
    public WeaponStats m_weaponStats;

    private int AmmoLeftInClip;
    private float m_nextTimeCanFire = 0;
    internal void Start()
    {
        m_AttackAnimInfo.RecoveryTime = m_weaponStats.firedelay;
    }
    protected override void OnStartUp()
    {
        base.OnStartUp();
        if (Time.timeSinceLevelLoad < m_nextTimeCanFire)
        {
            InputPacket ip = getLastInputPacket();
            if (m_weaponStats.auto && ip.InputKeyPressed.ContainsKey(m_useButton))
            {
                ResetAndProgress();
            }
            else{
                m_charBase.SkipActionToEnd();
            }
        }
            
    }
    protected override void OnAttack()
    {
        WeaponStats wp = m_weaponStats;
        base.OnAttack();
        EqpWeapon wep = null;
        if (SourceEqp != null)
        {
            wep = (EqpWeapon)SourceEqp;
            if (wep != null && wp.AmmoConsumedPerShot > wep.CurrentAmmo)
            {
                m_nextTimeCanFire = Time.timeSinceLevelLoad + wp.firedelay;
                return;
            }
        }
        m_charBase.GetComponent<Attackable>().DamageObj(wp.RecoilDamage);
        for (int i = 0; i < wp.shots; i++)
        {
            Vector3 rawTargetPoint = new Vector3(1f,0f,0f);
            rawTargetPoint = rawTargetPoint + new Vector3(wp.Offset.x, wp.Offset.y, 0f);
            float angle = Mathf.Atan2(rawTargetPoint.y, rawTargetPoint.x);
            float spread = wp.spread * Mathf.Deg2Rad;
            float rand = Random.Range(-spread, spread);

            angle += rand;
            Vector3 targetPoint = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            //Debug.Log(rand + " : " + Mathf.Cos(angle) + " : " + Mathf.Sin(angle));
            createProjectile(wp.projectile, Vector2.zero, targetPoint,Random.Range(0f,m_weaponStats.randomDelay));
        }
        //if (wp.attackSound != null && wp.attackSound.Count > 0 && AudioManager != null)
        //    AudioManager.playSound(Weapon.attackSound[Random.Range(0, Weapon.attackSound.Count)]);
        if (wp.AmmoConsumedPerShot > 0 && wep != null)
            wep.CurrentAmmo -= wp.AmmoConsumedPerShot;
        m_nextTimeCanFire = Time.timeSinceLevelLoad + wp.firedelay;
    }
    protected override void OnConclude()
    {
        base.OnConclude();
        InputPacket ip = getLastInputPacket();
        if (m_weaponStats.auto && ip.InputKeyPressed.ContainsKey(m_useButton))
            ResetAndProgress();
    }
    private void createProjectile(GameObject prefab, Vector3 origin, Vector3 targetPoint,float delay = 0.0f)
    {
        if (prefab == null)
        {
            HitScanInfo hsi = new HitScanInfo();
            hsi.CreatePos = origin;
            hsi.AimDirection = targetPoint;
            hsi.PenetrativePower = m_weaponStats.pierce;
            hsi.Damage = m_weaponStats.damage;
            hsi.Stun = m_weaponStats.stun;
            hsi.MaxRange = m_weaponStats.hitScanRange;
            hsi.HitboxDuration = m_weaponStats.duration;
            hsi.Knockback = new Vector3(10f * m_weaponStats.knockbackMult, 0f);
            m_hitboxMaker.createLineHB(hsi);
        } else
        {
            ProjectileInfo pi = new ProjectileInfo();
            pi.Projectile = prefab;
            pi.ProjectileCreatePos = origin;
            pi.ProjectileAimDirection = targetPoint;
            pi.ProjectileSpeed = m_weaponStats.speed;
            pi.PenetrativePower = m_weaponStats.pierce;
            pi.Damage = m_weaponStats.damage;
            pi.Stun = m_weaponStats.stun;
            pi.HitboxDuration = m_weaponStats.duration;
            pi.Knockback = new Vector3(10f * m_weaponStats.knockbackMult, 0f);
            if (delay == 0f) {
                m_hitboxMaker.CreateProjectile(pi);
            } else {
                m_hitboxMaker.QueueProjectile(pi,delay);
            }
        }
    }
}
