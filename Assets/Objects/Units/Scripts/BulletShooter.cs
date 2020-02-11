using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MovementTemplate
{

    /*public WeaponStats Weapon;
    public Vector2 Offset;
    AttackSound AudioManager;
    bool firing;
    float coolDown = 0;

    void Start()
    {
        Weapon = GetComponent<WeaponStats>();
        if (GetComponent<AttackSound>() != null)
            AudioManager = GetComponent<AttackSound>();
    }

    public override void HandleInput(InputPacket ip)
    {
        if (Weapon == null)
            return;
        if (Weapon.auto)
        {
            if (ip.fire1)
            {
                if (coolDown <= 0)
                {
                    fire(Weapon, ip.MousePointWorld);
                }
            }
        }
        else
        {
            if (ip.fire1Press)
            {
                if (coolDown <= 0)
                {
                    fire(Weapon, ip.MousePointWorld);
                }
            }
        }
        if (coolDown >= 0)
        {
            coolDown -= 1 * Time.deltaTime;
        }
    }

    public void fire(WeaponStats wp, Vector2 targetPoint)
    {
        GetComponent<Attackable>().TakeDamage(wp.RecoilDamage);
        for (int i = 0; i < Weapon.shots; i++)
        {
            //float angle = Mathf.Atan2(targetPoint.y - transform.position.y, targetPoint.x - transform.position.x) * Mathf.Rad2Deg - 90f;
            //float angle = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
            //GameObject bullet = GameObject.Instantiate(Weapon.bullet, transform.position + new Vector3(Offset.x * Mathf.Cos(angle), Offset.y * Mathf.Sin(angle), +.5f), Quaternion.identity);

            //Debug.Log("from: " + transform.position + " to: " + targetPoint + " ang: " + angle);
            ////bullet.GetComponent<Projectile>().SetAngle(angle + Random.Range(-Weapon.spread, Weapon.spread));
            //bullet.GetComponent<Projectile>().
            //bullet.GetComponent<Projectile>().SetWeapon(Weapon);
            //GetComponent<FactionHolder>().SetFaction(bullet);
            //Destroy(bullet, Weapon.duration);

            Vector3 rawTargetPoint = targetPoint;
            rawTargetPoint = rawTargetPoint + new Vector3(Offset.x,Offset.y,0f);
            float angle = Mathf.Atan2(rawTargetPoint.y, rawTargetPoint.x);
            float spread = Weapon.spread * Mathf.Deg2Rad;
            float rand = Random.Range(-spread, spread);
            
            angle += rand;
            targetPoint = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            //Debug.Log(rand + " : " + Mathf.Cos(angle) + " : " + Mathf.Sin(angle));
            CreateProjectile(wp.bullet, Vector2.zero, targetPoint, wp);
        }
        if (Weapon.attackSound != null && Weapon.attackSound.Count > 0 && AudioManager != null)
            AudioManager.playSound(Weapon.attackSound[Random.Range(0, Weapon.attackSound.Count)]);
        coolDown = Weapon.firerate / 1000;
        
    }

    public Projectile CreateProjectile(GameObject prefab, Vector2 creationPoint, Vector2 targetPoint,
        WeaponStats wps)
    {
        float projectileSpeed = wps.speed;
        float damage = wps.damage;
        float stun = 0f;
        float projectileDuration = wps.duration;
        Vector2 knockback = wps.knockbackMult * Vector2.right;
        bool fixedKnockback = false;
        ElementType element = ElementType.PHYSICAL;
        Vector3 newPos; Quaternion newRot;
        if (GetComponent<BasicMovement>().IsCurrentPlayer)
        {
            newPos = transform.GetChild(0).GetChild(0).position + wps.Offset;
            newRot = transform.GetChild(0).transform.rotation;
        }
        else
        {
            Vector2 cOff = (GetComponent<Orientation>() == null) ? creationPoint : GetComponent<Orientation>().OrientVectorToDirection2D(creationPoint);
            newPos = transform.position + (Vector3)cOff;
            newRot = Quaternion.identity;
        }

        GameObject go;
        if (prefab == null)
            return null;
        go = GameObject.Instantiate(prefab, newPos, newRot);
        Projectile newProjectile = go.GetComponent<Projectile>();

        newProjectile.Damage = damage;
        newProjectile.Duration = projectileDuration;
        newProjectile.OriginWeapon = Weapon;
        newProjectile.PenetrativePower = wps.pierce;
        if (fixedKnockback)
            newProjectile.Knockback = (GetComponent<Orientation>() == null) ? knockback : GetComponent<Orientation>().OrientVectorToDirection2D(knockback);
        else
        {
            float angle = Mathf.Atan2(targetPoint.y, targetPoint.x);
            newProjectile.Knockback = new Vector2(Mathf.Cos(angle)* knockback.x,Mathf.Sin(angle) * knockback.x);
            
        }
            
        newProjectile.IsFixedKnockback = fixedKnockback;
        newProjectile.Stun = stun;
        newProjectile.AddElement(element);
        newProjectile.Creator = gameObject;
        GetComponent<FactionHolder>().SetFaction(go);
        newProjectile.ProjectileSpeed = projectileSpeed;
        newProjectile.SetAimPoint(targetPoint); // (m_physics == null) ? targetPoint : m_physics.OrientVectorToDirection(targetPoint);
        newProjectile.Init();
        newProjectile.GravityScale = wps.GravityScale;
        newProjectile.timeToGravity = wps.timeToGravity;
        newProjectile.Deflector = wps.deflector;
        OnDeathDrop odi = go.AddComponent<OnDeathDrop>();
        odi.DeathItems = wps.OnDeathCreate;
        return newProjectile;
    }


    public void ResetCooldown()
    {
        coolDown = 0f;
    }*/
}
