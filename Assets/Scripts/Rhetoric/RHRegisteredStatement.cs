using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHRegisteredStatement
{
    public RHStatement statement;
    public RHSpeaker speaker;
    public RHConversation conversation;
    public RHRegisteredStatement(RHStatement statement, RHSpeaker speaker, RHConversation conversation)
    {
        this.statement = statement;
        this.speaker = speaker;
        this.conversation = conversation;
    }
}