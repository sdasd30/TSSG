using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NOUNWeapon : Noun
{
    public static new float IsA(GameObject targetObject, Observer perspective)
    {
        if (targetObject.GetComponent<Equipment>() == null)
            return -1f;
        float value = -0.2f;
        if (targetObject.GetComponent<EqpWeapon>() != null || targetObject.GetComponent<ActionGunshot>())
            value += 0.5f;
        if (targetObject.GetComponent<ActionInfo>() && targetObject.GetComponent<ActionInfo>().m_HitboxInfo.Count > 0)
        {
            ActionInfo aie = targetObject.GetComponent<ActionInfo>();
            value += Mathf.Min(0.5f, aie.m_HitboxInfo[0].Damage / 15);
        }
        return value;
    }
}
