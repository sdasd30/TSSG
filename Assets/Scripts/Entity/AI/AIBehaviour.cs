using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIBehaviour
{
    public string BehaviourName;
    public string PrefabName;
    public GameObject BehaviourPrefab;
    public Goal ParentGoal;
    public float PriorityScore;
    public AIBehaviour(string name, GameObject prefab, Goal parentGoal, float score = 1.0f)
    {
        BehaviourName = name;
        BehaviourPrefab = prefab;
        ParentGoal = parentGoal;
        PriorityScore = score;
    }
}