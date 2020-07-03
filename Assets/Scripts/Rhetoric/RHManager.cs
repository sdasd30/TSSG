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
    private Transform m_PauseText;
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

    public static void CreateDialogueOptionList(List<RHStatement> statements, RHSpeaker speaker, RHConversation baseConversation, string prompt = "Select your next statement", float scroll = 0.0f)
    {

        DialogueSelectionInitializer dialogue = new DialogueSelectionInitializer(prompt);
        List<RHStatement> sortedStatements = new List<RHStatement>();
        sortedStatements = RHManager.SortedList(statements, speaker, baseConversation);
        DialogueOptionBox box = null;
        foreach (RHStatement s in sortedStatements)
        {
            DialogueOptionInitializer doi = convertToDialogueOption(s, statements, speaker, baseConversation, box);
            dialogue.AddDialogueOption(doi);
        }
        void Close(DialogueOption selectedOption) { }
        dialogue.AddDialogueOption("Close", Close);
        GameObject go = TextboxManager.StartDialogueOptions(dialogue);
        box = go.GetComponent<DialogueOptionBox>();
        box.SetScrollValue(scroll);
        baseConversation.SetDialogueBox(go);
    }

    public static void CreateDialogueOptionListSortedType(List<RHStatement> statements, RHSpeaker speaker, RHConversation baseConversation, string prompt = "Select your next statement")
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
                DialogueOptionBox box = null;
                foreach (RHStatement s in sortedStatements[t])
                {
                    DialogueOptionInitializer doi = convertToDialogueOption(s, statements, speaker, baseConversation,box);
                    dialogueSubList.AddDialogueOption(doi);
                }
                void ReturnToBase(DialogueOption selectedOption)
                {
                    CreateDialogueOptionListSortedType(statements, speaker,baseConversation, prompt);
                }
                dialogueSubList.AddDialogueOption("Back", ReturnToBase);
                GameObject go = TextboxManager.StartDialogueOptions(dialogueSubList);
                box = go.GetComponent<DialogueOptionBox>();
                baseConversation.SetDialogueBox(go);
            }
            dialogue.AddDialogueOption(RHTypeToString(t), InitializeSubList);
        }
        void Close(DialogueOption selectedOption) { }
        dialogue.AddDialogueOption("Close", Close);
        baseConversation.SetDialogueBox(TextboxManager.StartDialogueOptions(dialogue));
    }

    private static DialogueOptionInitializer convertToDialogueOption(RHStatement s, List<RHStatement> allStatements, RHSpeaker speaker, RHConversation baseConversation, DialogueOptionBox oldBox)
    {
        DialogueOptionInitializer doi = new DialogueOptionInitializer();
        void SelectionFunction(DialogueOption selectedOption)
        {
            baseConversation.QueueStatement(s, baseConversation.Speakers[0]);
            RHManager.CreateDialogueOptionList(allStatements, speaker, baseConversation,"Select your next Statement", oldBox.GetScrollValue());
        }
        string name = s.StatementName;
        if (name == null || name == "")
            name = s.gameObject.name;
        doi.SelectionText = name;
        doi.hoverText = s.GetHoverText(baseConversation);
        doi.OnSelect = SelectionFunction;

        doi.CloseDialogueWindow = true;
        string timeStr = s.Time.ToString() + " s";
        doi.AddTextIcon(timeStr, Color.white);
        
        foreach (RHListener l in baseConversation.Listeners.Keys)
        {
            float f = s.GetPower(baseConversation.Speakers[0], l, baseConversation);
            doi.AddTextIcon(f.ToString(), Color.red);
            doi.Interactable = s.IsEnabled(baseConversation.Speakers[0],baseConversation);
        }
        return doi;
    }
    public static string RHTypeToString(RHType type)
    {
        if (type == RHType.ARGUMENT)
            return "Argument";
        else if (type == RHType.INFLUENCE)
            return "Influence";
        else if (type == RHType.RESPONSE)
            return "Response";
        else if (type == RHType.QUESTION)
            return "Question";
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
        if (response.isPausing)
            SetPause(true);
        AddHistoryText(response.textValue, response.fontColor, response.fontSize,response.isPausing);
    }
    public static void AddHistoryText(string s)
    {
        AddHistoryText(s, Color.white);
    }
    public static void AddHistoryText(string s, Color c, int fontSize = 12,bool isPausing = false)
    {
        m_instance.m_HistoryTextUI.AddLine(s, c, fontSize,isPausing);
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
    public static void SetPause(bool isPaused)
    {
        m_instance.m_PauseText.gameObject.SetActive(isPaused);
        ScaledTime.SetPause(isPaused, true);

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

    public static List<RHStatement> SortedList(List<RHStatement> initialList,RHSpeaker speaker, RHConversation c)
    {
        initialList.Sort((p1, p2) => p1.GetListRankingPriority(speaker,c).CompareTo(p2.GetListRankingPriority(speaker, c)));
        return initialList;
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
