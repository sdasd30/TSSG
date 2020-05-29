using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHConversation : MonoBehaviour
{
    public bool ExecuteOnStartup = false;
    public RHSpeaker DefaultSpeaker;
    public RHListener DefaultListener;

    public float TimeLimit;
    public float Threashould;
    public float MaxValue;
    private float m_nextTimeLimit;
    private float m_nextConversationEnd;
    private bool m_battleStarted = false;
    private List<RHSpeaker> m_speakers;
    private List<RHStatement> m_statementQueue = new List<RHStatement>();
    private RHStatement m_currentStatement;
    private Dictionary<RHListener, float> m_listeners;

    // Start is called before the first frame update
    void Start()
    {
        if (ExecuteOnStartup)
            RHManager.StartRhetoricBattle(this, DefaultSpeaker, DefaultListener);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_battleStarted)
        {
            AdvanceStatementIfNeeded();
            if (isRhetoricBattleOver())
                OnFinish();
        }
    }

    public List<RHStatement> GetValidStatements(RHSpeaker speaker )
    {
        return speaker.AvailableStatements;
    }
    
    public void SkipToNextStatment()
    {
        if (m_currentStatement != null)
            ScaledTime.skipTime(m_nextConversationEnd - ScaledTime.TimeElapsed);
    }
    public void QueueStatement(RHStatement statement)
    {
        if (!m_battleStarted)
        {
            m_battleStarted = true;
            m_nextConversationEnd = 0f;
            m_nextTimeLimit = ScaledTime.TimeElapsed + TimeLimit;
        }
        m_statementQueue.Add(statement);
    }
    private void AdvanceStatementIfNeeded()
    {
        if (ScaledTime.TimeElapsed < m_nextConversationEnd)
            return;
        if (m_statementQueue.Count == 0)
        {
            m_currentStatement = null;
            return;
        }
        m_currentStatement = m_statementQueue[0];
        m_nextConversationEnd = ScaledTime.TimeElapsed + m_currentStatement.Time;
        BroadcastStatement(m_speakers[0],m_currentStatement);
        m_statementQueue.RemoveAt(0);
    }
    private void BroadcastStatement(RHSpeaker speaker, RHStatement statement)
    {
        foreach (RHListener listener in m_listeners.Keys)
        {
            float power = statement.GetBasePower(speaker,listener);
            m_listeners[listener] += listener.ApplyStatementModifiers(power, listener, statement);
        }
    }
    private void OnFinish()
    {
        foreach (RHListener listener in m_listeners.Keys)
        {
            processFinish(listener);
        }
    }

    private float startingScore(RHListener listener)
    {
        return 0.0f;
    }
    protected virtual List<RHStatement> GetAvailableStatements(RHSpeaker speaker)
    {
        return speaker.AvailableStatements;
    }
    protected virtual bool isRhetoricBattleOver()
    {
        return (ScaledTime.TimeElapsed > m_nextTimeLimit);
    }

    protected virtual void processFinish(RHListener listener)
    {
        if (m_listeners[listener] > Threashould)
        {
            OnConversationSuccess(listener);
        } else
        {
            OnConversationFailure(listener);
        }
    }

    protected virtual void OnConversationSuccess(RHListener listener)
    {

    }

    public virtual void OnConversationFailure(RHListener listener)
    {

    }

    public void StartRhetoricBattle(List<RHSpeaker> speakers, List<RHListener> listeners)
    {
        if (speakers.Count > 0)
            this.m_speakers = speakers;
        else
        {
            this.m_speakers = new List<RHSpeaker>();
            this.m_speakers.Add(DefaultSpeaker);
        }
        if (listeners.Count > 0)
        {
            this.m_listeners = new Dictionary<RHListener, float>();
            this.m_listeners.Add(DefaultListener, startingScore(DefaultListener));
            foreach (RHListener l in listeners)
            {
                this.m_listeners.Add(l, startingScore(l));
            }
        }            
        else
        {
            this.m_listeners = new Dictionary<RHListener, float>();
            this.m_listeners.Add(DefaultListener, startingScore(DefaultListener));
        }
        foreach(RHSpeaker sp in m_speakers)
        {
            List<RHStatement> stlist = GetAvailableStatements(sp);
            sp.StartStatementSelection(stlist,this);
        }
    }
}
