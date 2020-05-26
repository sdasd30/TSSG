using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public bool ReverseSwingDirection;
    private Transform m_doorTransform;

    public bool m_isOpened;
    public float z;
    public bool m_isMoving;
    public float m_doorSwingSpeed = 1f;
    public float m_targetZOpen;
    public float m_targetZClosed;
    private Collider m_collider;

    // Start is called before the first frame update
    void Start()
    {
        m_targetZClosed = 0f;
        m_targetZOpen = (ReverseSwingDirection) ? 270f : 90f;
        m_collider = GetComponent<Collider>();
        m_doorTransform = transform.GetChild(0).transform;
        z = m_doorTransform.transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMoving)
            moveDoor();
    }
    public override void onPress(GameObject interactor)
    {
        base.onPress(interactor);
        if (m_isMoving)
            return;
        if (!m_isOpened)
        {
            createDoorHitbox();
        }
        m_isMoving = true;
        m_isOpened = !m_isOpened;
        m_collider.isTrigger = m_isOpened;
    }

    private void moveDoor()
    {
        if (m_isOpened )
        {
            if (Mathf.Abs(z - m_targetZOpen) < m_doorSwingSpeed)
            {
                z = m_targetZOpen;
                m_isMoving = false;
            } else
            {
                float dir = (ReverseSwingDirection) ? -1 : 1;
                z += m_doorSwingSpeed * dir;
            }
        } else
        {
            if (Mathf.Abs(z - m_targetZClosed) < m_doorSwingSpeed)
            {
                z = m_targetZClosed;
                m_isMoving = false;
            }
            else
            {
                float dir = (ReverseSwingDirection) ? 1 : -1;
                z += m_doorSwingSpeed * dir;
            }
        }
        Debug.Log(m_doorTransform.rotation);
        m_doorTransform.localRotation = Quaternion.Euler(new Vector3(0, z, 0f));
        Debug.Log(m_doorTransform.rotation);
    }

    private void createDoorHitbox()
    {

    }
}
