using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RHUITimePiece : MonoBehaviour
{
    public float TimeItemStarts { get { return m_timeItemStarts; } }

    private float m_timeItemStarts;
    [SerializeField]
    private Button m_button;
    [SerializeField]
    private TextMeshProUGUI m_text;

    private const float DARKEN_FACTOR = 0.5f;

    private RHRegisteredStatement m_statement;
    public RHRegisteredStatement Statement { get { return m_statement; }  }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_button.interactable = ScaledTime.UITimeElapsed < m_timeItemStarts;
    }

    public void SetPiecePiece(RHRegisteredStatement rs, float startingTime, Color backgroundColor, bool interactable, float scaling_factor = 32)
    {

        string name = rs.statement.StatementName;
        float duration = rs.statement.Time;

        GetComponent<Image>().color = new Color(backgroundColor.r * DARKEN_FACTOR, backgroundColor.g * DARKEN_FACTOR, backgroundColor.b * DARKEN_FACTOR, 0.75f);
        GetComponent<RectTransform>().sizeDelta = new Vector2(duration * scaling_factor, 40f);

        m_text.text = name;
        m_statement = rs;
        m_timeItemStarts = startingTime;
        m_button.interactable = interactable;

    }
    public void ClearTimePiece()
    {
        m_statement.conversation.DequeueStatement(m_statement);
    }

    public void shiftTime(float shiftAmount)
    {
        m_timeItemStarts += shiftAmount;
    }
    
}
