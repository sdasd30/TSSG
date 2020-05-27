using UnityEngine;
public class ImpressionModifier 
{
    public string ID;
    public virtual int CurrentStack { get { return m_currentStack; } private set { m_currentStack = value; } }
    private int m_currentStack = 0;
    private float m_expirationDate;
    private bool m_isRefreshable = true;
    private int m_maxStack = 1;
    private float m_stackBufferTime = 1.0f;

    private float m_lastApplyTime = 0;
    private float duration;
    private float m_baseModValue;
    private bool m_isTimed = true;
    public ImpressionModifier(string newID,float modValue, float duration = -1,
        int m_maxStack = 1, float stack_buffer_time = 1.0f, int starting_stack  = 1, bool m_isRefreshable = true)
    {
        ID = newID;
        m_baseModValue = modValue;
        this.duration = duration;
        if (duration < 0)
            m_isTimed = false;
        m_expirationDate = ScaledTime.TimeElapsed + duration;
        this.m_isRefreshable = m_isRefreshable;
        m_lastApplyTime = ScaledTime.TimeElapsed;
        this.m_maxStack = m_maxStack;
        this.m_stackBufferTime = stack_buffer_time;
        this.m_currentStack = starting_stack;
    }
    public bool is_expired()
    {

        return !m_isTimed || ScaledTime.TimeElapsed > m_expirationDate;
    }
    public float getModValue()
    {
        return m_currentStack * m_baseModValue;
    }
    public bool attempt_stack(int stackVal = 1)
    {
        if (!m_isRefreshable)
            return false;
        if (ScaledTime.TimeElapsed < m_lastApplyTime + m_stackBufferTime)
            return false;
        m_lastApplyTime = ScaledTime.TimeElapsed;
        m_expirationDate = ScaledTime.TimeElapsed + duration;
        int m_old = m_currentStack;
        m_currentStack = Mathf.Min(m_maxStack, m_currentStack + stackVal);
        return (m_old != m_currentStack);
    }
}
