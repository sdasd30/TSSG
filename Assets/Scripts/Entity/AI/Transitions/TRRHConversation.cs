using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRRHConversation: Transition
{
    private void Start()
    {
        MasterAI.registerEvent(typeof(AIEVConversationStarted), ConversationStarted);
    }
    private void ConversationStarted(AIEvent ev)
    {
        TriggerTransition();
    }
}
