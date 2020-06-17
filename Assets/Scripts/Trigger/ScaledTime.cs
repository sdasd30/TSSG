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
    public static float TimeElapsed
    {
        get { return m_instance.m_scaledTime; }
        private set { m_instance.m_scaledTime = value; }
    }

    private float m_scaledTime = 0.0f;
    private bool m_paused = false;
    private float m_timeScale = 1.0f;


    public static void skipTime(float skipAmount)
    {
        m_instance.m_scaledTime += skipAmount;
    }
    public static void SetPause(bool isPaused)
    {
        m_instance.m_paused = isPaused;
    }

    public static void SetScale(float scale)
    {
        m_instance.m_timeScale = scale;
    }
    public static float GetScale()
    {
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
        if (!m_paused)
            m_scaledTime += Time.deltaTime * m_timeScale;
    }
}
