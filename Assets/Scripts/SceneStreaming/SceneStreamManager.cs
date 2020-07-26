using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStreamManager : MonoBehaviour
{
    private static SceneStreamManager m_instance;
    private List<Scene> ScenesCurrentlyLoaded = new List<Scene>();

    private const int MAX_SCENES_LOADED = 5;
    private const int TARGET_SCENES_LOADED = 3;

    public static SceneStreamManager Instance
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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void LoadStreamedScenes(List<SceneField> scenesToLoad)
    {
        if (scenesToLoad.Count < 1)
            return;
        SaveObjManager.Instance.refreshPersItems();
        foreach (SceneField sf in scenesToLoad)
        {
            Scene s = SceneManager.GetSceneByName(sf.SceneName);
            Debug.Log(s.name);
            if (s.name == null)
                SceneManager.LoadScene(sf.SceneName, LoadSceneMode.Additive);
        }
    }
    public static void UnLoadStreamedScenes(List<SceneField> scenesToUnLoad)
    {
        if (scenesToUnLoad.Count < 1)
            return;
        SaveObjManager.Instance.refreshPersItems();
        foreach (SceneField sf in scenesToUnLoad)
        {
            Scene s = SceneManager.GetSceneByName(sf.SceneName);
            if (s != null)
                SceneManager.UnloadScene(sf.SceneName);
        }
    }

    public static void MoveObjectToScene( GameObject go, SceneField targetScene)
    {

        if (targetScene.SceneName.Length < 1)
            return;
        Scene s = SceneManager.GetSceneByName(targetScene.SceneName);
        Debug.Log(s);
        if (s != null)
        {
            SceneManager.MoveGameObjectToScene(go, s);
            SaveObjManager.MoveItem(go, targetScene.SceneName, go.transform.position);
        } else
        {
            SaveObjManager.MoveItem(go, targetScene.SceneName, go.transform.position);
            Destroy(go);
        }

    }

}
