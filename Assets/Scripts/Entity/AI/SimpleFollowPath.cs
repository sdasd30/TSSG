using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class SimpleFollowPath : MonoBehaviour
{
    public PathCreator pc;
    public float speed = 5;

    public float distTraveled = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distTraveled += speed * ScaledTime.deltaTime;
        transform.position = pc.path.GetPointAtDistance(distTraveled);
    }
}
