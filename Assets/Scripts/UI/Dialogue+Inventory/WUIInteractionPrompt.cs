using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WUIInteractionPrompt : WUIBase
{
    [SerializeField]
    private TextMeshProUGUI m_promptUI;
    private int m_lastLength = 0;
    void Start()
    {
        m_promptUI = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Target != null)
        {
            Interactor io = Target.GetComponent<Interactor>();
            if (io.OverlapInteractions.Count != m_lastLength)
            {
                if (io.OverlapInteractions.Count > 0)
                {
                    Interactable i = io.OverlapInteractions[io.OverlapInteractions.Count - 1];
                    if (i.ShouldDisplayPrompt())
                        m_promptUI.text = "Press ' Interact ' " + i.GetInteractionPrompt();
                    else
                        m_promptUI.text = "";
                } else
                {
                    m_promptUI.text = "";
                }
                m_lastLength = io.OverlapInteractions.Count;
            }
        } else
        {
            //Target = CurrentPlayerSettings.Instance.CurrentPlayer;
        }
    }
}
