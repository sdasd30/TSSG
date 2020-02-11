using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioOnCreate : MonoBehaviour
{
    RecieveSFX manager;
    public List<AudioClip> sounds;

    public void Start()
    {
        RecieveSFX[] objs = FindObjectsOfType<RecieveSFX>();
        foreach (RecieveSFX item in objs)
        {
            if (item.identifySelf().Equals("Attacks"))
            {
                Debug.Log("Attack manager identified");
                manager = item;
            }
        }

        if (manager == null)
            Debug.Log("No SFXManager in scene! Create one for attack sounds to play");
        manager.playSound(sounds[Random.Range(0,sounds.Count)]);
    }
}
