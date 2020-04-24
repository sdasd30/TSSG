using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WUIAIDebug : WUIBase
{

    public Text DispText;

    private AITaskManager m_AI;
    private AITaskManager m_Task;
    private string m_lastDisplayedString;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AI == null)
        {
            m_AI = Target.GetComponent<AITaskManager>();
            m_Task = Target.GetComponent<AITaskManager>();
        }
            
        string disp = "";
        if (m_AI.m_currentGoal == null || m_Task.m_currentTask == null)
            return;

        disp += "G: " + m_AI.m_currentGoal.name + " \nE: " + m_Task.debugLastEvent + "\n";
        disp += "TR: " + m_Task.debugLastTransition + " \ntsk: " + m_Task.m_currentTask.name + "\n";
        disp +=  m_Task.m_currentTask.debugExtraInfo();
        //disp += "EX: " + m_AI.m_currentGoal.outputDebugString();

        if (m_lastDisplayedString != disp)
        {
            DispText.text = disp;
            m_lastDisplayedString = disp;
        }
    }
}
