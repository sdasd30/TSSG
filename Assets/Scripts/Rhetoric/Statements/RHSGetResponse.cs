using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHSGetResponse : RHStatement
{
    public RHResourceType m_mainResource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override RHStatement GenerateStandardResponse(RHSpeaker originalSpeaker, RHListener originalListener)
    {
        if (!IsGenerateResponse(originalListener.GetTrust(originalSpeaker,true)))
            return base.GenerateStandardResponse(originalSpeaker, originalListener);
        int intensity = Mathf.RoundToInt(Mathf.Max(1, originalListener.GetEmotionalIntensity() / 30f));
        RHSGenerateResources response = GetEmptyGenerateResourcesStatement();
        response.addResource(new RHResource(m_mainResource, intensity));
        return response;
    }

    private bool IsGenerateResponse(float Trust)
    {
        if (Trust > 0.0f)
        {
            if (RandomChanceRange(100f, 60f + ((Trust / 100f) * 40f), false))
                return true;
            else
                return false;
        }
        else
        {
            if (RandomChanceRange(100f, 60f - (((100 + Trust )/ 100f) * 60f), false))
                return true;
            else
                return false;
        }
    }
}
