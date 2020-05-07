using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Verb : LogicalConcept
{
    public virtual float IsA(ActionInfo action, Observer perspective)
    {
        return 0.0f;
    }
}
