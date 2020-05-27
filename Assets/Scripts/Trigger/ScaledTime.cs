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

    public static void skipTime(float skipAmount)
    {
        m_instance.m_scaledTime += skipAmount;
    }
    public static void SetPause(bool isPaused)
    {
        m_instance.m_paused = isPaused;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (!m_paused)
            m_scaledTime += Time.deltaTime;
    }
}
