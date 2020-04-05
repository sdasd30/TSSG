using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldCanvasManager : MonoBehaviour
{
    public delegate void IsAcceptUISystem(GameObject targetObject);
    Dictionary<UIWorldUISystem, IsAcceptUISystem> currentUICheckFunctions;

    public GameObject WorldCanvasPrefab;
    private Dictionary<System.Type, int> typeCount;
    private Dictionary<System.Type, List<UIWorldUISystem>> checkedTypes;
    private Dictionary<System.Type, List<Object>> allAccountedForObjects;
    private float nextCheckTime;
    private const float check_interval = 0.25f;

    private static UIWorldCanvasManager m_instance;
    public static UIWorldCanvasManager Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }


    private void Awake()
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
        DontDestroyOnLoad(gameObject);

        currentUICheckFunctions = new Dictionary<UIWorldUISystem, IsAcceptUISystem>();
        typeCount = new Dictionary<System.Type, int>();
        checkedTypes = new Dictionary<System.Type, List<UIWorldUISystem>>();
        allAccountedForObjects = new Dictionary<System.Type, List<Object>>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > nextCheckTime)
        {
            checkForNewObjs();
        }
    }

    public static void AddUISystem(UIWorldUISystem newUISystem)
    {
        if (m_instance ==  null)
        {
            Debug.Log("No UI World Canvas Manager has been initialized");
            return;
        }
        m_instance.m_addUISystem(newUISystem);
    }

    public static GameObject GetWorldCanvasPrefab()
    {
        if (m_instance == null)
        {
            Debug.Log("No UI World Canvas Manager has been initialized");
            return null;
        }
        return m_instance.WorldCanvasPrefab;
    }

    private void m_addUISystem(UIWorldUISystem newUISystem)
    {
        if (currentUICheckFunctions.ContainsKey(newUISystem))
            return;

        currentUICheckFunctions[newUISystem] = newUISystem.AcceptFunction;
        
        System.Type targetType = newUISystem.TargetObjectType;
        if (targetType == null && newUISystem.TargetObjectTypeStr.Length > 0)
            targetType = System.Type.GetType(newUISystem.TargetObjectTypeStr);
        if (!typeCount.ContainsKey(targetType))
        {
            typeCount[targetType] = 0;
            checkedTypes[targetType] = new List<UIWorldUISystem>();
            allAccountedForObjects[targetType] = new List<Object>();
        }
        checkedTypes[targetType].Add(newUISystem);
        foreach(Object o in allAccountedForObjects[targetType])
        {
            if (o != null)
            {
                MonoBehaviour mb = (MonoBehaviour)o;
                if (mb.gameObject != null)
                    newUISystem.AcceptFunction(mb.gameObject);
            }
        }
    }
    private void checkForNewObjs()
    {
        Dictionary<System.Type, int> newL = new Dictionary<System.Type, int>();
        foreach (System.Type t in typeCount.Keys)
        {
            Object[] listObjs = GameManager.FindObjectsOfType(t);
            if (listObjs.Length != typeCount[t])
            {
                UpdateOnNewObj(t, listObjs);
            }
            newL[t] = listObjs.Length;
        }
        typeCount = newL;
        nextCheckTime = Time.timeSinceLevelLoad + check_interval;
    }

    private void UpdateOnNewObj(System.Type t, Object [] listObjs)
    {
        List<Object> newObj = new List<Object>();
        List<Object> oldObj = allAccountedForObjects[t];
        foreach (Object o in listObjs)
        {
            if (!oldObj.Contains(o))
            {
                foreach (UIWorldUISystem obj in checkedTypes[t])
                {
                    MonoBehaviour mb = (MonoBehaviour)o;
                    currentUICheckFunctions[obj](mb.gameObject);
                }
            }
            newObj.Add(o);
        }
        allAccountedForObjects[t] = oldObj;
    }
}
