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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (speaker.CheckResourcesDirty())
            updateIcons();
    }

    void updateIcons()
    {
        foreach (RectTransform rt in m_iconTransform)
            Destroy(rt);
        foreach(RHResource r in speaker.RHResources)  
        {
            GameObject icon = Instantiate(TextboxManager.ImagePrefab, m_iconTransform);
            icon.GetComponent<Image>().sprite = RHManager.GetResourceIcon(r.m_resourceType);
            icon.GetComponent<TextMeshProUGUI>().text = r.m_Amount.ToString();
        }
    }
}
