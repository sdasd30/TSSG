using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRLogicalConcept : Transition
{
    public List<LogicStatementComponent> SawItemOfTypes;
    public bool WithinZone = true;

    public override void OnTriggerEvent(AIEvent aie)
    {
        if (aie.GetType() != typeof(AIEVSawRelationshipUpdate))
            return;
        AIEVSawRelationshipUpdate newAIE = (AIEVSawRelationshipUpdate)aie;
        bool b = LogicManager.IsA(newAIE.ObservedObj, SawItemOfTypes, GetComponent<Observer>());
        if (b)
            TriggerTransition();
    }
}
