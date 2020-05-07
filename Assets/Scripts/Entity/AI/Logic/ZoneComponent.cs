using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneComponent : MonoBehaviour
{
    Zone MasterZone;

    void Start()
    {
        if (GetComponent<BoxCollider>() != null)
            GetComponent<BoxCollider>().isTrigger = true;
        if (transform.parent != null && transform.parent.GetComponent<Zone>())
            MasterZone = transform.parent.GetComponent<Zone>();
    }

    internal void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<AITaskManager>() != null)
        {
            MasterZone.OnAddChar(other.GetComponent<AITaskManager>());
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        if (MasterZone.OverlapCharacters.Contains(other.gameObject.GetComponent<AITaskManager>()))
        {
            MasterZone.OnRemoveChar(other.GetComponent<AITaskManager>());
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, .15f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
}
