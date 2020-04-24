using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TROnSound : Transition
{
    public NoiseType TriggerOnNoiseType = NoiseType.NONE;

    private void Start()
    {
        MasterAI.registerEvent("sound", OnSound);
    }
    private void OnSound(List<Object> soundHeard)
    {
        System.Object sound = soundHeard[0];
        NoiseForTrigger nft = (NoiseForTrigger)sound;
        if (nft != null && nft.noiseType == TriggerOnNoiseType)
        {
            TriggerTransition();
        }
    }
}
