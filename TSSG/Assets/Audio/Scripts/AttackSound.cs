using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AttackSound : MonoBehaviour
{
    RecieveSFX manager;

    public void playSound(AudioClip sent)
    {
        RecieveSFX[] objs = FindObjectsOfType<RecieveSFX>();
        foreach (RecieveSFX item in objs)
        {
            if (item.identifySelf().Equals("Attacks"))
            {
                //Debug.Log("Attack manager identified");
                manager = item;
            }
        }

        if (manager == null)
            Debug.Log("No SFXManager in scene! Create one for attack sounds to play");
        //Debug.Log("Attempting to play " + sent);    
        manager.playSound(sent);
    }
}
