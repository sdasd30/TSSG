using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RHResource 
{
    public RHResourceType m_resourceType = RHResourceType.POSITIVE;
    public virtual RHResourceType ResourceType { get { return m_resourceType; } }
    public int m_Amount;
    public const float PositiveScaleValue = 0.5f;
    public const float NegativeScaleValue = -0.5f;
    public const float QuestionInterestScale = -0.2f;
    public const float StoryInterestScale = 0.2f;
    public RHResource(RHResourceType resourceType, int stack = 1)
    {
        this.m_resourceType = resourceType;
        this.m_Amount = stack;
    }
    public virtual void OnUpdate(RHSpeaker speaker, RHListener listener, RHConversation conversation, float scaledDelta)
    {
        float modValue = 0;
        switch (m_resourceType)
        {
            case RHResourceType.POSITIVE:
                modValue = scaledDelta * PositiveScaleValue * Mathf.Min(3, m_Amount);
                conversation.ModifyListenerValue(listener, modValue);
                break;
            case RHResourceType.NEGATIVE:
                
                modValue = scaledDelta * NegativeScaleValue * Mathf.Min(3, m_Amount);
                conversation.ModifyListenerValue(listener, modValue);
                break;
            case RHResourceType.QUESTION:
                modValue = scaledDelta * QuestionInterestScale * Mathf.Min(3, m_Amount);
                conversation.ModifyInterestTime( modValue);
                break;
            case RHResourceType.PERSONAL:
                modValue = scaledDelta * StoryInterestScale * Mathf.Min(3, m_Amount);
                conversation.ModifyInterestTime(modValue);
                break;
            case RHResourceType.IDEA:
                
                break;
        }
    }
}
