using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHDebugConvoGenerator : MonoBehaviour
{

    [SerializeField]
    private GameObject m_ConversationPrefab;
    [SerializeField]
    private Vector2 m_conversationTimeRange;
    [SerializeField]
    private Vector2 m_conversationThreashouldRange;
    [SerializeField]
    private Vector2 m_conversationMaxRange;
    [SerializeField]
    private RHSpeaker DefaultSpeaker;
    [SerializeField]
    private RHListener DefaultListener;

    [SerializeField]
    private bool m_autoGenerateListener = true;

    [SerializeField]
    private Vector2 m_listenerTrustRange;
    [SerializeField]
    private Vector2 m_listenerFavorRange;
    [SerializeField]
    private Vector2 m_listenerAuthorityRange;
    [SerializeField]
    private Vector2 m_listenerEmotionsRange;

    private GameObject m_conversation;

    private bool convoGenerated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_conversation == null && !convoGenerated)
        {
            m_conversation = GenerateConversation();
            if (m_autoGenerateListener)
            {
                randomizeListener(DefaultListener, m_conversation.GetComponent<RHConversation>());
            }
            List<RHSpeaker> participants = new List<RHSpeaker>();
            participants.Add(DefaultSpeaker);
            participants.Add(DefaultListener.GetComponent<RHSpeaker>());
            RHManager.StartRhetoricBattle(m_conversation.GetComponent<RHConversation>(), participants, DefaultSpeaker);
            convoGenerated = true;
        }

    }

    private GameObject GenerateConversation()
    {
        GameObject o = Instantiate(m_ConversationPrefab);
        float timeLimit = Random.Range(m_conversationTimeRange.x, m_conversationTimeRange.y);
        float threashould = Random.Range(m_conversationThreashouldRange.x, m_conversationThreashouldRange.y);
        float maxValue = Random.Range(m_conversationMaxRange.x, m_conversationMaxRange.y);
        o.GetComponent<RHConversation>().SetInitialValues(timeLimit, maxValue, threashould);
        return o;
    }

    private void randomizeListener(RHListener l, RHConversation c)
    {
        l.SetEmotionalIntensity( Random.Range(m_listenerEmotionsRange.x, m_listenerEmotionsRange.y) );
        l.ModifyStat(DefaultSpeaker, c, RHStat.AUTHORITY, Random.Range(m_listenerAuthorityRange.x, m_listenerAuthorityRange.y));
        l.ModifyStat(DefaultSpeaker, c, RHStat.FAVOR, Random.Range(m_listenerFavorRange.x, m_listenerFavorRange.y));
        l.ModifyStat(DefaultSpeaker, c, RHStat.TRUST, Random.Range(m_listenerTrustRange.x, m_listenerTrustRange.y));
    }
}
