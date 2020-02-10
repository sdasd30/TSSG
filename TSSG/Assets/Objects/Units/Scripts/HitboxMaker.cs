using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HitboxMaker : MonoBehaviour
{
    private Dictionary<HitboxInfo, float> m_queuedHitboxes = new Dictionary<HitboxInfo, float>();
    private Dictionary<ProjectileInfo, float> m_queuedProjectiles = new Dictionary<ProjectileInfo, float>();

    MovementBase m_charBase;
    Orientation m_orient;

	void Awake () {
        m_charBase = GetComponent<MovementBase>();
        m_orient = GetComponent<Orientation>();
	}

	void Start() {

	}
    private void Update()
    {
        updateQueueActions();
    }
    public LineHitbox createLineHB(HitScanInfo hsi)
    {
        if (ListHitboxes.Instance == null)
        {
            Debug.LogWarning("LIstHitboxes not found. Please add a Gameobject with the ListHItboxes prefab");
            return null;
        }
        var go = GameObject.Instantiate(ListHitboxes.Instance.HitboxLine, transform.position, Quaternion.identity);
        LineHitbox lh = go.GetComponent<LineHitbox>();
        lh.InitFromLineInfo(hsi, gameObject, GetComponent<FactionHolder>());

        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnHitboxCreate(line));
        return lh;
    }

    public Hitbox CreateHitbox(HitboxInfo hbi) {
        if (ListHitboxes.Instance == null)
        {
            Debug.LogWarning("LIstHitboxes not found. Please add a Gameobject with the ListHItboxes prefab");
            return null;
        }
        var go = GameObject.Instantiate(ListHitboxes.Instance.Hitbox, transform.position, Quaternion.identity);

		Hitbox newBox = go.GetComponent<Hitbox>();
        newBox.InitFromHitboxInfo(hbi,gameObject,GetComponent<FactionHolder>());
		return newBox;
	}

    public Hitbox CreateHitbox(Vector3 hitboxScale, Vector3 offset, float damage, float stun, float hitboxDuration, Vector3 knockback, bool fixedKnockback = true,
        bool followObj = true, ElementType element = ElementType.PHYSICAL, bool applyProps = true)
    {
        Vector3 cOff = (m_charBase == null) ? offset : m_orient.OrientVectorToDirection(offset);
        Vector3 newPos = transform.position + (Vector3)cOff;
        var go = GameObject.Instantiate(ListHitboxes.Instance.Hitbox, newPos, Quaternion.identity);

        Hitbox newBox = go.GetComponent<Hitbox>();
        if (followObj) {
            go.transform.SetParent (gameObject.transform);
            newBox.transform.localScale = m_orient.OrientVectorToDirection2D(new Vector3(hitboxScale.x / transform.localScale.x, hitboxScale.y / transform.localScale.y, hitboxScale.z / transform.localScale.z), false);
        } else {
            newBox.SetScale ((m_charBase == null) ? hitboxScale : m_orient.OrientVectorToDirection2D(hitboxScale,false));
        }
        newBox.Damage = damage;

        newBox.Duration = hitboxDuration;
        newBox.Knockback = (m_charBase == null) ? knockback : m_orient.OrientVectorToDirection2D(knockback);
        newBox.IsFixedKnockback = fixedKnockback;
        newBox.Stun = stun;
        newBox.AddElement(element);
        newBox.Creator = gameObject;
        if (GetComponent<FactionHolder>() != null)
            GetComponent<FactionHolder>().SetFaction(go);
        if (followObj)
            newBox.SetFollow (gameObject,offset);
        //if (applyProps)
        //    ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newBox));
        newBox.Init();
        return newBox;
    }

    //public HitboxDoT CreateHitboxDoT(Vector3 hitboxScale, Vector3 offset, float damage, float stun, float hitboxDuration, Vector3 knockback, bool fixedKnockback = true,
    //    bool followObj = true, ElementType element = ElementType.PHYSICAL)
    //{
    //    Vector3 cOff = (m_charBase == null) ? offset : m_orient.OrientVectorToDirection(offset);
    //    Vector3 newPos = transform.position + (Vector3)cOff;
    //    var go = GameObject.Instantiate(ListHitboxes.Instance.HitboxDoT, newPos, Quaternion.identity);

    //    HitboxDoT newBox = go.GetComponent<HitboxDoT>();
    //    if (followObj) {
    //        go.transform.SetParent (gameObject.transform);
    //        Vector3 ls = new Vector3(hitboxScale.x / transform.localScale.x, hitboxScale.y / transform.localScale.y, hitboxScale.z / transform.localScale.z);
    //        newBox.transform.localScale = (m_charBase == null) ?  ls : m_orient.OrientVectorToDirection2D(ls , false);
    //    } else {
    //        newBox.SetScale ((m_charBase == null) ? hitboxScale : m_orient.OrientVectorToDirection2D(hitboxScale,false));
    //    }
    //    newBox.Damage = damage;
    //    newBox.Duration = hitboxDuration;
    //    newBox.Knockback = (m_charBase == null) ? knockback : m_orient.OrientVectorToDirection2D(knockback);
    //    newBox.IsFixedKnockback = fixedKnockback;
    //    newBox.Stun = stun;
    //    newBox.AddElement(element);
    //    newBox.Creator = gameObject;
    //    newBox.Faction = Faction;

    //    ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newBox)); 
    //    newBox.Init();
    //    return newBox;
    //}

    //public HitboxMulti CreateHitboxMulti(Vector3 hitboxScale, Vector3 offset, float damage, float stun, float hitboxDuration, Vector3 knockback, bool fixedKnockback = true,
    //    bool followObj = true, ElementType element = ElementType.PHYSICAL, float refreshTime = 0.2f)
    //{
    //    Vector3 cOff = (m_charBase == null) ? offset : m_orient.OrientVectorToDirection(offset);
    //    Vector3 newPos = transform.position + (Vector3)cOff;
    //    var go = GameObject.Instantiate(ListHitboxes.Instance.HitboxMulti, newPos, Quaternion.identity);
    //    HitboxMulti newBox = go.GetComponent<HitboxMulti>();

    //    if (followObj) {
    //        go.transform.SetParent (gameObject.transform);
    //        Vector3 ls = new Vector3(hitboxScale.x / transform.localScale.x, hitboxScale.y / transform.localScale.y, hitboxScale.z / transform.localScale.z);
    //        newBox.transform.localScale = (m_charBase == null) ?  ls : m_orient.OrientVectorToDirection2D(ls , false);
    //    } else {
    //        newBox.SetScale ((m_charBase == null) ? hitboxScale : m_orient.OrientVectorToDirection2D(hitboxScale,false));
    //    }
    //    newBox.Damage = damage;
    //    newBox.Duration = hitboxDuration;
    //    newBox.Knockback = (m_charBase == null) ? knockback : m_orient.OrientVectorToDirection2D(knockback);
    //    newBox.IsFixedKnockback = fixedKnockback;
    //    newBox.Stun = stun;
    //    newBox.AddElement(element);
    //    newBox.Creator = gameObject;
    //    newBox.Faction = Faction;
    //    newBox.refreshTime = refreshTime;

    //    ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newBox));
    //    newBox.Init();
    //    return newBox;
    //}
    public GameObject CreateItem( GameObject prefab, Vector3 creationPoint, Vector3 throwPoint,
        float throwSpeed) {
        Vector3 cOff = (m_charBase == null) ? creationPoint : m_orient.OrientVectorToDirection(creationPoint);
        Vector3 newPos = transform.position + (Vector3)cOff;
        GameObject go = Instantiate (prefab, newPos, Quaternion.identity);
        if (go.GetComponent<MovementBase> () != null) {
            Vector3 throwVec = throwSpeed * throwPoint.normalized;
            go.GetComponent<BasicPhysics> ().AddToVelocity (m_orient.OrientVectorToDirection (throwVec));
        }
        return go;
    }
    
    public Projectile CreateProjectile(ProjectileInfo pi)
    {
        GameObject go;
        if (pi.Projectile != null)
        {
            go = GameObject.Instantiate(pi.Projectile, transform.position, Quaternion.identity);
        }
        else
        {
            go = GameObject.Instantiate(ListHitboxes.Instance.StandardProjectile, transform.position, Quaternion.identity);
        }
        go.GetComponent<Projectile>().InitFromProjectileInfo(pi, gameObject, GetComponent<FactionHolder>());
        return go.GetComponent<Projectile>();
    }

    public Projectile CreateProjectile(GameObject prefab, Vector3 creationPoint, Vector3 targetPoint,
        float projectileSpeed, float damage, float stun, float projectileDuration, Vector3 knockback, 
        bool fixedKnockback = true, ElementType element = ElementType.PHYSICAL, int Penetration = 0)
    {
        Vector3 cOff = (m_charBase == null) ? creationPoint : m_orient.OrientVectorToDirection2D(creationPoint);
        Vector3 newPos = transform.position + (Vector3)cOff;
        GameObject go;
        if (prefab != null) {
            go = GameObject.Instantiate (prefab, newPos, Quaternion.identity);
        } else {
            go = GameObject.Instantiate (ListHitboxes.Instance.StandardProjectile, newPos, Quaternion.identity);
        }
        Projectile newProjectile = go.GetComponent<Projectile>();

        newProjectile.Damage = damage;
        newProjectile.Duration = projectileDuration;
        newProjectile.Knockback = (m_charBase == null) ? knockback : m_orient.OrientVectorToDirection2D(knockback);
        newProjectile.IsFixedKnockback = fixedKnockback;
        newProjectile.Stun = stun;
        newProjectile.PenetrativePower = Penetration;
        newProjectile.AddElement(element);
        newProjectile.Creator = gameObject;
        //newProjectile.Faction = Faction;
        newProjectile.AimPoint = (m_charBase == null) ? targetPoint : m_orient.OrientVectorToDirection2D(targetPoint);
        newProjectile.ProjectileSpeed = projectileSpeed;

        //ExecuteEvents.Execute<ICustomMessageTarget> (gameObject, null, (x, y) => x.OnHitboxCreate(newProjectile));
        newProjectile.Init();
        return newProjectile;
    }

    public void ClearHitboxes()
	{
		foreach (Hitbox hb in GetComponentsInChildren<Hitbox>())
			Destroy(hb.gameObject);
	}

	public void RegisterHit(GameObject otherObj, HitInfo hi, HitResult hr)
	{
		//if (m_charBase)
  //          m_charBase.RegisterHit (otherObj,hi,hr);
	}
    public void QueueProjectile(ProjectileInfo pi, float delay)
    {
        m_queuedProjectiles.Add(pi, Time.timeSinceLevelLoad + delay);
    }
    public void QueueHitbox(HitboxInfo hi, float delay)
    {
        m_queuedHitboxes.Add(hi, Time.timeSinceLevelLoad + delay);
    }
    private void updateQueueActions()
    {
        Dictionary<HitboxInfo, float> newQueue = new Dictionary<HitboxInfo, float>();
        foreach (HitboxInfo hi in m_queuedHitboxes.Keys)
        {
            if (Time.timeSinceLevelLoad > m_queuedHitboxes[hi])
                GetComponent<HitboxMaker>().CreateHitbox(hi);
            else
                newQueue.Add(hi, m_queuedHitboxes[hi]);
        }
        m_queuedHitboxes = newQueue;
        Dictionary<ProjectileInfo, float> newQueue2 = new Dictionary<ProjectileInfo, float>();
        foreach (ProjectileInfo pi in m_queuedProjectiles.Keys)
        {
            if (Time.timeSinceLevelLoad > m_queuedProjectiles[pi])
                CreateProjectile(pi);
            else
                newQueue2.Add(pi, m_queuedProjectiles[pi]);
        }
        m_queuedProjectiles = newQueue2;
    }
}
