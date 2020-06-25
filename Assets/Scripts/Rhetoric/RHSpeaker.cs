using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

public class RHSpeaker : MonoBehaviour
{
    public bool DebugIncludeAllStatements;
    public List<RHStatement> AvailableStatements;
    public Color speakerColor = Color.white;
    private List<RHPersonalityTrait> m_traits = new List<RHPersonalityTrait>();
    public virtual List<RHPersonalityTrait> Traits { get { return m_traits; } }

    [SerializeField]
    private List<RHResource> m_resources = new List<RHResource>();
    public virtual List<RHResource> RHResources { get { return m_resources; } }

    private bool m_resourcesDirty = false;
    // Start is called before the first frame update
    void Start() {
        if (DebugIncludeAllStatements)
            InitializeDebug();
        RHPersonalityTrait[] traits = GetComponentsInChildren<RHPersonalityTrait>();
        m_traits.AddRange(traits);
    }
    public void ModifyResources(SerializableDictionary<RHResourceType, int> m_modifySpeakerResources, bool invert = false)
    {
        foreach (RHResourceType rh in m_modifySpeakerResources.Keys)
        {
            bool found = false;
            foreach(RHResource r in m_resources)
            {
                if (r.m_resourceType == rh)
                {
                    r.m_Amount += m_modifySpeakerResources[rh];
                    found = true;
                    m_resourcesDirty = true;
                    break;
                }
            }
        }
    }
    public void AddResource(RHResource addedResource)
    {
        m_resourcesDirty = true;
        foreach (RHResource r in m_resources)
        {
            if (r.m_resourceType == addedResource.m_resourceType)
            {
                r.m_Amount += addedResource.m_Amount;
                return;
            }
        }
        m_resources.Add(addedResource);
    }
    public bool CheckResourcesDirty()
    {
        if (m_resourcesDirty)
        {
            m_resourcesDirty = false;
            return true;
        }
        return false;
    }
    public bool meetsRequirements(SerializableDictionary<RHResourceType, int> m_requirements)
    {
        foreach (RHResourceType rh in m_requirements.Keys)
        {
            bool found = false;
            foreach (RHResource r in m_resources)
            {
                if (r.m_resourceType == rh)
                {
                    if (r.m_Amount < m_requirements[rh])
                        return false;
                    break;
                }
            }
            if (!found)
                return false;
        }
        return true;
    }
    public void OnRhetoricStart( List<RHStatement> availableStatements, RHConversation conversation, Dictionary<RHListener, float> listeners)
    {
        if (GetComponent<MovementBase>() != null && GetComponent<MovementBase>().IsPlayerControl)
        {
            RHManager.CreateDialogueOptionList(availableStatements,this, conversation);
            RHManager.SetResourceUIActive(this);
        }
        foreach (RHPersonalityTrait t in m_traits)
        {
            t.OnSpeakerStart(this, listeners, conversation);
        }
    }
    public void resourceUpdate(RHConversation c,RHListener l, float conversationDelta)
    {
        foreach (RHResource r in m_resources)
            r.OnUpdate(this,l, c, conversationDelta);
    }
    private void InitializeDebug()
    {
        AvailableStatements = new List<RHStatement>();
        foreach (System.Object o in Resources.LoadAll("RHStatements"))
        {
            GameObject instance = o as GameObject;
            if (instance.GetComponent<RHStatement>() != null)
                AvailableStatements.Add(instance.GetComponent<RHStatement>());
        }
    }
}
