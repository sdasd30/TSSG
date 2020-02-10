using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{

    public float duration = 3.0f;
    public bool toDisappear = true;

    private bool m_alreadyBeingDestroyed = false;
    // Use this for initialization
    void Start()
    {
        if (toDisappear)
        {
            GameObject.Destroy(gameObject, duration);
            m_alreadyBeingDestroyed = true;
        }
    }

    private void Update()
    {
        if (!m_alreadyBeingDestroyed && toDisappear)
        {
            m_alreadyBeingDestroyed = true;
            GameObject.Destroy(gameObject, duration);
        }
    }

    private void OnDestroy()
    {
        if (GetComponent<DeathSound>() != null)
            GetComponent<DeathSound>().playSound();
    }
}
