using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (FactionHolder))]
public class Attackable : MonoBehaviour {
	public float maxHP = 1;
	public float hp = 1;
	private bool alive = true;


    private DamageSource m_lastDamageSource;
    // Use this for initialization
    void Start () {
		hp = maxHP;
        if (GetComponent<FactionHolder>() == null)
            gameObject.AddComponent<FactionHolder>();
	}

	// Update is called once per frame
	void Update () {
        if (hp > maxHP) hp = maxHP;
        checkDead();
	}

    public void checkDead()
    {
        if (hp <= 0 && alive)
        {
            alive = false;
            //if (m_lastDamageSource != null && GetComponent<Score>() != null)
            //{
            //    FindObjectOfType<StatTracker>().TrackKill(GetComponent<Score>(), m_lastWeaponHurtBy);
            //}
            if (GetComponent<DeathSound>() != null)
            {
                GetComponent<DeathSound>().playSound();
            }
        }
        if (!alive)
        {
            Destroy(this.gameObject);
        }
    }

	public void TakeDamage(float damage){
        if (GetComponent<HurtSound>() != null && damage > 0)
            GetComponent<HurtSound>().playSound();
        hp -= damage;
        hp = Mathf.Min(maxHP, hp);
	}

    public void TakeKnockback(Vector2 vec)
    {
        GetComponent<PhysicsSS>().addToVelocity(vec);
    }

    public void TakeHit(HitInfo hi)
    {
        TakeDamage(hi.Damage);
        TakeKnockback(hi.Knockback);
        m_lastDamageSource = hi.OriginSource;
    }
}
