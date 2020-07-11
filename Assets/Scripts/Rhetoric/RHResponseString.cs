using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RHResponseString
{
    public RHResponseString(string textValue)
    {
        this.textValue = textValue;
        this.fontSize = 12;
        this.fontColor = Color.white;
    }
    public RHResponseString(string textValue, int fontSize)
    {
        this.textValue = textValue;
        this.fontSize = fontSize;
        this.fontColor = Color.white;
    }
    public RHResponseString(string textValue, int fontSize, Color c)
    {
        this.textValue = textValue;
        this.fontSize = fontSize;
        this.fontColor = c;
    }
    public string textValue;
    public int fontSize;
    public Color fontColor;
    public bool isPausing = false;
    public float m_refreshTime = 60f;

    private string m_statementName = "";
    private string m_previousStatement = "";
    private float m_powerRequirement = -999;
    private RHSpeaker m_speaker;
    private RHListener m_listener;
    private bool m_isPlayerSpeaker;
    private bool m_isPlayerListener;
    

    public void SetConditions(string statementName, string listener = "", string speaker = "", string previousStatement = "", float effectiveness = -999)
    {
        m_statementName = statementName;
        m_previousStatement = previousStatement;
        m_powerRequirement = effectiveness;
        if (listener.ToLower() == "player")
            m_isPlayerListener = true;
        else if (listener.Length > 0)
        {
            GameObject go = GameObject.Find(listener);
            if (go != null)
            {
                m_listener = go.GetComponent<RHListener>();
            }
        }
        if (speaker.ToLower() == "player")
            m_isPlayerSpeaker = true;
        else if (speaker.Length > 0)
        {
            GameObject go = GameObject.Find(speaker);
            if (go != null)
            {
                m_speaker = go.GetComponent<RHSpeaker>();
            }
        }

    }

    public bool IsConditionTrue(RHStatement st, List<RHStatement> previousStatements, RHSpeaker s, RHListener l, float diff, RHSpeaker playerSpeaker)
    {
        if (m_statementName.Length > 0 && st.StatementName.ToLower() != m_statementName.ToLower())
            return false;
        RHStatement last = previousStatements[previousStatements.Count - 1];
        if (m_previousStatement.Length > 0 && last.StatementName.ToLower() != m_previousStatement.ToLower())
            return false;

        if (m_isPlayerListener && l.GetComponent<RHSpeaker>() != playerSpeaker)
            return false;
        else if (m_speaker != null && m_speaker != s)
            return false;

        if (m_isPlayerSpeaker && s != playerSpeaker)
            return false;
        else if (m_listener != null && m_listener != l)
            return false;

        if (diff < m_powerRequirement)
            return false;
        return true;
    }
}