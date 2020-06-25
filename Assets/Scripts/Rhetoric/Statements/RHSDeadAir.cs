using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHSDeadAir : RHStatement
{
    public override RHType RhetoricType { get { return RHType.NONE; } }

    // Start is called before the first frame update
    void Start()
    { }

    // Update is called once per frame
    void Update()
    { }

    public override RHStatement GenerateStandardResponse(RHSpeaker originalSpeaker, RHListener originalListener)
    {

        float length = GetResponseLengthFromAuthority(originalListener.GetAuthority(originalSpeaker, true) + 50f);
        if (length == 0)
            return null;
        else
            return base.GenerateStandardResponse(originalSpeaker, originalListener);
    }
}
