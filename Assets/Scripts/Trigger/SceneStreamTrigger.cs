using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStreamTrigger : Interactable
{
    public bool SceneMoveTrigger = true;
    public List<SceneField> ScenesToLoad;
    public List<SceneField> ScenesToUnLoad;
    public SceneField MoveToScene;

    void Update() { }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.8f, 0, .5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    protected override void onTrigger(GameObject interactor)
    {
        if (interactor != null)
        {
            
            if (interactor.GetComponent<Attackable>().Alive == false)
                return;
            Debug.Log("Triggered");
            Debug.Log(CurrentPlayerSettings.GetCurrentPlayer());
            Debug.Log(interactor);
            if (CurrentPlayerSettings.GetCurrentPlayer() == interactor)
            {
                Debug.Log("Loading scenes");
                SceneStreamManager.LoadStreamedScenes(ScenesToLoad);
            }
                
            if (MoveToScene != null)
                SceneStreamManager.MoveObjectToScene(interactor, MoveToScene);
        }
    }
}