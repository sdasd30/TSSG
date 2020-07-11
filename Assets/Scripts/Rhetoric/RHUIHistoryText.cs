using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RHUIHistoryText : MonoBehaviour
{
    [SerializeField]
    private GameObject TextPrefab;
    [SerializeField]
    private RectTransform Content;
    [SerializeField]
    private Scrollbar ScrollSlider;

    private const float SIZE_OF_TEXT = 40;
    private bool m_checkTextboxFinished = false;
    private Textbox m_textboxToCheckFinished = null;
    public void ClearText()
    {
        foreach (Transform child in Content)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void AddLine(string s,Color c, int fontSize, bool checkTextboxFinished = false)
    {
        GameObject go = Instantiate(TextPrefab, Content);
        TextMeshProUGUI t = go.GetComponentInChildren<TextMeshProUGUI>();
        Textbox tbox = go.GetComponent<Textbox>();
        tbox.setText(s);
        t.color = c;
        t.alpha = 1.0f;
        t.fontSize = fontSize;
        Vector2 oldSize = Content.sizeDelta;
        Content.sizeDelta = new Vector2(oldSize.x, oldSize.y + SIZE_OF_TEXT);
        ScrollSlider.value = 0.0f;
        if (checkTextboxFinished)
        {
            RHManager.SetPause(true);
            m_checkTextboxFinished = true;
            m_textboxToCheckFinished = tbox;
        }
    }

    private void Update()
    {
        if (m_checkTextboxFinished)
        {
            if (m_textboxToCheckFinished == null || m_textboxToCheckFinished.IsFinished())
                RHManager.SetPause(false);
        }
    }
}
