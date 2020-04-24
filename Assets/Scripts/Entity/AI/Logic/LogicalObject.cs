using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicalObject : MonoBehaviour
{

    public string LogicalName;
    public GameObject Parent;

    public virtual float IsA(GameObject logicalObj)
    {
        if (Parent == null)
        {
 
            return (gameObject.name == logicalObj.name) ? 1f : 0f ;
        }
        else
        {
            return Parent.GetComponent<LogicalObject>().IsA(logicalObj);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
