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

    // Update is called once per frame
    void Update()
    {
        float adjustedCurrentX = adjustedTimeXpos(ScaledTime.UITimeElapsed);
        Vector2 pos = Scroller.GetComponent<RectTransform>().anchoredPosition;
        Scroller.GetComponent<RectTransform>().anchoredPosition = new Vector2(-adjustedCurrentX, pos.y);
        float timeLeft = conversation.InterestTimeEnd - ScaledTime.UITimeElapsed;
        TimeDisplay.text = PROMPT + ": " +  timeLeft.ToString("F2") + " sec";
    }

    public void StartUI(RHConversation c)
    {
        conversation = c;
        startingTime = ScaledTime.UITimeElapsed;
    }
    public void AddItem(RHRegisteredStatement rs, Color backgroundColor,  float timeItemStarts, bool canStack = false, bool clickable = false )
    {
        float duration = rs.statement.Time;
        string name = rs.statement.StatementName;
        if (canStack && m_lastStatementName == name)
        {
            ExpandLastStatement(duration);
            return;
        }

        GameObject newObj = Instantiate(TimeElementActivePrefab, ActiveTransform.transform);
        
        moveFollowingStatements(timeItemStarts, duration);
        float t = (timeItemStarts - startingTime) * SCALING_FACTOR;
        newObj.GetComponent<RHUITimePiece>().SetPiecePiece(rs, timeItemStarts, backgroundColor, clickable, SCALING_FACTOR);
        newObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(t, 0f);
        
        m_lastStatementName = name;
        m_lastTimePiece = newObj;
    }
    public void RemoveItem(RHRegisteredStatement rs)
    {
        foreach (Transform t in ActiveTransform.transform)
        {
            RHUITimePiece piece = t.gameObject.GetComponent<RHUITimePiece>();
            if (piece.Statement == rs)
            {
                moveFollowingStatements(piece.TimeItemStarts, piece.Statement.statement.Time);
                Destroy(t.gameObject);
                break;
            }
        }
    }
    public void ClearItems()
    {
        foreach (Transform child in Scroller.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    private void moveFollowingStatements(float timeItemStarts, float offset)
    {
        float startingX = (timeItemStarts - startingTime) * SCALING_FACTOR;
        float durationOffset = offset * SCALING_FACTOR;
        foreach(RectTransform t in ActiveTransform.transform)
        {
            if (t.anchoredPosition.x + 10f >= startingX)
            {
                Vector2 v = t.anchoredPosition;
                t.anchoredPosition = new Vector2(t.anchoredPosition.x + durationOffset, t.anchoredPosition.y);
                t.gameObject.GetComponent<RHUITimePiece>().shiftTime(offset);
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

//if (passive) {
//            newObj = Instantiate(TimeElementPrefab, PassiveScroller.transform);
//newObj.GetComponent<Image>().color = new Color(backgroundColor.r* DARKEN_FACTOR, backgroundColor.g* DARKEN_FACTOR, backgroundColor.b* DARKEN_FACTOR, 0.75f);

//newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(duration* SCALING_FACTOR, 64f);
            
//        }