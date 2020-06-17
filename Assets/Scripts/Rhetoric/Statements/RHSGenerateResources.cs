using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RHSGenerateResources : RHStatement
{
    public List<RHResource> m_Resources;
    // Start is called before the first frame update
    private void Start()
    {
        distributeResources();
        init();
    }
    
    private void distributeResources()
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
        }
    }
}
