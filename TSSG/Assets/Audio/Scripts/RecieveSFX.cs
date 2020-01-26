using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]

public class RecieveSFX : MonoBehaviour
{
    AudioSource aS;
    private void Start()
    {
        aS = GetComponent<AudioSource>();
    }
    public void playSound(AudioClip sound)
    {
        aS.PlayOneShot(sound);
    }

    public string identifySelf()
    {
        //Debug.Log("Asked for audio type, returning: " + aS.outputAudioMixerGroup.name);
        return aS.outputAudioMixerGroup.name;
    }
}
