using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledTime : MonoBehaviour
{
    private static ScaledTime m_instance;

    public static ScaledTime Instance
    {
        get { return m_instance; }
        private set { m_instance = value; }
    }
    public static float UITimeElapsed
    {
        get { return m_instance.m_uiScaledTime; }
        private set { m_instance.m_uiScaledTime = value; }
    }
    public static float deltaTime {
        get { return (m_instance.m_paused) ? 0f : Time.deltaTime * m_instance.m_timeScale; }
    }
    public static float UIdeltaTime
    {
        get { return (m_instance.m_uiPaused) ? 0f : Time.deltaTime * m_instance.m_UITimeScale; }
    }

    public static float ActionTimeElapsed
    {
        get { return m_instance.m_actionScaledTime; }
        private set { m_instance.m_actionScaledTime = value; }
    }

    private float m_uiScaledTime = 0.0f;
    private float m_actionScaledTime = 0.0f;
    private bool m_uiPaused = false;
    private bool m_paused = false;
    private float m_UITimeScale = 1.0f;
    private float m_timeScale = 1.0f;


    public static void skipTime(float skipAmount,bool UI = false)
    {
        if (UI)
            m_instance.m_uiScaledTime += skipAmount;
        else
            m_instance.m_actionScaledTime += skipAmount;
    }
    public static void SetPause(bool isPaused, bool UI = false)
    {
        if (UI)
            m_instance.m_uiPaused = isPaused;
        else
            m_instance.m_paused = isPaused;
    }

    public static void SetScale(float scale, bool UI = false)
    {
        if (UI)
            m_instance.m_UITimeScale = scale;
        else
            m_instance.m_timeScale = scale;
    }
    public static float GetScale(bool UI = false)
    {
        if (UI)
            return m_instance.m_UITimeScale;
        else
            return m_instance.m_timeScale;
    }
    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    void Update()
    {
        if (!m_uiPaused)
            m_uiScaledTime += Time.deltaTime * m_UITimeScale;
        if (m_paused)
            m_actionScaledTime += Time.deltaTime * m_timeScale;
    }
}
