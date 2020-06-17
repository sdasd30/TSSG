using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RHUIStatusIcon : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Label;
    [SerializeField]
    private RectTransform m_timeBar;
    [SerializeField]
    private Image m_Image;

    private float m_nextTimeLimit = 0f;
    private float m_timeLimit = 0f;
    private bool m_timeLimitSet = false;
    public void SetTimeLimit(float time)
    {
        m_timeLimit = time;
        m_nextTimeLimit = ScaledTime.TimeElapsed + time;
        m_timeLimitSet = true;
    }
    public void SetImage(Sprite s)
    {
        m_Image.sprite = s;
    }
    public void SetLabel(string s)
    {
        m_Label.text = s;
    }

    private void UpdateTimeBar()
    {
        float timeSoFar = m_nextTimeLimit - ScaledTime.TimeElapsed;
        Vector2 d = m_timeBar.sizeDelta;
        float newF = (timeSoFar/ m_timeLimit) * (m_Image.rectTransform.sizeDelta.y);
        m_timeBar.sizeDelta = new Vector2(d.x, newF);
    }
    void Update()
    {
        if (m_timeLimitSet)
            UpdateTimeBar();
    }
}
