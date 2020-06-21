using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RHSGenerateResources : RHStatement
{
    private List<RHResource> m_Resources = new List<RHResource>();
    // Start is called before the first frame update
    public void addResource(RHResource r)
    {
        if (r.m_resourceType != RHResourceType.NONE)
            m_Resources.Add(r);
    }
    private void Start()
    {
        init();
    }
    
    
    public void distributeResources()
    {
        List<RHResource> m_distributedResources = new List<RHResource>();
        int numResourcesToDistribute = 0;
        foreach (RHResource rg in m_Resources)
        {
            m_distributedResources.Add(rg);
            numResourcesToDistribute++;
        }
        int numEventsCreated = 0;
        foreach (RHResource rg in m_distributedResources)
        {
            numEventsCreated++;
            RHEvent ev = gameObject.AddComponent<RHEvent>();
            ev.AddResource(rg);
            ev.TimeOffsetFromStatementStart = (Time / (numResourcesToDistribute+1)) * numEventsCreated;
            m_eventList.Add(ev);
        }
    }
}
