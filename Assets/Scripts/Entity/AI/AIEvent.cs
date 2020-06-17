using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEvent
{
    public GameObject ObservedObj;
    public bool ToBroadCastSawEvent;
    public bool IsObservationEvent = false;
}

public class AIEVSound : AIEvent {
    public NoiseForTrigger Noise;
    public AIEVSound(NoiseForTrigger noise)
    {
        ToBroadCastSawEvent = true;
        Noise = noise;
    }
}

public class AIEVHitConfirm : AIEvent
{
    public GameObject ObjectHit;
    public HitInfo MyHitInfo;
    public HitResult MyHitResult;
    public AIEVHitConfirm(GameObject objectHit, HitInfo hitInfo, HitResult hitResult)
    {
        ToBroadCastSawEvent = true;
        MyHitInfo = hitInfo;
        MyHitResult = hitResult;
    }
}

public class AIEVObservedAction : AIEvent
{
    public ActionInfo ObservedAction;
    public AIEVObservedAction(GameObject observedObj, ActionInfo action)
    {
        ObservedObj = observedObj;
        ObservedAction = action;
    }
}

public class AIEVItemGet : AIEvent
{
    public Item NewItem;
    public EquipmentSlot Slot;
    public AIEVItemGet(Item newItem, EquipmentSlot slot = null)
    {
        ToBroadCastSawEvent = true;
        NewItem = newItem;
        Slot = slot;
    }
}

public class AIEVSeenByObserver : AIEvent
{
    public AIEVSeenByObserver(Observable me)
    {
        ToBroadCastSawEvent = false;
        ObservedObj = me.gameObject;
    }
}

public class AIEVSawRelationshipUpdate : AIEvent
{
    public Relationship MyRelationship;
    public AIEVSawRelationshipUpdate(GameObject observed,Relationship r)
    {
        ObservedObj = observed;
        MyRelationship = r;
        ToBroadCastSawEvent = false;
    }
}

public class AIEVConversationStarted : AIEvent
{
    public RHConversation conversation;
    public AIEVConversationStarted(RHConversation c)
    {
        conversation = c;
    }
}
public class AIEVStatementReceived : AIEvent
{
    public RHConversation conversation;
    public RHSpeaker speaker;
    public RHStatement statement;
    public AIEVStatementReceived(RHConversation c, RHStatement statement, RHSpeaker speaker)
    {
        conversation = c;
        this.speaker = speaker;
        this.statement = statement;
    }
}
