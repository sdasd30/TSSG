using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileInfo {
	public float Delay = 0f;
	public GameObject Projectile = null;
	public Vector3 ProjectileCreatePos = new Vector3(1.0f, 0f);
	public bool AimTowardsTarget = false;
	public float MaxAngle = 360f;
	public Vector3 ProjectileAimDirection = new Vector3(1.0f, 0f);
	public float ProjectileSpeed = 10.0f;
	public int PenetrativePower = 1;
	public float Damage = 10.0f;
	public float Stun = 0.3f;
	public float HitboxDuration = 0.5f;
	public Vector3 Knockback = new Vector3(10.0f,10.0f);
	public ElementType Element = ElementType.PHYSICAL;

    public NoiseType MyNoiseType;
    public float NoiseRange = 10.0f;

	[HideInInspector]
	public List<OnRegisterHit> hitCallback = new List<OnRegisterHit>();
}


public class ActionProjectile : ActionInfo {
	
	[SerializeField]
	private List<ProjectileInfo> m_ProjectileData;

	protected override void OnAttack()
	{
		base.OnAttack();
		if (m_ProjectileData.Count != 0) {
			createProjectiles ();
		}
	}

	protected void createProjectiles()
	{
		foreach (ProjectileInfo pi in m_ProjectileData) {
			if (pi.Delay <= 0f)
                m_hitboxMaker.CreateProjectile(pi);
			else
                m_hitboxMaker.QueueProjectile (pi, pi.Delay);
		}
	}
}
