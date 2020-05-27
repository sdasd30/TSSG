using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHManager : MonoBehaviour
{
    private static RHManager m_instance;

    public static RHManager Instance
    {
        get { return m_instance; }
        private set { m_instance = value; }
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void StartRhetoricBattle(RHConversation conversation, RHSpeaker speaker, RHListener listener)
    {
        List<RHSpeaker> newListSpeakers = new List<RHSpeaker>();
        newListSpeakers.Add(speaker);
        List<RHListener> newListListeners = new List<RHListener>();
        newListListeners.Add(listener);
        StartRhetoricBattle(conversation, newListSpeakers, newListListeners);
    }

    public static void StartRhetoricBattle(RHConversation conversation,List<RHSpeaker> speakers, List<RHListener> listeners)
    {

    }

    private static void DisplayStatementOptions(List<RHStatement> statements)
    {

    }

    private void executeStatement(RHConversation conversation, RHSpeaker speaker, RHStatement executedStatement)
    {
        conversation.ProcesssStatement(speaker, executedStatement);
    }
}
