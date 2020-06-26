using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueOptionBox : Textbox {

	public string Prompt = "";
    [SerializeField]
    TextMeshProUGUI m_Prompt;
    [SerializeField]
    Transform m_OptionsTransform;
    [SerializeField]
    GameObject m_scrollbar;

    List<DialogueOption> m_options;

	// Use this for initialization
	void Start () {
        m_Prompt.text = Prompt;
        if (m_OptionsTransform.childCount < 3)
            m_scrollbar.SetActive(false);
        else
            m_scrollbar.SetActive(true);
    }

	public void AddDialogueOption(DialogueOptionInitializer dop) {
        GameObject newOption;
        if (dop.AdditionalIcons.Count > 0)
        {
            newOption = Instantiate(FindObjectOfType<TextboxManager>().DialogueOptionWithIconsPrefab, m_OptionsTransform);
            foreach (GameObject go in dop.AdditionalIcons)
            {
                go.transform.SetParent(newOption.transform.Find("Icons"));
            }
        } else
        {
            newOption = Instantiate(FindObjectOfType<TextboxManager>().DialogueOptionPrefab, m_OptionsTransform);
        }
		newOption.GetComponent<DialogueOption> ().SelectionText = dop.SelectionText;
		newOption.GetComponent<DialogueOption> ().OnSelect = dop.OnSelect;
		newOption.GetComponent<DialogueOption> ().MasterBox = this;
		newOption.GetComponent<DialogueOption> ().remainderText = dop.remainderText;
		newOption.GetComponentInChildren<TextMeshProUGUI> ().text = TextboxManager.TrimSpecialSequences (dop.SelectionText);
        newOption.GetComponent<DialogueOption>().CloseDialogueWindow = dop.CloseDialogueWindow;

        newOption.GetComponent<Button>().interactable = dop.Interactable;
        if (dop.hoverText.Length > 0)
        {
            UIHoverText hoverText = newOption.AddComponent<UIHoverText>();
            hoverText.SetText(dop.hoverText);
        }
        EventSystem.current.SetSelectedGameObject(newOption);
        Vector2 v = m_OptionsTransform.gameObject.GetComponent<RectTransform>().sizeDelta;
        m_OptionsTransform.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(v.x, v.y + 51);

    }

	public void AddDialogueOption(string optionText, DialogueOption.SelectFunction func, string hoverText = "", bool autoClose = true) {
		GameObject newOption = Instantiate (FindObjectOfType<TextboxManager> ().DialogueOptionPrefab, m_OptionsTransform);
		newOption.GetComponent<DialogueOption> ().SelectionText = optionText;
		newOption.GetComponentInChildren<TextMeshProUGUI> ().text = TextboxManager.TrimSpecialSequences (optionText);
		newOption.GetComponent<DialogueOption> ().MasterBox = this;
		newOption.GetComponent<DialogueOption> ().OnSelect = func;
        newOption.GetComponent<DialogueOption>().CloseDialogueWindow = autoClose;
        if (hoverText.Length > 0)
        {
            Debug.Log("Setting: " + hoverText);
            UIHoverText txt = newOption.AddComponent<UIHoverText>();
            txt.SetText(hoverText);
        }
        EventSystem.current.SetSelectedGameObject(newOption);
	}
    public void SetScrollValue(float value)
    {
        m_scrollbar.GetComponent<Scrollbar>().value = value;
    }
    public float GetScrollValue()
    {
        return m_scrollbar.GetComponent<Scrollbar>().value;
    }

}
