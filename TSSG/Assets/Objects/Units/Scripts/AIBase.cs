using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBase : MonoBehaviour
{
    public virtual InputPacket AITemplate()
    {
        return new InputPacket();
    }
}
