using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RHUITime : MonoBehaviour
{
    public Text TimeDisplay;
    public GameObject Scroller;
    public GameObject ActiveTransform;
    public GameObject PassiveScroller;
    public GameObject TimeElementActivePrefab;
    public GameObject TimeElementPrefab;

    private RHConversation conversation;
    private const float BLOCKING_WIDTH = 2.0f;
    private float startingTime;
    private const float SCALING_FACTOR = 32f;
    private float m_nextStartingTime = 0;
    private const string PROMPT = "INTEREST";
    private string m_lastStatementName = "";
    private GameObject m_lastTimePiece;

    private const float DARKEN_FACTOR = 0.5f;

    // Update is called once per frame
    void Update()
    {
        float adjustedCurrentX = adjustedTimeXpos(ScaledTime.TimeElapsed);
        Vector2 pos = Scroller.GetComponent<RectTransform>().anchoredPosition;
        Scroller.GetComponent<RectTransform>().anchoredPosition = new Vector2(-adjustedCurrentX, pos.y);
        float timeLeft = conversation.InterestTimeEnd - ScaledTime.TimeElapsed;
        TimeDisplay.text = PROMPT + ": " +  timeLeft + " sec";
    }

    public void StartUI(RHConversation c)
    {
        conversation = c;
        startingTime = ScaledTime.TimeElapsed;
    }
    public void AddItem(string name,  float duration, Color backgroundColor, bool passive = false, float timeItemStarts = 0f, bool canStack = false )
    {
        if (canStack && m_lastStatementName == name)
        {
            ExpandLastStatement(duration);
            return;
        }
        GameObject newObj;
        if (passive) {
            newObj = Instantiate(TimeElementPrefab, PassiveScroller.transform);
            newObj.GetComponent<Image>().color = new Color(backgroundColor.r * DARKEN_FACTOR,backgroundColor.g * DARKEN_FACTOR, backgroundColor.b * DARKEN_FACTOR, 0.75f);

            newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(duration * SCALING_FACTOR, 64f);
            
        } else {
            newObj = Instantiate(TimeElementActivePrefab, ActiveTransform.transform);
            newObj.GetComponent<Image>().color = new Color(backgroundColor.r * DARKEN_FACTOR, backgroundColor.g * DARKEN_FACTOR, backgroundColor.b * DARKEN_FACTOR, 0.75f);
            newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(duration * SCALING_FACTOR, 40f);
            
        }

        if (timeItemStarts == 0)
        {
            newObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_nextStartingTime, 0f);
            m_nextStartingTime += (+duration) * SCALING_FACTOR;
        } else
        {
            float t = (timeItemStarts - startingTime) * SCALING_FACTOR;
            newObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(t , 0f);
            pushBackStatements(timeItemStarts, duration);
        }
        Vector2 v = newObj.GetComponent<RectTransform>().sizeDelta;
        newObj.transform.Find("Label").gameObject.GetComponent<Text>().text = name;
        m_lastStatementName = name;
        m_lastTimePiece = newObj;
    }

    public void ClearItems()
    {
        foreach (Transform child in Scroller.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    private void pushBackStatements(float timeItemStarts, float duration)
    {
        float startingX = (timeItemStarts - startingTime) * SCALING_FACTOR;
        float durationOffset = duration * SCALING_FACTOR;
        foreach(RectTransform t in ActiveTransform.transform)
        {
            if (t.anchoredPosition.x > startingX)
            {
                Vector2 v = t.anchoredPosition;
                t.anchoredPosition = new Vector2(t.anchoredPosition.x + durationOffset, t.anchoredPosition.y);
            }
        }
    }
    private void ExpandLastStatement(float duration)
    {
        Vector2 lastScale = m_lastTimePiece.GetComponent<RectTransform>().sizeDelta;
        m_lastTimePiece.GetComponent<RectTransform>().sizeDelta = new Vector2(lastScale.x + duration * SCALING_FACTOR, lastScale.y);
        m_nextStartingTime += (duration) * SCALING_FACTOR;
    }
    private float adjustedTimeXpos(float time)
    {
        return (time - startingTime) * SCALING_FACTOR;
    }
}
