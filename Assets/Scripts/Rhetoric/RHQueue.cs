using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHQueue 
{

    private int m_nextSpeakerIndex = 0;
    private float m_nextStatementEnd;
    private Dictionary<RHSpeaker, List<RHRegisteredStatement>> m_statementQueue = new Dictionary<RHSpeaker, List<RHRegisteredStatement>>();
    private List<RHSpeaker> m_speakers = new List<RHSpeaker>();

    public RHQueue()
    {

    }
    public void RegisterSpeaker(RHSpeaker s)
    {
        m_statementQueue[s] = new List<RHRegisteredStatement>();
        m_speakers.Add(s);
    }
    public void RegisterStatement(RHRegisteredStatement rs)
    {
        m_statementQueue[rs.speaker].Add(rs);

    }
    public void RemoveStatement(RHRegisteredStatement registeredStatmenet)
    {
        int removeIndex = 0;
        bool found = false;
        foreach (RHRegisteredStatement rs in m_statementQueue[registeredStatmenet.speaker])
        {
            if (rs.statement == registeredStatmenet.statement)
            {
                found = true;
                break;
            }
            removeIndex++;
        }
        if (found)
            m_statementQueue[registeredStatmenet.speaker].RemoveAt(removeIndex);
    }
    public RHRegisteredStatement PopStatement()
    {
        RHRegisteredStatement next_statement = null;
        int emptyQueues = 0;
        while (emptyQueues < m_statementQueue.Keys.Count)
        {
            RHSpeaker nextSpeaker = m_speakers[m_nextSpeakerIndex];
            m_nextSpeakerIndex = (m_nextSpeakerIndex + 1) % m_speakers.Count;
            List<RHRegisteredStatement> queue = m_statementQueue[nextSpeaker];
            if (queue.Count > 0)
            {
                next_statement = m_statementQueue[nextSpeaker][0];
                m_statementQueue[nextSpeaker].RemoveAt(0);
                break;
            }
            else
            {
                emptyQueues++;
            }
        }
        return next_statement;
    }
    public float CalculateQueuedStatementExecutionTime(RHRegisteredStatement RHRegisteredStatement, float m_nextStatementEnd)
    {
        float executionTime = m_nextStatementEnd;
        int tempIndex = m_nextSpeakerIndex;
        Dictionary<RHSpeaker, int> nextEntry = new Dictionary<RHSpeaker, int>();

        int emptyQueues = 0;
        while (emptyQueues < m_speakers.Count)
        {
            RHSpeaker nextSpeaker = m_speakers[tempIndex];
            tempIndex = (tempIndex + 1) % m_speakers.Count;
            List<RHRegisteredStatement> queue = m_statementQueue[nextSpeaker];
            if (!nextEntry.ContainsKey(nextSpeaker))
                nextEntry[nextSpeaker] = 0;
            if (queue.Count > nextEntry[nextSpeaker])
            {
                RHRegisteredStatement next_statement = queue[nextEntry[nextSpeaker]];
                if (next_statement == RHRegisteredStatement)
                    break;
                executionTime += next_statement.statement.Time;
                nextEntry[nextSpeaker]++;
                emptyQueues = 0;
            }
            else
            {
                emptyQueues++;
            }
        }
        return executionTime;
    }
}
