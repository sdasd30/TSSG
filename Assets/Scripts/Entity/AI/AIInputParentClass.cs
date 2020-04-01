using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputParentClass : MonoBehaviour
{
    public virtual InputPacket AITemplate()
    {
        return new InputPacket();
    }
}
