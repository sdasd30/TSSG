using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRRHStatement : Transition
{
    private void Start()
    {
        MasterAI.registerEvent(typeof(AIEVStatementReceived), OnStatement);
    }
    private void OnStatement(AIEvent OnSound)
    {
        TriggerTransition();
    }
}
