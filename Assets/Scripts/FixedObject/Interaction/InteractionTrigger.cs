using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    public Interactor MasterInteractor;

    internal void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>() != null && other.gameObject != MasterInteractor.gameObject)
        {
            MasterInteractor.AddInteract(other.GetComponent<Interactable>());
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        MasterInteractor.RemoveInteract(other.GetComponent<Interactable>());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, .05f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }

}
