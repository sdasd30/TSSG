using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHealth : PowerUpBase
{
    public float newRate = 4.0f;
    public float length = 1f;


    public override void TriggerEffect(GameObject target) {
        if (target.GetComponent<Regenerate>() != null)
        {
            FindObjectOfType<Regenerate>().TemporaryRegenChange(length, newRate);
        }    
    }

    
}