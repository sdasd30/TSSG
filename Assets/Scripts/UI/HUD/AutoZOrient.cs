using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoZOrient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float newZ = 360f - transform.parent.rotation.z;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
    //void FixedUpdate()
    //{
    //    float newZ = 360f - transform.parent.rotation.z;
    //    transform.rotation = Quaternion.Euler(90f, 0f, newZ);
    //}
}
