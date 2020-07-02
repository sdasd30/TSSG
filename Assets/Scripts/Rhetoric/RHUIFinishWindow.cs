using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RHUIFinishWindow : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI m_VictoryDefeatText;
    [SerializeField]
    public TextMeshProUGUI m_ResultText;
    [SerializeField]
    public TextMeshProUGUI m_BodyText;

    private RHConversation m_conversation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnConfirm()
    {
        m_conversation?.CloseConversation();
    }
    public void SetConversation(RHConversation c, bool Succeeded)
    {
        m_conversation = c;
        m_VictoryDefeatText.text = (Succeeded) ? "Conversation Goals Succeeded" : "Conversation Goals Failed";
        m_BodyText.text = "";

    }
    public void AddListenerResult(string result)
    {
        m_BodyText.text = m_BodyText.text + "\n" + result;
    }
}
