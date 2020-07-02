using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;


public class UIHoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float HoverTriggerTime = 0.25f;
    [SerializeField]
    private string HoverText = "";


    private bool m_isHovering = false;
    private float m_timeUntilDraw = 0f;
    private GameObject m_hoverBox;

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_isHovering = true;
        m_timeUntilDraw = Time.timeSinceLevelLoad + HoverTriggerTime;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_isHovering = false;
        if (m_hoverBox != null)
            Destroy(m_hoverBox);
    }

    public void SetText(string text)
    {
        HoverText = text;
        if (m_hoverBox != null)
        {
            
            m_hoverBox.GetComponent<Textbox>().setText(HoverText);
        }
    }

    private void createBox()
    {
        m_hoverBox = Instantiate(TextboxManager.Instance.hoverTextPrefab,transform.GetComponentInParent<Canvas>().transform);
        m_hoverBox.GetComponent<Textbox>().setText(HoverText);
        m_hoverBox.GetComponent<Textbox>().SetTypeMode(false );
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isHovering && Time.timeSinceLevelLoad > m_timeUntilDraw && m_hoverBox == null)
        {
            createBox();
        }
        if (m_hoverBox != null)
        {
            var screenPoint = Input.mousePosition;
            Vector2 tbSize = m_hoverBox.GetComponent<RectTransform>().sizeDelta;
            //screenPoint.z = 10.0f; //distance of the plane from the camera
            float x = screenPoint.x + 256;
            float y = screenPoint.y - 80;
            
            if (y + tbSize.y > Screen.height)
                y = y - tbSize.y;
            if (x + tbSize.x - 64 > Screen.width)
                x = x - tbSize.x;
            m_hoverBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }
}
