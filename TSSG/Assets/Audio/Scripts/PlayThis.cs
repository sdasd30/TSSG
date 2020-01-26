using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayThis : MonoBehaviour
{
    public AudioClip music;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<MusicManager>().changeMusic(music);
    }
}
