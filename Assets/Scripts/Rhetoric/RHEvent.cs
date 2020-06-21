using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHEvent : MonoBehaviour
{
    public float TimeOffsetFromStatementStart;
    [SerializeField]
    private List<RHResource> m_resource = new List<RHResource>();

    public void ExecuteEvent(RHSpeaker speaker, RHListener target)
    {
        modifySpeakerResources(target);
    }

    public void AddResource(RHResource r)
    {
        m_resource.Add(r);
    }
    private void modifySpeakerResources(RHListener target)
    {
        foreach (RHResource r in m_resource)
            target.GetComponent<RHSpeaker>().AddResource(r);
    }
}