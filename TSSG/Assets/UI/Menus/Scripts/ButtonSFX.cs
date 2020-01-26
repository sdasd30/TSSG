using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    RecieveSFX manager;
    public AudioClip ButtonHighlight;
    public AudioClip ButtonClick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ButtonHighlight != null)
            playSound(ButtonHighlight);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (ButtonClick != null)
            playSound(ButtonClick);
    }

    public void playSound(AudioClip sound)
    { 
        manager = FindObjectOfType<RecieveSFX>();


        if (manager == null)
            Debug.Log("No SFXManager in scene! Create one for button sounds to play");
        manager.playSound(sound);
    }
}
