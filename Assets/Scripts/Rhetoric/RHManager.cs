using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHManager : MonoBehaviour
{
    private static RHManager m_instance;

    [SerializeField]
    private GameObject ListenerUIPrefab;
    [SerializeField]
    private GameObject TimeUI;
    [SerializeField]
    private Transform ListenersTransform;
    [SerializeField]
    private Transform m_SlowText;
    [SerializeField]
    private RHUIHistoryText m_HistoryTextUI;
    [SerializeField]
    private RHUIResourceText m_ResourceUI;
    [SerializeField]
    private GameObject m_deadAirPrefab;
    public static GameObject DeadAirPrefab { get { return m_instance.m_deadAirPrefab; } private set { m_instance.m_deadAirPrefab = value; } }
    [SerializeField]
    private List<Sprite> m_ResourceIcons = new List<Sprite>();
    //public static List<Sprite> ResourceIcons { get { return m_instance.m_ResourceIcons; } }

    [SerializeField]
    private GameObject m_generateResourcesPrefab;
    public static GameObject GenerateResourcesPrefab { get { return m_instance.m_generateResourcesPrefab; } private set { m_instance.m_generateResourcesPrefab = value; } }

    public static RHManager Instance
    {
        get { return m_instance; }
        private set { m_instance = value; }
    }

    public static void StartRhetoricBattle(RHConversation conversation, List<RHSpeaker> participants, RHSpeaker startingSpeaker)
    {
        conversation.StartRhetoricBattle(participants, startingSpeaker);
    }

    public static void CreateDialogueOptionList(List<RHStatement> statements, RHConversation baseConversation,string prompt = "Select your next statement")
    {

        DialogueSelectionInitializer dialogue = new DialogueSelectionInitializer(prompt);
        Dictionary<RHType, List<RHStatement>> sortedStatements = new Dictionary<RHType, List<RHStatement>>();

        foreach (RHStatement s in statements)
        {
            if (!sortedStatements.ContainsKey(s.RhetoricType))
                sortedStatements[s.RhetoricType] = new List<RHStatement>();
            sortedStatements[s.RhetoricType].Add(s);
        }
        foreach (RHType t in sortedStatements.Keys)
        {
            void InitializeSubList(DialogueOption dop2)
            {
                DialogueSelectionInitializer dialogueSubList = new DialogueSelectionInitializer(prompt);
                foreach (RHStatement s in sortedStatements[t])
                {
                    DialogueOptionInitializer doi = convertToDialogueOption(s, baseConversation);
                    dialogueSubList.AddDialogueOption(doi);
                }
                void ReturnToBase(DialogueOption selectedOption)
                {
                    CreateDialogueOptionList(statements, baseConversation, prompt);
                }
                dialogueSubList.AddDialogueOption("Back", ReturnToBase);
                baseConversation.SetDialogueBox(TextboxManager.StartDialogueOptions(dialogueSubList));
            }
            dialogue.AddDialogueOption(RHTypeToString(t), InitializeSubList);
        }
        void Close(DialogueOption selectedOption) { }
        dialogue.AddDialogueOption("Close", Close);
        baseConversation.SetDialogueBox(TextboxManager.StartDialogueOptions(dialogue));
    }

    private static DialogueOptionInitializer convertToDialogueOption(RHStatement s, RHConversation baseConversation)
    {
        DialogueOptionInitializer doi = new DialogueOptionInitializer();
        void SelectionFunction(DialogueOption selectedOption)
        {
            baseConversation.QueueStatement(s, baseConversation.Speakers[0]);
        }
        string name = s.StatementName;
        if (name == null || name == "")
            name = s.gameObject.name;
        doi.SelectionText = name;
        doi.hoverText = s.GetHoverText(baseConversation);
        doi.OnSelect = SelectionFunction;

        doi.CloseDialogueWindow = false;
        string timeStr = s.Time.ToString() + " s";
        doi.AddTextIcon(timeStr, Color.white);
        
        foreach (RHListener l in baseConversation.Listeners.Keys)
        {
            float f = s.GetPower(baseConversation.Speakers[0], l, baseConversation);
            doi.AddTextIcon(f.ToString(), Color.red);
            doi.Interactable = s.IsEnabled(baseConversation.Speakers[0], l, baseConversation);
        }
        return doi;
    }
    public static string RHTypeToString(RHType type)
    {
        if (type == RHType.ETHOS)
            return "Ethos";
        else if (type == RHType.LOGOS)
            return "Logos";
        else if (type == RHType.PATHOS)
            return "Pathos";
        else
            return "Other";
    }

    public static GameObject PrefabUIListener()
    {
        return m_instance.ListenerUIPrefab;
    }
    public static GameObject UITime()
    {
        return m_instance.TimeUI;
    }

    public static Transform ListenersBaseTransform()
    {
        return m_instance.ListenersTransform;
    }
    public static void AddHistoryText(RHResponseString response)
    {
        if (response == null || response.textValue.Length == 0)
            return;
        AddHistoryText(response.textValue, response.fontColor, response.fontSize);
    }
    public static void AddHistoryText(string s)
    {
        AddHistoryText(s, Color.white);
    }
    public static void AddHistoryText(string s, Color c, int fontSize = 12)
    {
        m_instance.m_HistoryTextUI.AddLine(s, c, fontSize);
    }
    public static void ClearHistory()
    {
        m_instance.m_HistoryTextUI.ClearText();
    }
    public static void SetHistoryTextActive(bool isActive)
    {
        m_instance.m_HistoryTextUI.gameObject.SetActive(isActive);
    }
    public static void SetSlowTextActive(bool isActive)
    {
        m_instance.m_SlowText.gameObject.SetActive(isActive);
    }
    public static void SetResourceUIActive(RHSpeaker speaker)
    {
        m_instance.m_ResourceUI.gameObject.SetActive(true);
        m_instance.m_ResourceUI.SetSpeaker(speaker);
    }
    public static void SetResourceUIInActive()
    {
        m_instance.m_ResourceUI.gameObject.SetActive(false);
        m_instance.m_ResourceUI.SetSpeaker(null);
    }


    public static Sprite GetResourceIcon(RHResourceType resourceType)
    {
        return m_instance.m_ResourceIcons[(int)resourceType];
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
}
