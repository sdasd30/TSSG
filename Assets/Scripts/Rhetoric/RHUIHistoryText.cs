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

    public void ClearText()
    {
        foreach (Transform child in Content)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void AddLine(string s,Color c, int fontSize)
    {
        GameObject go = Instantiate(TextPrefab, Content);
        TextMeshProUGUI t = go.GetComponent<TextMeshProUGUI>();
        t.text = s;
        t.color = c;
        t.fontSize = fontSize;
        Vector2 oldSize = Content.sizeDelta;
        Content.sizeDelta = new Vector2(oldSize.x, oldSize.y + SIZE_OF_TEXT);
        ScrollSlider.value = 0.0f;
    }
}
