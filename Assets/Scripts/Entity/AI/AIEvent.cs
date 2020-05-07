using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEvent
{
    public System.Type EventType;
    public GameObject ObservedObj;
    public bool ToBroadCastSawEvent;
    public bool IsObservationEvent = false;
}

public class AIEVSound : AIEvent {
    public NoiseForTrigger Noise;
    public AIEVSound(NoiseForTrigger noise)
    {
        EventType = typeof(AIEVSound);
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
        EventType = typeof(AIEVHitConfirm);
        MyHitInfo = hitInfo;
        MyHitResult = hitResult;
    }
}

public class AIEVObserve : AIEvent {
    public GameObject ObservedObj;
}

public class AIEVObservedAction : AIEVObserve
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
        NewItem = newItem;
        Slot = slot;
    }
}
