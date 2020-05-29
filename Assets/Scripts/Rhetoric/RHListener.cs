using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHListener : MonoBehaviour
{
    public float EmotionalIntensity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float ApplyStatementModifiers(float baseValue, RHListener listener, RHStatement statement)
    {
        return baseValue;
    }
    public float AddBaseValueModifier(RHSpeaker speaker, RHConversation conversation)
    {
        return 0f;
    }

    private float GetAuthority(RHSpeaker speaker)
    {
        return GetComponent<Observer>().GetImpressionModifiers(speaker.gameObject.name, new NOUNAuthority());
    }
    private float GetTrust(RHSpeaker speaker)
    {
        return GetComponent<Observer>().GetImpressionModifiers(speaker.gameObject.name, new NOUNTrustable());
    }
    private float GetFavor(RHSpeaker speaker)
    {
        return GetComponent<Observer>().GetImpressionModifiers(speaker.gameObject.name, new NOUNFavorable());
    }
}
