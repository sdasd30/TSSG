using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VBGunshot : Verb
{
    public override float IsA(ActionInfo action, Observer perspective)
    {
        ActionGunshot g = action as ActionGunshot;
        if (g != null)
            return 1.0f;
        return 0.0f;
    }
}
