using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class MusicManager : MonoBehaviour
{
    AudioSource mAudio;



    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

    }

    public void changeMusic(AudioClip prefered)
    {
        mAudio = GetComponent<AudioSource>();
        
        if (mAudio.clip == null || mAudio.clip.name != prefered.name)
        {
            //Debug.Log("Current: " + mAudio.clip.name + "Prefered: " + prefered.name);
            mAudio.clip = prefered;
            mAudio.Play();
        }
        
    }


}