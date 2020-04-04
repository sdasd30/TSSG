using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Luminosity.IO;

//[RequireComponent(typeof(BasicMovement))]
public class Interactor : MonoBehaviour
{
    [SerializeField]
    private Vector3 HitboxScale;
    [SerializeField]
    private Vector3 HitboxOffset;
    [SerializeField]
    private List<Interactable> m_overlapInteractions;
    public List<Interactable> OverlapInteractions { get { return m_overlapInteractions; } private set { m_overlapInteractions = value; } }
    private InteractionTrigger m_interactionHitbox;
    private Orientation m_orient;

    void Start()
    {
        OverlapInteractions = new List<Interactable>();
        m_interactionHitbox = Instantiate(ListHitboxes.Instance.InteractBox,transform).GetComponent<InteractionTrigger>();
        m_interactionHitbox.transform.parent = transform;
        m_interactionHitbox.MasterInteractor = this;
        m_orient = GetComponent<Orientation>();

        Vector3 newPos = transform.position + (Vector3)HitboxOffset;
        //Debug.Log("Instantiated at: " + newPos);
        var go = GameObject.Instantiate(ListHitboxes.Instance.InteractBox, gameObject.transform);
        go.transform.localScale = HitboxScale;
        go.transform.localPosition = HitboxOffset;
        go.GetComponent<InteractionTrigger>().MasterInteractor = this;
    }

    public void OnAttemptInteract()
    {
        float minDistance = 4000;
        int maxPriority = -1;
        Interactable bestInteractable = null;
        foreach(Interactable i in OverlapInteractions)
        {
            if (i == null)
                continue;
            if (i.interactableObjectInfo.Priority > maxPriority || 
                ( i.interactableObjectInfo.Priority == maxPriority && 
                Vector3.Distance(i.gameObject.transform.position,transform.position) < minDistance))
            {
                minDistance = Vector3.Distance(i.gameObject.transform.position, transform.position);
                maxPriority = i.interactableObjectInfo.Priority;
                bestInteractable = i;
            }
        }
        if (bestInteractable != null)
        {
            bestInteractable.onPress(gameObject);
        }
    }

    public void OnAttemptInteract(Interactable i, bool force = false)
    {
        if (force)
            i.onPress(gameObject);
        else if (OverlapInteractions.Contains(i))
            i.onPress(gameObject);
    }

    public void AddInteract(Interactable i)
    {
        OverlapInteractions.Add(i);
    }
    public void RemoveInteract(Interactable i)
    {
        if (OverlapInteractions.Contains(i))
            OverlapInteractions.Remove(i);
    }
}
