using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance
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
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void Reset()
    {
        SaveObjManager.charContainer = new CharacterSaveContainer();
        Instance.GetComponent<SaveObjManager>().SetDirectory("AutoSave");
        Instance.GetComponent<SaveObjManager>().resetRoomData();
    }
}
