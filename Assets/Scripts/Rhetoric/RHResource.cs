using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RHResource 
{
    public RHResourceType m_resourceType = RHResourceType.POSITIVE;
    public virtual RHResourceType ResourceType { get { return m_resourceType; } }
    public int m_Amount;
    public RHResource(RHResourceType resourceType, int stack = 1)
    {
        this.m_resourceType = resourceType;
        this.m_Amount = stack;
    }
    public virtual void OnUpdate(RHSpeaker speaker, RHConversation conversation)
    {

    }
}
