using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RHType { LOGOS, PATHOS,ETHOS, NONE}

public class RHStatement : MonoBehaviour
{
    public float Time;
    public RHType RhetoricType;
    private float m_basePower;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual float GetBasePower(RHSpeaker speaker, RHListener listener)
    {
        return m_basePower;
    }
}
