using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDoubleJump : PowerUpBase
{
    public float timeOn = 14;
    public override void TriggerEffect(GameObject target)
    {
        if (target.GetComponent<BasicMovement>() != null)
        {
            target.GetComponent<BasicMovement>().EnableDoubleJumps(timeOn);
        }

    }
}
