using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    protected float m_remainingDuration = 1.0f;

    public CharacterBase MasterInteractor;
    public List<Interactable> OverlapInteractions;
    [SerializeField]
    private float m_duration = 1.0f;
    public float Duration { get { return m_duration; } set { m_duration = value; } }

    // Start is called before the first frame update
    void Start()
    {
        m_remainingDuration = m_duration;
    }

    // Update is called once per frame
    void Update() {

        if (OverlapInteractions.Count > 0)
            OnAttemptInteract();
        MaintainOrDestroyHitbox();
    }
    private void MaintainOrDestroyHitbox()
    {
        if (m_remainingDuration <= 0.0f)
        {
            //Debug.Log ("Hitbox destroyed!" + m_duration);
            GameObject.Destroy(gameObject);
        }
        m_remainingDuration -= Time.deltaTime;
    }
    internal void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger enter other INteractor: ");
        //Debug.Log(other.gameObject.GetComponent<Interactable>());
        if (other.gameObject.GetComponent<Interactable>() != null && other.gameObject != MasterInteractor.gameObject)
        {
            //Debug.Log("Adding an INteractable");
            OverlapInteractions.Add(other.GetComponent<Interactable>());
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        if (OverlapInteractions.Contains(other.gameObject.GetComponent<Interactable>()))
        {
            OverlapInteractions.Remove(other.GetComponent<Interactable>());
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, .05f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
    public void OnAttemptInteract()
    {
        float minDistance = 4000;
        int maxPriority = -1;
        Interactable bestInteractable = null;
        foreach (Interactable i in OverlapInteractions)
        {
            if (i == null)
                continue;
            if (i.interactableObjectInfo.Priority > maxPriority ||
                (i.interactableObjectInfo.Priority == maxPriority &&
                Vector3.Distance(i.gameObject.transform.position, transform.position) < minDistance))
            {
                minDistance = Vector3.Distance(i.gameObject.transform.position, transform.position);
                maxPriority = i.interactableObjectInfo.Priority;
                bestInteractable = i;
            }
        }
        if (bestInteractable != null)
        {
            bestInteractable.onPress(MasterInteractor.gameObject);
        }
        OverlapInteractions.Clear();
    }

    public void OnAttemptInteract(Interactable i, bool force = false)
    {
        if (force)
            i.onPress(gameObject);
        else if (OverlapInteractions.Contains(i))
            i.onPress(gameObject);
    }

}
