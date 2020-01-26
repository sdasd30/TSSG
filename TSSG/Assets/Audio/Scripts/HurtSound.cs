using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class HurtSound : MonoBehaviour
{
    RecieveSFX manager;
    public List<AudioClip> sounds = new List<AudioClip>();

    public void playSound()
    {
        RecieveSFX[] objs = FindObjectsOfType<RecieveSFX>();
        foreach (RecieveSFX item in objs)
        {
            if (item.identifySelf().Equals("Living"))
            {
                //Debug.Log("Living manager identified");
                manager = item;
            }
        }

        if (manager == null)
            Debug.Log("No LivingSFXManager in scene! Create one for death sounds to play");
        manager.playSound(sounds[Random.Range(0, sounds.Count)]);
    }
}
