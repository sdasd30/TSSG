using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{

    public bool isPaused = false;
    public GameObject PauseMenu;
    public Text PauseText;
    private float m_timeSincePauseAction = 0.0f;
    private bool m_inSequence = false;
    private bool CanChangePause = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") && !m_inSequence)
        {
            togglePause();
        }
        if (m_inSequence)
        {
            if (isPaused)
                PauseSequence();
            else
                UnPauseSequence();

        }
    }

    void PauseSequence()
    {
        Time.timeScale = 0f;//Mathf.Max(0f, 1f - m_timeSincePauseAction);
        m_timeSincePauseAction += Time.unscaledDeltaTime;
        if (PauseText != null)
            PauseText.color = new Color(1f, 1f, 1f, Mathf.Max(0f, Mathf.Min(1f, m_timeSincePauseAction)));
        if (m_timeSincePauseAction > 1f)
        {
            if (PauseMenu != null)
                PauseMenu.SetActive(true);
            m_inSequence = false;
        }
    }
    void UnPauseSequence()
    {
        Time.timeScale = Mathf.Min(1f, (m_timeSincePauseAction /2f) + 0.5f);
        m_timeSincePauseAction += Time.unscaledDeltaTime;
        if (PauseText != null)
            PauseText.color = new Color(1f, 1f, 1f, Mathf.Max(0f, Mathf.Max(0f,1f - m_timeSincePauseAction)));
        if (PauseMenu != null)
            PauseMenu.SetActive(false);
        if (m_timeSincePauseAction > 1f)
        {
            m_inSequence = false;
        }
    }
    public void togglePause()
    {
        //if (FindObjectOfType<GameManager>().StartedGameOver)
        //    return;
        if (CanChangePause)
            setPause(!isPaused);
    }
    private void setPause(bool pause)
    {
        isPaused = pause;
        m_inSequence = true;
        m_timeSincePauseAction = 0f;
    }

    public void SetCanChangePause(bool canChange)
    {
        CanChangePause = canChange;
    }
}
