using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionInteract : ActionInfo
{
    protected override void OnAttack()
    {
        base.OnAttack();
        if (GetComponent<Interactor>() != null)
        {
            GetComponent<Interactor>().OnAttemptInteract();
        }
    }
}
