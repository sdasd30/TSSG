using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRRHConversation: Transition
{
    private void Start()
    {
        MasterAI.registerEvent(typeof(AIEVConversationStarted), OnStatement);
    }
    private void OnStatement(AIEvent OnSound)
    {
        TriggerTransition();
    }
}
