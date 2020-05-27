using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHConversation : MonoBehaviour
{
    public float TimeLimit;
    private float m_nextTimeLimit;
    private List<RHSpeaker> speakers;
    private List<RHListener> listeners;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<RHStatement> GetValidStatements(RHSpeaker speaker )
    {
        return speaker.AvailableStatements;
    }
    

    public void ProcesssStatement(RHSpeaker speaker, RHStatement statement)
    {
        foreach (RHListener listener in listeners)
        {
            float finalResult = statement.GetBasePower(speaker,listener);
            listener.ProcessStatementValue(finalResult, listener, statement);
        }
        
    }

    public virtual bool IsOver()
    {
        return (ScaledTime.TimeElapsed > m_nextTimeLimit);
    }

    public virtual void processFinish()
    {

    }

    public virtual void OnConversationSuccess(RHListener listener)
    {

    }

    public virtual void OnConversationFailure(RHListener listener)
    {

    }
}
