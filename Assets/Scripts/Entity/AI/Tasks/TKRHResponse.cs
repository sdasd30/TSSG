using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TKRHResponse : Task
{
    [SerializeField]
    private float authorityResponseModifier;
    [SerializeField]
    private float favorabilityResponseModifier;

    public override void TriggerEvent(AIEvent aie)
    {
        AIEVStatementReceived aiev = aie as AIEVStatementReceived;
        
        if (aiev == null)
            return;
        RHStatement newStatement = aiev.statement.GenerateStandardResponse(aiev.speaker, MasterAI.GetComponent<RHListener>());
        if (newStatement != null)
            aiev.conversation.QueueStatement(newStatement, MasterAI.GetComponent<RHSpeaker>());
        //RHStatement statementReceived = aiev.statement;
        //RHStatement response = statementReceived.GenerateResponse( MasterAI.GetComponent<RHListener>());

        //aiev.conversation.QueueStatement(response, MasterAI.GetComponent<RHSpeaker>());
    }

    
}
