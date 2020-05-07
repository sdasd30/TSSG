using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VBAttack : Verb
{
    public override float IsA(ActionInfo action, Observer perspective)
    {
        if (action.m_HitboxInfo.Count > 0 && action.m_HitboxInfo[0].Damage > 3)
            return 1.0f;
        return 0.0f;
    }
}