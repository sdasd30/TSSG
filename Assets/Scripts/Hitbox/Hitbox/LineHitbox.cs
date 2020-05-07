using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class LineHitbox : Hitbox {
	public float range = 1000;
	private LineRenderer line;
	public Vector3 aimPoint = new Vector3();
	bool foundPoint = false;
	Vector3 endPoint = new Vector3();

    public int EnemiesCanPenetrate = 0;
    public bool CanPenetrateWall = false;
    public int m_numPenetrated = 0;

    public override void Init ()
	{
		base.Init ();
		line = GetComponent<LineRenderer> ();
		//line.SetVertexCount (2);
		line.positionCount = 2;
		line.startWidth = 0.2f;
		line.startColor = Color.red;
	}

	new void Update ()
	{
		base.Tick ();
		RaycastHit [] hit_list;
		hit_list = Physics.RaycastAll (transform.position, aimPoint, range);
		if (foundPoint) {
			line.SetPosition (0, transform.position);
			line.SetPosition (1, endPoint);
		} else {
			line.SetPosition (0, transform.position);
			line.SetPosition (1, transform.position + new Vector3 ((aimPoint * range).x, (aimPoint * range).y, (aimPoint * range).z));
            float minDistance = float.MaxValue;
            foreach (RaycastHit hit in hit_list) {
				Collider collider = hit.collider;
                float dist = Vector3.Distance(transform.position, hit.point);
                if (dist > minDistance)
                    continue;
                if (!CanPenetrateWall && collider.gameObject != Creator && !collider.isTrigger &&
                    collider.GetComponent<Attackable>() == null)
                {
                    line.SetPosition(0, transform.position);
                    line.SetPosition(1, hit.point);
                    foundPoint = true;
                    endPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    minDistance = Mathf.Min(minDistance, dist);
                }
				HitResult hitType = OnTriggerEnter (collider);
				if (hitType != HitResult.NONE) {
					if (hitType == HitResult.REFLECTED)
						getReflected (hit.point);
                    //OnAttackable(collider.gameObject.GetComponent<Attackable>());
                    if (m_numPenetrated >= EnemiesCanPenetrate)
                    {
                        line.SetPosition(0, transform.position);
                        line.SetPosition(1, hit.point);
                        foundPoint = true;
                        endPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        return;
                    } else
                    {
                        m_numPenetrated++;
                    }
				}
			}
		}
	}

	public void getReflected(Vector3 hitPoint) {
		float offsetX = Random.Range (-15, 15) / 100f;
		float offsetY = Random.Range (-15, 15) / 100f;
        float offsetZ = Random.Range(-15, 15) / 100f;
        Vector2 realD = new Vector3 (-aimPoint.x + offsetX, -aimPoint.y + offsetY,aimPoint.z + offsetZ);
		Vector2 realKB = new Vector3 (-Knockback.x, Knockback.y, Knockback.z);
		realD = Vector3.ClampMagnitude (realD, 1.0f);

		Vector3 newPos = new Vector3(hitPoint.x - aimPoint.x, hitPoint.y - aimPoint.y, 0);
		GameObject go = Instantiate(gameObject,newPos,Quaternion.identity) as GameObject; 
		LineHitbox line = go.GetComponent<LineHitbox> ();
		line.setRange (range);
		line.Damage = Damage;
		line.setAimPoint (realD);
		line.Duration = Duration;
		line.Knockback = realKB;
		line.IsFixedKnockback = true;
		//line.Faction = 
		//line.setFaction (faction);
		line.Creator = gameObject;
		//line.reflect = hitboxReflect;
		line.Stun = Stun;
		//line.mAttr = mAttrs;
	}

	public void setRange(float r) {
		range = r;
	}
	public void setAimPoint(Vector3 aP) {
		aimPoint = aP;
	}

    public void InitFromLineInfo(HitScanInfo hsi, GameObject parent, FactionHolder fh = null)
    {
        float deg = Mathf.Deg2Rad * parent.transform.rotation.eulerAngles.y;
        aimPoint = Orientation.OrientToVectorZ(hsi.AimDirection, deg);
        if (hsi.FollowCharacter)
        {
            transform.SetParent(parent.transform);
            transform.localPosition = hsi.CreatePos;
            transform.localRotation = Quaternion.identity;
        } else
        {
            transform.position += Orientation.OrientToVectorZ(hsi.CreatePos, deg);
        }
        setRange(hsi.MaxRange);
        Damage = hsi.Damage;
        setAimPoint(aimPoint);
        Duration = hsi.HitboxDuration;
        Knockback = Orientation.OrientToVectorZ(hsi.Knockback, deg);
        IsFixedKnockback = true;
        CanPenetrateWall = hsi.CanPenetrateWall;
        Creator = parent;
        EnemiesCanPenetrate = hsi.PenetrativePower;
        if (fh != null)
        {
            fh.SetFaction(gameObject);
        }
        hitCallback = hsi.hitCallback;
        AddElement(hsi.Element);
        Stun = hsi.Stun;
        Init();
    }
}