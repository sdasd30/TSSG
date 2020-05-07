using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TROnSound : Transition
{
    public NoiseType TriggerOnNoiseType = NoiseType.NONE;

    private void Start()
    {
        MasterAI.registerEvent(typeof(AIEVSound), OnSound);
    }
    private void OnSound(AIEvent OnSound)
    {
        AIEVSound ev = (AIEVSound)OnSound;
        NoiseForTrigger nft = ev.Noise;
        if (nft != null && nft.noiseType == TriggerOnNoiseType)
        {
            TriggerTransition();
        }
    }
}
