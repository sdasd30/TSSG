﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : Interactable
{

    public bool onContact = true;
    public string NextSceneName;
    public Vector3 newPos = Vector2.zero;
    public string TriggerID = "none";
    public string TargetTriggerID = "none";
    public Direction dir;

    void Update() { }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, .5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    protected override void onTrigger(GameObject interactor)
    {
        if (interactor != null)
        {
            if (interactor.GetComponent<Attackable>().Alive == false)
                return;
            TriggerUsed = true;
            if (TriggerID != "none")
            {
                Direction realDir = dir;
                string realTarget = TargetTriggerID;
                if (TargetTriggerID == "none")
                {
                    realTarget = TriggerID;
                }
                if (realDir == Direction.NEUTRAL)
                {
                    float diffX = transform.position.x - interactor.transform.position.x;
                    float diffY = transform.position.y - interactor.transform.position.y;
                    if (Mathf.Abs(diffX) > Mathf.Abs(diffY))
                    {
                        if (diffX < 0f)
                        {
                            realDir = Direction.LEFT;
                        }
                        else
                        {
                            realDir = Direction.RIGHT;
                        }
                    }
                    else
                    {
                        if (diffY > 0f)
                        {
                            realDir = Direction.UP;
                        }
                        else
                        {
                            realDir = Direction.DOWN;
                        }
                    }
                }
                if (interactor.GetComponent<PersistentItem>() != null)
                    SaveObjManager.MoveItem(interactor, NextSceneName, realTarget, realDir);
            }
            else if (Vector2.Equals(Vector2.zero, newPos) && (interactor.GetComponent<PersistentItem>() != null))
            {
                SaveObjManager.MoveItem(interactor, NextSceneName, interactor.gameObject.transform.position);
            }
            else if (interactor.GetComponent<PersistentItem>() != null)
            {
                SaveObjManager.MoveItem(interactor, NextSceneName, newPos);
            }
            if (true)
            { //interactor.GetComponent<CharacterBase> ().IsCurrentPlayer) {
              //GameManager.Instance.LoadRoom (sceneName);

                Initiate.Fade(NextSceneName, Color.black, 5.0f);
            }
            Destroy(interactor);
        }
    }
}