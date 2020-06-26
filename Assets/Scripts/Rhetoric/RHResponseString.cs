using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
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

    private string m_statementName = "";
    private float m_powerRequirement = -999;
    private RHSpeaker m_speaker;
    private RHListener m_listener;
    public void SetConditions(string statementName, RHListener l = null, RHSpeaker s = null,  float diff = -999)
    {
        m_statementName = statementName;
        m_powerRequirement = diff;
        m_speaker = s;
        m_listener = l;
    }

    public bool IsConditionTrue(RHStatement st, RHSpeaker s, RHListener l, float diff)
    {
        if (m_statementName.Length > 0 && st.gameObject.name != m_statementName)
            return false;
        if (m_speaker != null && m_speaker != s)
            return false;
        if (m_listener != null && m_listener != l)
            return false;
        if (diff < m_powerRequirement)
            return false;
        return true;
    }
}