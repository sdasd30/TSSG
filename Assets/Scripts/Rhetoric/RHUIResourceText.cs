using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RHUIResourceText : MonoBehaviour
{

    [SerializeField]
    private RHSpeaker speaker;
    [SerializeField]
    private RectTransform m_iconTransform;

    private int lastValue = 0;
    
    public void SetSpeaker(RHSpeaker speaker)
    {
        this.speaker = speaker;
    }

    // Update is called once per frame
    void Update()
    {
        if (speaker != null && speaker.CheckResourcesDirty())
            updateIcons();
    }

    void updateIcons()
    {
        foreach (Transform rt in m_iconTransform)
            Destroy(rt.gameObject);
        foreach(RHResource r in speaker.RHResources)  
        {
            GameObject icon = Instantiate(TextboxManager.ImagePrefab, m_iconTransform);
            icon.GetComponentInChildren<Image>().sprite = RHManager.GetResourceIcon(r.m_resourceType);
            icon.GetComponentInChildren<TextMeshProUGUI>().text = r.m_Amount.ToString();
        }
    }
}
