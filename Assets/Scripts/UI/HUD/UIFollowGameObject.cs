using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowGameObject : MonoBehaviour
{
    public GameObject TargetObject;
    public Vector3 LocalPosition;

    public bool CopyPosition = true;
    public bool CopyRotation = false;
    public bool CopyScale = false;
    public bool DestroyOnTargetDestroyed = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetObject == null)
        {
            if (DestroyOnTargetDestroyed)
                Destroy(gameObject);
        } else
        {
            if (CopyPosition)
                transform.position = TargetObject.transform.position + LocalPosition;
            if (CopyRotation)
                transform.rotation = TargetObject.transform.rotation;
            if (CopyScale)
                transform.localScale = TargetObject.transform.localScale;
        }
    }
}
