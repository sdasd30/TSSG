using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class RHConversation : MonoBehaviour
{
    [SerializeField]
    private float m_startingInterest;
    public virtual float StartingInterest { get { return m_startingInterest; } private set { m_startingInterest = value; } }
    [SerializeField]
    private float m_threashould;
    public virtual float Threashould { get { return m_threashould; } private set { m_threashould = value; } }
    [SerializeField]
    private float m_maxValue;
    public virtual float MaxValue { get { return m_maxValue; } private set { m_maxValue = value; } }
    [SerializeField]
    private bool m_canSlow = true;
    [SerializeField]
    private RHResponseString m_introText;
    [SerializeField]
    private bool m_destroyAfterFinish = true;
    [SerializeField]
    private bool m_debugExecuteOnStartup = false;
    [SerializeField]
    private List<RHSpeaker> DefaultParticipants;
    [SerializeField]
    private List<Color> m_defaultColors;
    [SerializeField]
    private Dictionary<RHSpeaker, Color> m_speakerColorMaps ;

    private Dictionary<RHListener, float> m_listeners_with_scores;
    public Dictionary<RHListener, float> Listeners { get { return m_listeners_with_scores; } }
    private List<RHSpeaker> m_speakers;
    private List<RHListener> m_listeners;
    public List<RHSpeaker> Speakers { get { return m_speakers; } }

    private List<RHStatement> m_previousStatements = new List<RHStatement>();
    public List<RHStatement> PreviousStatements { get { return m_previousStatements; } }


    private float m_nextInterestTimeLimit;
    public virtual float InterestTimeEnd { get { return m_nextInterestTimeLimit; } private set { m_nextInterestTimeLimit = value; } }

    private bool m_isFinished = false;
    private float m_nextStatementEnd;
    private bool m_battleStarted = false;
    private GameObject m_currentDialogueBox;
    private RHUITime m_timeUI;    
    private float m_nextEventTime = float.MaxValue;
    private int m_nextSpeakerIndex = 0;
    private float m_lastScaled = 0;
    private class RegisteredStatement
    {
        public RHStatement statement;
        public RHSpeaker speaker;
        public RegisteredStatement(RHStatement statement, RHSpeaker speaker)
        {
            this.statement = statement;
            this.speaker = speaker;
        }
    }
    private Dictionary<RHSpeaker, List<RegisteredStatement>> m_statementQueue = new Dictionary<RHSpeaker, List<RegisteredStatement>>();
    private RegisteredStatement m_currentStatement;

    private class RegisteredEvent
    {
        public RHEvent m_event;
        public RHSpeaker speaker;
        public RegisteredEvent(RHEvent rhevent, RHSpeaker speaker)
        {
            this.m_event = rhevent;
            this.speaker = speaker;
        }
    }
    private Dictionary<float, List<RegisteredEvent>> m_registeredEvents = new Dictionary<float, List<RegisteredEvent>>();

    public void SetDebug(float timeLimit, float threashould, float maxValue)
    {
        m_nextInterestTimeLimit = timeLimit;
        m_threashould = threashould;
        m_maxValue = maxValue;
    }
    
    

    public List<RHStatement> GetValidStatements(RHSpeaker speaker )
    {
        return speaker.AvailableStatements;
    }
    
    public void SkipToNextStatment()
    {
        if (m_currentStatement != null)
            ScaledTime.skipTime(m_nextStatementEnd - ScaledTime.TimeElapsed);
    }
    public void QueueStatement(RHStatement statement, RHSpeaker speaker,bool stack = false)
    {
        if (!m_battleStarted)
        {
            initializeTime();
        }
        
        statement.OnStatementQueued(speaker);
        m_statementQueue[speaker].Add( new RegisteredStatement(statement,speaker));

        m_timeUI.AddItem(statement.StatementName, statement.Time, m_speakerColorMaps[speaker],false, calculateQueuedStatementExecutionTime(statement),stack);
        if (!m_battleStarted)
            AdvanceStatements();
        m_battleStarted = true;
    }

    public void QueueEffect(RHSpeaker speaker, string name, float startingTime, float duration)
    {
        m_timeUI.AddItem(name, duration, m_speakerColorMaps[speaker],true,startingTime,false);
    }
    
    public void ModifyInterestTime(float delta)
    {
        m_nextInterestTimeLimit += delta;
    }
    private void initializeTime()
    {
        
        m_nextStatementEnd = 0f;
        m_nextInterestTimeLimit = ScaledTime.TimeElapsed + m_startingInterest;
        RHManager.UITime().SetActive(true);
        m_timeUI = RHManager.UITime().GetComponent<RHUITime>();
        m_timeUI.StartUI(this);

    }
    private float calculateQueuedStatementExecutionTime(RHStatement statement)
    {
        float executionTime = m_nextStatementEnd;
        int tempIndex = m_nextSpeakerIndex;
        Dictionary<RHSpeaker, int> nextEntry = new Dictionary<RHSpeaker, int>();
        
        int emptyQueues = 0;
        while (emptyQueues < m_speakers.Count)
        {
            RHSpeaker nextSpeaker = m_speakers[tempIndex];
            tempIndex = (tempIndex + 1) % m_speakers.Count;
            List<RegisteredStatement> queue = m_statementQueue[nextSpeaker];
            if (!nextEntry.ContainsKey(nextSpeaker))
                nextEntry[nextSpeaker] = 0;
            if (queue.Count > nextEntry[nextSpeaker])
            {
                RHStatement next_statement = queue[nextEntry[nextSpeaker]].statement;
                if (next_statement == statement)
                    break;
                executionTime += next_statement.Time;
                nextEntry[nextSpeaker]++;
                emptyQueues = 0;
            } else
            {
                emptyQueues++;
            }
        }
        return executionTime;
    }
    private void registerEvents(List<RHEvent> eList, RHSpeaker speaker, float startingTime)
    {
        foreach(RHEvent e in eList)
        {
            registerEvent(e, speaker, startingTime);
        }
    }
    private void registerEvent(RHEvent e, RHSpeaker speaker, float startingTime)
    {
        float eventTime = startingTime + e.TimeOffsetFromStatementStart;
        m_nextEventTime = Mathf.Min(m_nextEventTime, eventTime);
        if (!m_registeredEvents.ContainsKey(eventTime))
            m_registeredEvents[eventTime] = new List<RegisteredEvent>();
        m_registeredEvents[eventTime].Add(new RegisteredEvent(e,speaker));
    }
    private void processEvents()
    {
        if (ScaledTime.TimeElapsed > m_nextEventTime)
        {
            foreach (RegisteredEvent e in m_registeredEvents[m_nextEventTime])
            {
                foreach(RHListener l in m_listeners)
                {
                    if (e.speaker.gameObject != l.gameObject)
                        e.m_event.ExecuteEvent(e.speaker, l);
                }
            }
            m_registeredEvents.Remove(m_nextEventTime);
            if (m_registeredEvents.Count > 0)
            {
                m_nextEventTime = Mathf.Min(m_registeredEvents.Keys.ToArray());
            } else
            {
                m_nextEventTime = float.MaxValue;
            }
            
        }
    }

    private void callNextStatement()
    {
        if (m_currentStatement != null) { 
            BroadcastStatement(m_currentStatement.speaker, m_currentStatement.statement);
            m_previousStatements.Add(m_currentStatement.statement);
        }
    }
    private void AdvanceStatements()
    {
        RegisteredStatement next_statement = null;
        int emptyQueues = 0;
        RHSpeaker nextSpeaker = null;
        while (emptyQueues < m_speakers.Count)
        {
            nextSpeaker = m_speakers[m_nextSpeakerIndex];
            m_nextSpeakerIndex = (m_nextSpeakerIndex + 1) % m_speakers.Count;
            List<RegisteredStatement> queue = m_statementQueue[nextSpeaker];
            if (queue.Count > 0)
            {
                next_statement = new RegisteredStatement(queue[0].statement,nextSpeaker);        
                m_statementQueue[nextSpeaker].RemoveAt(0);
                break;
            }
            else
            {
                emptyQueues++;
            }
        }
        if (next_statement == null) {
            QueueStatement(RHManager.DeadAirPrefab.GetComponent<RHStatement>(),m_speakers[m_nextSpeakerIndex],true);
            AdvanceStatements();
            return;
        }
        else {
            m_currentStatement = next_statement;
            registerEvents(m_currentStatement.statement.RHEvents, nextSpeaker, ScaledTime.TimeElapsed);
        }
        m_nextStatementEnd = ScaledTime.TimeElapsed + m_currentStatement.statement.Time;
    }
    private void BroadcastStatement(RHSpeaker speaker, RHStatement statement)
    {
        statement.OnStatementExecuted(speaker);
        foreach (RHListener listener in m_listeners)
        {
            if (listener.GetComponent<RHSpeaker>() == speaker)
            {
                continue;
            }
                
            float power = statement.GetPower(speaker, listener, this);
            float diff = listener.ApplyStatementModifiers(power, speaker, statement, this);
            if (diff != 0)
            {
                m_listeners_with_scores[listener] += diff;
            }
            listener.GetComponent<AITaskManager>()?.triggerEvent(new AIEVStatementReceived(this,statement,speaker));
            RHManager.AddHistoryText(statement.GetResponseString(listener, speaker, diff));
            RHManager.AddHistoryText(listener.GetResponseString(listener, speaker, diff));
        }
    }
    private int GetNumberOfOccurances(RHStatement baseStatement)
    {
        int i = 0;
        foreach( RHStatement s in m_previousStatements)
        {
            if (s.StatementName == baseStatement.StatementName)
                i++;
        }
        return i;
    }
    private void OnFinish()
    {
        foreach (RHListener listener in m_listeners)
        {
            processFinish(listener);
        }
        RHManager.UITime().SetActive(false);
        RHManager.UITime().GetComponent<RHUITime>().ClearItems();
        RHManager.ClearHistory();
        RHManager.SetHistoryTextActive(false);
        RHManager.SetResourceUIInActive();

        m_isFinished = true;
        if (m_currentDialogueBox != null)
            Destroy(m_currentDialogueBox);
        if (m_destroyAfterFinish)
            Destroy(gameObject);
    }
    private void processSlowInput()
    {
        if (Input.GetKeyDown("space"))
        {
            if (ScaledTime.GetScale() == 1.0f)
            {
                RHManager.SetSlowTextActive(true);
                ScaledTime.SetScale(0.5f);
            }
            else
            {
                RHManager.SetSlowTextActive(false);
                ScaledTime.SetScale(1.0f);
            }
        }
    }
    private float startingScore(RHListener listener) {
        return StartingInterest;
    }
    protected virtual List<RHStatement> GetAvailableStatements(RHSpeaker speaker)
    {
        return speaker.AvailableStatements;
    }
    protected virtual bool isRhetoricBattleOver()
    {
        return (ScaledTime.TimeElapsed > m_nextInterestTimeLimit);
    }

    protected virtual void processFinish(RHListener listener)
    {
        if (m_listeners_with_scores[listener] > m_threashould)
        {
            OnConversationSuccess(listener);
        } else
        {
            OnConversationFailure(listener);
        }
    }

    protected virtual void OnConversationSuccess(RHListener listener) {}

    public virtual void OnConversationFailure(RHListener listener) { }
    
    public void StartRhetoricBattle( List<RHSpeaker> participants, RHSpeaker startingSpeaker)
    { 
        this.m_listeners_with_scores = new Dictionary<RHListener, float>();
        this.m_listeners = new List<RHListener>();
        this.m_speakers = new List<RHSpeaker>();
        m_speakerColorMaps = new Dictionary<RHSpeaker, Color>();
        m_lastScaled = ScaledTime.TimeElapsed;

        if (participants.Count > 0)
        {
            int colorIndex = 0;
            foreach(RHSpeaker s in participants)
            {
                this.m_speakers.Add(s);
                this.m_listeners_with_scores.Add(s.GetComponent<RHListener>(), startingScore(s.GetComponent<RHListener>()));
                this.m_listeners.Add(s.GetComponent<RHListener>());
                m_statementQueue[s] = new List<RegisteredStatement>();
                if (s.speakerColor == Color.white)
                    m_speakerColorMaps[s] = m_defaultColors[colorIndex];
                else
                    m_speakerColorMaps[s] = s.speakerColor;
                colorIndex = (colorIndex + 1) % m_defaultColors.Count;
            }
        } else {
            foreach(RHSpeaker s in DefaultParticipants)
            {
                this.m_speakers.Add(s);
                this.m_listeners_with_scores.Add(s.GetComponent<RHListener>(), startingScore(s.GetComponent<RHListener>()));
                this.m_listeners.Add(s.GetComponent<RHListener>());
            }     
        }

        foreach (RHListener l in m_listeners)
        {
            if (l.GetComponent<RHSpeaker>() == startingSpeaker)
                continue;
            GameObject go = Instantiate(RHManager.PrefabUIListener(), RHManager.ListenersBaseTransform());
            go.GetComponent<RHUIListener>().InitializeUI(l, this, startingSpeaker, m_speakerColorMaps[l.GetComponent<RHSpeaker>()]);
            l.GetComponent<AITaskManager>()?.triggerEvent(new AIEVConversationStarted(this));
        }
        foreach (RHSpeaker sp in m_speakers)
        {
            List<RHStatement> stlist = GetAvailableStatements(sp);
            sp.OnRhetoricStart(stlist,this, this.m_listeners_with_scores);
        }
        m_previousStatements = new List<RHStatement>();
        RHManager.ClearHistory();
        RHManager.SetHistoryTextActive(true);
        RHManager.AddHistoryText(m_introText);
    }

    public void SetDialogueBox(GameObject go) {
        m_currentDialogueBox = go;
    }
    public void ModifyListenerValue(RHListener l, float delta)
    {
        if (m_listeners.Contains(l))
        {
            m_listeners_with_scores[l] += delta;
        }
        return;
    }
    void Start()
    {
        if (m_debugExecuteOnStartup)
            RHManager.StartRhetoricBattle(this, DefaultParticipants, DefaultParticipants[0]);
    }

    void Update()
    {
        if (!m_battleStarted)
            return;
        float d = ScaledTime.TimeElapsed - m_lastScaled;
        foreach (RHSpeaker s in m_speakers)
            foreach(RHListener l in m_listeners)
                if (l.GetComponent<RHSpeaker>() != s)
                    s.resourceUpdate(this,l, d);
        m_lastScaled = ScaledTime.TimeElapsed;
        if (ScaledTime.TimeElapsed >= m_nextStatementEnd)
        {
            callNextStatement();
            AdvanceStatements();
        }
        if (isRhetoricBattleOver())
            OnFinish();


        processEvents();
        if (!m_canSlow)
            return;
        processSlowInput();
    }
}    
