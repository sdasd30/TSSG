using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RHResource 
{
    public virtual RHResourceType m_resourceType { get { return RHResourceType.POSITIVE; } }
    public int m_Amount;

    public virtual void OnUpdate(RHSpeaker speaker, RHConversation conversation)
    {

    }
}
