using UnityEngine;
using System.Collections.Generic;

public enum HitResult { NONE, FOCUSHIT, HIT, HEAL, BLOCKED, REFLECTED };

public enum ElementType { PHYSICAL, FIRE, BIOLOGICAL, PSYCHIC, LIGHTNING };

public class HitInfo
{
    public float Damage = 10f;
    public float FocusDamage = 10f;
    public float Penetration = 0f;
    public Vector3 Knockback = new Vector3();
    public bool IsFixedKnockback = false;
    public bool ResetKnockback = true;
    public float Stun = 0f;
    public float FreezeTime = 0f;
    public List<ElementType> Element = new List<ElementType>();
    public Hitbox mHitbox;

    public GameObject Creator;
    public DamageSource OriginSource;
    public GameObject target;
    public float LastTimeHit;

    public bool HasElement(ElementType element)
    {
        foreach (ElementType et in Element)
        {
            if (et == element)
                return true;
        }
        return false;
    }
    public float TimeSinceLastHit()
    {
        return Time.timeSinceLevelLoad - LastTimeHit;
    }
}
public delegate void OnRegisterHit(GameObject attackable, HitInfo hitinfo, HitResult hitResult);

public class Hitbox : MonoBehaviour
{
    private DamageSource m_originSource;
    public DamageSource OriginSource { get { return m_originSource; } set { m_originSource = value; } }

    [SerializeField]
    private float m_damage = 10.0f;
    public float Damage { get { return m_damage; } set { m_damage = value; } }

    [SerializeField]
    private float m_focusDamage = -1f;
    public float FocusDamage { get { return m_focusDamage; } set { m_focusDamage = value; } }

    [SerializeField]
    private float m_penetration = 0.0f;
    public float Penetration { get { return m_penetration; } set { m_penetration = value; } }


    [SerializeField]
    private float m_duration = 1.0f;
    public float Duration { get { return m_duration; } set { m_duration = value; } }

    protected float m_remainingDuration = 1.0f;

    [SerializeField]
    protected bool m_hasDuration = true;

    [SerializeField]
    private bool m_isFixedKnockback = false;
    public bool IsFixedKnockback { get { return m_isFixedKnockback; } set { m_isFixedKnockback = value; } }

    [SerializeField]
    private bool m_resetKnockback = true;
    public bool IsResetKnockback { get { return m_resetKnockback; } set { m_resetKnockback = value; } }

    [SerializeField]
    private Vector3 m_knockback = new Vector3(0.0f, 40.0f);
    public Vector3 Knockback { get { return m_knockback; } set { m_knockback = value; } }

    [SerializeField]
    private float m_stun = 0.0f;
    public float Stun { get { return m_stun; } set { m_stun = value; } }

    [SerializeField]
    private bool m_isRandomKnockback = false;
    public bool IsRandomKnockback { get { return m_isRandomKnockback; } set { m_isRandomKnockback = value; } }

    [SerializeField]
    private float m_FreezeTime = 0f;
    public float FreezeTime { get { return m_FreezeTime; } set { m_FreezeTime = value; } }

    public bool Deflector = false;

    //private ElementType m_element = ElementType.PHYSICAL;
    [SerializeField]
    private List<ElementType> m_elementList = new List<ElementType>();
    public List<ElementType> Element { get { return m_elementList; } set { m_elementList = value; } }

    [HideInInspector]
    public GameObject Creator { get; set; }

    [SerializeField]
    private GameObject m_followObj;

    [SerializeField]
    public Vector3 m_followOffset;

    private Vector4 m_knockbackRanges;
    public List<Attackable> m_collidedObjs = new List<Attackable>();
    public List<Attackable> m_overlappingControl = new List<Attackable>();
    public List<OnRegisterHit> hitCallback = new List<OnRegisterHit>();

    public bool HitboxActive { get { return m_hitboxActive; } private set { m_hitboxActive = value; } }

    private bool m_hitboxActive = true;

    virtual public void Init()
    {
        if (m_focusDamage == -1f)
            m_focusDamage = m_damage;

        if (m_isRandomKnockback)
            RandomizeKnockback();
        m_hasDuration = m_duration > 0;
        Tick();
        //Debug.Log ("Hitbox initialized");
        m_remainingDuration = m_duration;
        if (m_elementList.Count == 0)
            m_elementList.Add(ElementType.PHYSICAL);
    }

    virtual internal void Update()
    {
        Tick();
    }

    protected virtual void Tick()
    {
        if (m_hasDuration)
            MaintainOrDestroyHitbox();
    }

    public void SetScale(Vector2 scale)
    {
        transform.localScale = scale;
    }

    public void SetFollow(GameObject obj, Vector2 offset)
    {
        m_followObj = obj;
        m_followOffset = offset;
        Vector3 newOffset = new Vector3(offset.x, offset.y, 0f);
        if (m_followObj.GetComponent<BasicPhysics>() != null &&
            m_followObj.GetComponent<BasicPhysics>().GetComponent<Orientation>().FacingLeft)
        {
            newOffset.x *= -1f;
            //Debug.Log ("Reverse");
        }
        m_followOffset = newOffset;
    }

    public void SetKnockbackRanges(float minX, float maxX, float minY, float maxY)
    {
        IsRandomKnockback = true;
        IsFixedKnockback = true;
        m_knockbackRanges = new Vector4(minX, maxX, minY, maxY);
    }

    //public HitboxInfo ToHitboxInfo()
    //{
    //    HitboxInfo hbi = new HitboxInfo();
    //    return hbi;

    //}
    private void MaintainOrDestroyHitbox()
    {
        if (m_remainingDuration <= 0.0f)
        {
            //Debug.Log ("Hitbox destroyed!" + m_duration);
            GameObject.Destroy(gameObject);
        }
        m_remainingDuration -= Time.deltaTime;
    }
    public void ResetDuration()
    {
        m_remainingDuration = m_duration;
    }
    private void FollowObj()
    {
        //transform.position = new Vector3(m_followObj.transform.position.x + m_followOffset.x, m_followObj.transform.position.y + m_followOffset.y, 0);
    }

    private void RandomizeKnockback()
    {
        m_knockback.x = Random.Range(m_knockbackRanges.x, m_knockbackRanges.y);
        m_knockback.y = Random.Range(m_knockbackRanges.z, m_knockbackRanges.w);
    }

    protected virtual HitResult OnAttackable(Attackable atkObj)
    {
        if (atkObj == null || (!canAttack(atkObj)))
            return HitResult.NONE;
        if (IsRandomKnockback)
            RandomizeKnockback();
        HitInfo newHI = ToHitInfo();
        newHI.LastTimeHit = Time.timeSinceLevelLoad;
        newHI.Knockback = Knockback;
        newHI.target = atkObj.gameObject;
        //HitResult r = atkObj.TakeHit(newHI);
        HitResult r = atkObj.TakeHit(newHI);
        m_collidedObjs.Add(atkObj);

        if (!m_overlappingControl.Contains(atkObj))
            m_overlappingControl.Add(atkObj);
        foreach(OnRegisterHit orh in hitCallback)
        {
            orh.Invoke(atkObj.gameObject, newHI, r);
        }
        //CreateHitFX(atkObj.gameObject, Knockback, r);
        return HitResult.HIT;
    }

    public virtual HitInfo ToHitInfo()
    {
        HitInfo hi = new HitInfo();
        hi.Damage = Damage;
        hi.FocusDamage = FocusDamage;
        hi.Stun = Stun;
        hi.Element = m_elementList;
        hi.Penetration = Penetration;
        hi.IsFixedKnockback = IsFixedKnockback;
        hi.ResetKnockback = m_resetKnockback;
        //Debug.Log (hi.ResetKnockback);
        hi.FreezeTime = m_FreezeTime;
        //Debug.Log ("MFreeze: " + m_FreezeTime);

        hi.Creator = Creator;
        hi.OriginSource = OriginSource;
        hi.mHitbox = this;
        
        return hi;
    }

    protected bool canAttack(Attackable atkObj)
    {
        return GetComponent<FactionHolder>().CanAttack(atkObj);
    }

    internal HitResult OnTriggerEnter(Collider other)
    {
        if (other == null)
            return HitResult.NONE;
        if (!m_hitboxActive)
            return HitResult.NONE;
        OnHitObject(other);
        if (Deflector && other.gameObject.GetComponent<Projectile>() != null &&
            GetComponent<FactionHolder>().CanAttack(other.gameObject))
            OnDeflectProjectile(other.gameObject.GetComponent<Projectile>());
        if (other.gameObject.GetComponent<Attackable>() == null)
            return HitResult.NONE;
        return OnAttackable(other.gameObject.GetComponent<Attackable>());
    }
    protected virtual void OnDeflectProjectile(Projectile projectile)
    {
        GetComponent<FactionHolder>().SetFaction(projectile.gameObject);
        Vector2 ap = projectile.AimPoint;
        projectile.SetAimPoint(new Vector2(-ap.x, -ap.y));
        projectile.ResetDuration();
    }
    protected virtual void OnHitObject(Collider other)
    {
    }
    internal void OnTriggerExit(Collider other)
    {
        /*
		 * TODO: Delay removal of collided object to avoid stuttered collisions 
		 */
        /*
		if (other.gameObject.GetComponent<Attackable> () && collidedObjs.Contains(other.gameObject.GetComponent<Attackable>())) {
			collidedObjs.Remove (other.gameObject.GetComponent<Attackable> ());
		}
		*/
        if (!m_hitboxActive)
            return;
        if (other.gameObject.GetComponent<Attackable>()
            && m_overlappingControl.Contains(other.gameObject.GetComponent<Attackable>()))
        {
            m_overlappingControl.Remove(other.gameObject.GetComponent<Attackable>());
        }
    }

    /*private void SwitchActiveCollider(bool FacingLeft)
    {
        if (m_upRightDownLeftColliders.Count == 0)
            return;
        var dirIndex = ConvertDirToUpRightDownLeftIndex(FacingLeft);
        // Or'd check on enabled in case collider falls under several categories
        for (var i = 0; i < m_upRightDownLeftColliders.Count; i++)
        {
            m_upRightDownLeftColliders[i].enabled |= (i == dirIndex);
        }
    }

    private int ConvertDirToUpRightDownLeftIndex(bool FacingLeft)
    {
        if (FacingLeft)
            return 3;
        return 1;
    }*/

    void OnDrawGizmos()
    {
        //Gizmos.color = new Color(1, 0, 0, .8f);
        //Gizmos.DrawCube(transform.position, transform.lossyScale);
    }

    public void AddElement(ElementType element)
    {
        m_elementList.Add(element);
    }
    public bool HasElement(ElementType element)
    {
        foreach (ElementType et in m_elementList)
        {
            if (et == element)
                return true;
        }
        return false;
    }
    public void ClearElement()
    {
        m_elementList.Clear();
    }
    //protected void CreateHitFX(GameObject hitObj, Vector2 knockback, HitResult hr)
    //{
    //    foreach (ElementType et in m_elementList)
    //    {
    //        m_hitFX(et, hitObj, knockback, hr);
    //        m_playerSound(et, hr);
    //    }
    //}
    //private void m_hitFX(ElementType et, GameObject hitObj, Vector2 knockback, HitResult hr)
    //{
    //    GameObject fx = null;
    //    if (hr == HitResult.BLOCKED || hr == HitResult.FOCUSHIT)
    //    {
    //        fx = GameObject.Instantiate(FXHit.Instance.FXHitBlock, hitObj.transform.position, Quaternion.identity);
    //    }
    //    if (hr == HitResult.HEAL)
    //    {
    //        fx = GameObject.Instantiate(FXHit.Instance.FXHeal, hitObj.transform.position, Quaternion.identity);
    //    }
    //    else if (hr == HitResult.HIT || hr == HitResult.FOCUSHIT)
    //    {
    //        switch (et)
    //        {
    //            case ElementType.PHYSICAL:
    //                fx = GameObject.Instantiate(FXHit.Instance.FXHitPhysical, hitObj.transform.position, Quaternion.identity);
    //                break;
    //            case ElementType.FIRE:
    //                fx = GameObject.Instantiate(FXHit.Instance.FXHitFire, hitObj.transform.position, Quaternion.identity);
    //                break;
    //            case ElementType.LIGHTNING:
    //                fx = GameObject.Instantiate(FXHit.Instance.FXHitLightning, hitObj.transform.position, Quaternion.identity);
    //                break;
    //            case ElementType.BIOLOGICAL:
    //                fx = GameObject.Instantiate(FXHit.Instance.FXHitBiological, hitObj.transform.position, Quaternion.identity);
    //                break;
    //            case ElementType.PSYCHIC:
    //                fx = GameObject.Instantiate(FXHit.Instance.FXHitPsychic, hitObj.transform.position, Quaternion.identity);
    //                break;
    //            default:
    //                Debug.Log("Hit Effect not yet added");
    //                break;
    //        }
    //    }
    //    if (fx != null)
    //    {
    //        fx.GetComponent<Follow>().followObj = hitObj;
    //        float angle = (Mathf.Atan2(knockback.y, knockback.x) * 180) / Mathf.PI;
    //        fx.transform.Rotate(new Vector3(0f, 0f, angle));
    //    }
    //}
    //protected void m_playerSound(ElementType et, HitResult hr)
    //{

    //    if (hr == HitResult.BLOCKED || hr == HitResult.FOCUSHIT)
    //    {
    //        FindObjectOfType<AudioManager>().PlayClipAtPos(FXHit.Instance.SFXGuard, transform.position, 0.5f, 0f, 0.25f);
    //    }
    //    else if (hr == HitResult.HEAL)
    //    {
    //        FindObjectOfType<AudioManager>().PlayClipAtPos(FXHit.Instance.SFXHeal, transform.position, 0.5f, 0f, 0.25f);
    //    }
    //    else if (hr == HitResult.HIT)
    //    {
    //        switch (et)
    //        {
    //            case ElementType.PHYSICAL:
    //                FindObjectOfType<AudioManager>().PlayClipAtPos(FXHit.Instance.SFXPhysical, transform.position, 0.5f, 0f, 0.25f);
    //                break;
    //            case ElementType.FIRE:
    //                FindObjectOfType<AudioManager>().PlayClipAtPos(FXHit.Instance.SFXFire, transform.position, 0.5f, 0f, 0.25f);
    //                break;
    //            case ElementType.LIGHTNING:
    //                FindObjectOfType<AudioManager>().PlayClipAtPos(FXHit.Instance.SFXElectric, transform.position, 0.75f, 0f, 0.25f);
    //                break;
    //            case ElementType.BIOLOGICAL:
    //                //FindObjectOfType<AudioManager> ().PlayClipAtPos (FXHit.Instance.SFXPhysical,transform.position,0.5f,0f,0.25f);
    //                break;
    //            case ElementType.PSYCHIC:
    //                FindObjectOfType<AudioManager>().PlayClipAtPos(FXHit.Instance.SFXPsychic, transform.position, 0.5f, 0f, 0.25f);
    //                break;
    //            default:
    //                Debug.Log("Hit Effect not yet added");
    //                break;
    //        }
    //    }
    //}

    public virtual void SetHitboxActive(bool a)
    {
        m_hitboxActive = a;
        m_collidedObjs.Clear();
        m_overlappingControl.Clear();
    }

    public void InitFromHitboxInfo(HitboxInfo hbi, GameObject parent, FactionHolder fh = null)
    {        
        if (hbi.FollowCharacter)
        {
            transform.SetParent(parent.transform);
            transform.localScale = new Vector3(hbi.HitboxScale.x / transform.localScale.x, hbi.HitboxScale.y / transform.localScale.y, hbi.HitboxScale.z / transform.localScale.z);
            transform.localPosition = hbi.HitboxOffset;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            SetScale(hbi.HitboxScale);// : orient.OrientVectorToDirection2D(hbi.HitboxScale, false));
        }
        Damage = hbi.Damage;
        OriginSource = hbi.OriginSource;

        FocusDamage = hbi.FocusDamage;
        Penetration = hbi.Penetration;
        Duration = hbi.HitboxDuration;
        float deg = Mathf.Deg2Rad * parent.transform.rotation.eulerAngles.y;
        Knockback = Orientation.OrientToVectorZ(hbi.Knockback, deg);// (orient == null) ? hbi.Knockback : orient.OrientVectorToDirection2D(hbi.Knockback);
        IsFixedKnockback = hbi.FixedKnockback;
        Stun = hbi.Stun;
        FreezeTime = hbi.FreezeTime;
        AddElement(hbi.Element);
        Creator = parent;
        hitCallback = hbi.hitCallback;
        if (fh != null)
            fh.SetFaction(gameObject);
        IsResetKnockback = hbi.ResetKnockback;
        if (hbi.FollowCharacter)
            SetFollow(parent, hbi.HitboxOffset);
        hitCallback = hbi.hitCallback;
        if (hbi.MyNoiseType != NoiseType.NONE)
        {
            if (GetComponent<NoiseAI>() == null)
                gameObject.AddComponent<NoiseAI>();
            NoiseAI noise = GetComponent<NoiseAI>();
            noise.PlaySound(hbi.MyNoiseType, hbi.NoiseRange);
        }
        Init();
    }
}