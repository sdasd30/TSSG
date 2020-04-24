using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicalObjManager : MonoBehaviour
{
    private static LogicalObjManager m_instance;

    public static LogicalObjManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public static float IsA(GameObject objOne, GameObject objTwo)
    {
        if (objOne.GetComponent<LogicalObject>() == null ||
             objTwo.GetComponent<LogicalObject>() == null)
            return 0f;
        return objOne.GetComponent<LogicalObject>().IsA(objTwo);
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
