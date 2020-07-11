using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    private float m_statRetainAmount = 0.2f;
    [SerializeField]
    private List<RHStatement> m_permittedStatements = new List<RHStatement>();
    [SerializeField]
    private List<RHStatement> m_bannedStatements = new List<RHStatement>();

    [SerializeField]
    private List<RHFinishEffect> m_winEffects = new List<RHFinishEffect>();
    [SerializeField]
    private List<RHFinishEffect> m_failEffects = new List<RHFinishEffect>();

    [SerializeField]
    private string m_responsesExcelPath = "";
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
    private Dictionary<RHSpeaker, Color> m_speakerColorMaps;

    private Dictionary<RHListener, float> m_listeners_with_scores;
    public Dictionary<RHListener, float> Listeners { get { return m_listeners_with_scores; } }
    private List<RHSpeaker> m_speakers;
    private RHSpeaker m_startingSpeaker;
    private List<RHListener> m_listeners;
    public List<RHSpeaker> Speakers { get { return m_speakers; } }

    
    private List<RHResponseString> m_registeredResponses = new List<RHResponseString>();
    private List<RHStatement> m_previousStatements = new List<RHStatement>();
    public List<RHStatement> PreviousStatements { get { return m_previousStatements; } }


    private float m_nextInterestTimeLimit;
    public virtual float InterestTimeEnd { get { return m_nextInterestTimeLimit; } private set { m_nextInterestTimeLimit = value; } }

    private bool m_isFinished = false;
    public bool IsFinished { get { return m_isFinished; } }
    private float m_nextStatementEnd;
    private bool m_battleStarted = false;
    private GameObject m_currentDialogueBox;
    private RHUITime m_timeUI;    
    private float m_nextEventTime = float.MaxValue;
    private float m_lastScaled = 0;

    private RHQueue m_queue;
    private RHRegisteredStatement m_currentStatement;

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

    public string MeetsRequirements(RHStatement statement, RHSpeaker speaker)
    {
        if (m_permittedStatements.Count > 0)
        {
            bool found = false;
            foreach (RHStatement s in m_bannedStatements)
                if (s.StatementName == statement.StatementName)
                    found = true;
            if (!found)
                return "Not permitted for this conversation";
        }
        if (m_bannedStatements.Count > 0)
        {
            foreach (RHStatement s in m_bannedStatements)
                if (s.StatementName == statement.StatementName)
                    return "Not permitted for this conversation";
        }

        return statement.MeetsRequirements(speaker,this);
    }
    public void SetInitialValues(float timeLimit, float maxPersuasion, float persuasionRequirement)
    {
        m_startingInterest = timeLimit;
        m_maxValue = maxPersuasion;
        m_threashould = persuasionRequirement;
    }
    public void StartRhetoricBattle(List<RHSpeaker> participants, RHSpeaker startingSpeaker)
    {
        if (participants.Count == 0)
        {
            Debug.LogError("Conversation attempted with 0 participants");
            return;
        }

        ScaledTime.SetPause(true, true);
        m_nextStatementEnd = ScaledTime.UITimeElapsed;
        m_nextInterestTimeLimit = ScaledTime.UITimeElapsed + m_startingInterest;
        RHManager.UITime().SetActive(true);
        m_timeUI = RHManager.UITime().GetComponent<RHUITime>();
        m_timeUI.StartUI(this);

        m_queue = new RHQueue();
        ScaledTime.SetScale(0f);
        m_lastScaled = ScaledTime.UITimeElapsed;

        this.m_listeners_with_scores = new Dictionary<RHListener, float>();
        this.m_listeners = new List<RHListener>();
        this.m_speakers = new List<RHSpeaker>();
        
        m_speakerColorMaps = new Dictionary<RHSpeaker, Color>();
        int colorIndex = 0;
        m_startingSpeaker = startingSpeaker;
        foreach (RHSpeaker s in participants)
        {
            initializeSpeaker(s, colorIndex,startingSpeaker);
            colorIndex = (colorIndex + 1) % m_defaultColors.Count;
        }
        RefreshStatementMenu(m_startingSpeaker);
        m_previousStatements = new List<RHStatement>();
        RHManager.ClearHistory();
        RHManager.SetHistoryTextActive(true);
        RHManager.AddHistoryText(m_introText);
    }

    
    public List<RHStatement> GetValidStatements(RHSpeaker speaker )
    {
        return speaker.AvailableStatements;
    }
    
    public void SkipToNextStatment()
    {
        if (m_currentStatement != null)
            ScaledTime.skipTime(m_nextStatementEnd - ScaledTime.UITimeElapsed,true);
    }
    public void QueueStatement(RHStatement statement, RHSpeaker speaker,bool stack = false)
    {
        if (!m_battleStarted)
            startActiveBattle();
        
        statement.OnStatementQueued(speaker);
        RHRegisteredStatement rs = new RHRegisteredStatement(statement, speaker,this);
        m_queue.RegisterStatement(rs);
        bool clickable = speaker == m_startingSpeaker && ((statement as RHSDeadAir) == null);
        m_timeUI.AddItem(rs, m_speakerColorMaps[speaker], m_queue.CalculateQueuedStatementExecutionTime(rs, m_nextStatementEnd),stack, clickable);
        if (!m_battleStarted)
            advanceStatements();
        m_battleStarted = true;
    }
    public void DequeueStatement(RHRegisteredStatement rs)
    {
        rs.statement.OnStatementCancelled(rs.speaker);
        m_timeUI.RemoveItem(rs);
        m_queue.RemoveStatement(rs);
    }

    public void ModifyInterestTime(float delta)
    {
        m_nextInterestTimeLimit += delta;
    }
    public void ModifyListenerPersuasion(RHListener l, float delta)
    {
        if (m_listeners.Contains(l))
        {
            m_listeners_with_scores[l] += delta;
        }
        return;
    }    

    public void SetDialogueBox(GameObject go) {
        m_currentDialogueBox = go;
    }
    
    public void RefreshStatementMenu(RHSpeaker s)
    {
        List<RHStatement> stlist = GetAvailableStatements(s);
        float scroll = 0f;
        DialogueOptionBox dop = FindObjectOfType<DialogueOptionBox>();
        if (dop != null)
        {
            Destroy(dop.gameObject);
            scroll = dop.GetScrollValue();
        }
        RHManager.CreateDialogueOptionList(stlist, s, this, "Select your next statement",scroll);
    }
    protected virtual List<RHStatement> GetAvailableStatements(RHSpeaker speaker)
    {
        return speaker.AvailableStatements;
    }
    protected virtual bool isRhetoricBattleFinished()
    {
        if (ScaledTime.UITimeElapsed > m_nextInterestTimeLimit)
            return true;
        foreach (RHListener l in m_listeners)
            if (l.GetComponent<RHSpeaker>() != m_startingSpeaker && m_listeners_with_scores[l] < MaxValue)
                return false;
        return true;
    }

    protected virtual bool processFinish(RHListener listener, RHUIFinishWindow uif)
    {
        foreach (RHSpeaker s in m_speakers)
        {
            Dictionary<RHStat, float> d = listener.GetDifferenceStats(s);
            foreach (RHStat stat in d.Keys)
                listener.ModifyStat(s, this, stat, d[stat] * m_statRetainAmount, true);
            listener.ClearTempStats(s);
        }

        if (m_listeners_with_scores[listener] > m_threashould)
        {
            OnConversationSuccess(listener, uif);
            return true;
        }
        else
        {
            OnConversationFailure(listener, uif);
            return false;
        }
    }
    protected virtual void OnConversationSuccess(RHListener listener, RHUIFinishWindow uif)
    {
        foreach (RHFinishEffect rfe in m_winEffects)
        {
            uif.AddListenerResult( rfe.ExecuteFinishEffect(m_startingSpeaker, listener));
        }
    }
    protected virtual void OnConversationFailure(RHListener listener, RHUIFinishWindow uif)
    {
        foreach (RHFinishEffect rfe in m_failEffects)
        {
            uif.AddListenerResult(rfe.ExecuteFinishEffect(m_startingSpeaker, listener));
        }
    }

    void Update()
    {
        if (!m_battleStarted)
            return;

        UpdateParticipants();
        
        if (ScaledTime.UITimeElapsed >= m_nextStatementEnd)
        {
            callNextStatement();
            advanceStatements();
        }
        if (isRhetoricBattleFinished())
            OnFinish();

        processEvents();
        
        processSlowInput();
    }

    private void initializeSpeaker(RHSpeaker s, int colorIndex, RHSpeaker startingSpeaker)
    {
        this.m_speakers.Add(s);
        RHListener l = s.GetComponent<RHListener>();
        this.m_listeners_with_scores.Add(l, startingScore(l));
        this.m_listeners.Add(l);
        m_queue.RegisterSpeaker(s);
        if (s.speakerColor == Color.white)
            m_speakerColorMaps[s] = m_defaultColors[colorIndex];
        else
            m_speakerColorMaps[s] = s.speakerColor;

        List<RHStatement> stlist = GetAvailableStatements(s);
        s.OnRhetoricStart(stlist, this, this.m_listeners);

        if (s == startingSpeaker)
            return;

        GameObject go = Instantiate(RHManager.PrefabUIListener(), RHManager.ListenersBaseTransform());
        go.GetComponent<RHUIListener>().InitializeUI(l, this, startingSpeaker, m_speakerColorMaps[l.GetComponent<RHSpeaker>()]);
        l.GetComponent<AITaskManager>()?.triggerEvent(new AIEVConversationStarted(this));
    }
    private void ProcessHistoryText(RHStatement st, RHSpeaker speaker, RHListener listener, float diff)
    {
        foreach (RHResponseString s in m_registeredResponses)
        {
            if (s.IsConditionTrue(st, m_previousStatements, speaker, listener, diff,m_startingSpeaker))
                RHManager.AddHistoryText(s);
        }
    }
    private int GetNumberOfOccurances(RHStatement baseStatement)
    {
        int i = 0;
        foreach (RHStatement s in m_previousStatements)
        {
            if (s.StatementName == baseStatement.StatementName)
                i++;
        }
        return i;
    }
    public void CloseConversation()
    {
        RHManager.UITime().SetActive(false);
        RHManager.UITime().GetComponent<RHUITime>().ClearItems();
        RHManager.ClearHistory();
        RHManager.SetHistoryTextActive(false);
        RHManager.SetResourceUIInActive();
        RHManager.FinishBox.gameObject.SetActive(false);

        m_isFinished = true;
        if (m_currentDialogueBox != null)
            Destroy(m_currentDialogueBox);
        if (m_destroyAfterFinish)
            Destroy(gameObject);
        ScaledTime.SetPause(false, true);
        ScaledTime.SetScale(1f);
    }
    private void OnFinish()
    {
        RHUIFinishWindow finWin = RHManager.FinishBox.GetComponent<RHUIFinishWindow>();
        
        RHManager.FinishBox.gameObject.SetActive(true);
        bool success = true;
        foreach (RHListener listener in m_listeners)
        {
            success = success && processFinish(listener,finWin);
        }
        finWin.SetConversation(this, success);
        ScaledTime.SetPause(true, true);
        
    }
    private void processSlowInput()
    {
        if (!m_canSlow)
            return;
        if (Input.GetKeyDown("space"))
        {
            if (ScaledTime.GetScale(true) == 0.0f)
            {
                RHManager.SetPause(false);
            }
            else if (ScaledTime.GetScale(true) == 1.0f)
            {
                RHManager.SetSlowTextActive(true);
                ScaledTime.SetScale(0.5f, true);
            }
            else
            {
                RHManager.SetSlowTextActive(false);
                ScaledTime.SetScale(1.0f, true);
            }
        }
    }

    private void startActiveBattle()
    {

        ScaledTime.SetPause(false, true);

    }
    
    private void onStatementStarted(RHRegisteredStatement st)
    {
        m_currentStatement = st;
        st.statement.OnStatementStarted(st.speaker);
        registerEvents(st.statement.RHEvents, st.speaker, ScaledTime.UITimeElapsed);
        m_nextStatementEnd = ScaledTime.UITimeElapsed + m_currentStatement.statement.Time;
    }
    private void registerEvents(List<RHEvent> eList, RHSpeaker speaker, float startingTime)
    {
        foreach (RHEvent e in eList)
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
        m_registeredEvents[eventTime].Add(new RegisteredEvent(e, speaker));
    }
    private void processEvents()
    {
        if (ScaledTime.UITimeElapsed > m_nextEventTime)
        {
            foreach (RegisteredEvent e in m_registeredEvents[m_nextEventTime])
            {
                foreach (RHListener l in m_listeners)
                {
                    if (e.speaker.gameObject != l.gameObject)
                        e.m_event.ExecuteEvent(e.speaker, l);
                }
            }
            m_registeredEvents.Remove(m_nextEventTime);
            if (m_registeredEvents.Count > 0)
            {
                m_nextEventTime = Mathf.Min(m_registeredEvents.Keys.ToArray());
            }
            else
            {
                m_nextEventTime = float.MaxValue;
            }
            RefreshStatementMenu(m_startingSpeaker);
        }
    }

    private void callNextStatement()
    {
        if (m_currentStatement == null)
        {
            Debug.LogError("Somehow, the current statement is null.");
            return;
        }
        BroadcastStatement(m_currentStatement);
        m_previousStatements.Add(m_currentStatement.statement);
    }
    private void advanceStatements()
    {
        RHRegisteredStatement next_statement = m_queue.PopStatement();
        if (next_statement == null)
        {
            QueueStatement(RHManager.DeadAirPrefab.GetComponent<RHStatement>(), m_startingSpeaker, true);
            advanceStatements();
        }
        else
        {
            onStatementStarted(next_statement);
        }
    }
    private void BroadcastStatement(RHRegisteredStatement rs)
    {
        rs.statement.OnStatementFinished(rs.speaker);
        foreach (RHListener listener in m_listeners)
        {
            if (listener.GetComponent<RHSpeaker>() == rs.speaker)
                continue;
            listener.OnReceiveStatement(rs);
        }
    }

    private float startingScore(RHListener listener)
    {
        return StartingInterest;
    }

    private void UpdateParticipants()
    {
        float d = ScaledTime.UITimeElapsed - m_lastScaled;
        foreach (RHSpeaker s in m_speakers)
            foreach (RHListener l in m_listeners)
                if (l.GetComponent<RHSpeaker>() != s)
                    s.resourceUpdate(this, l, d);
        m_lastScaled = ScaledTime.UITimeElapsed;
    }
}    
