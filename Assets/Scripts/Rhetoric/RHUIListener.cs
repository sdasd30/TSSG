using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RHUIListener : MonoBehaviour
{
    public RHConversation MoniteringConversation;
    public RHSpeaker Speaker;
    public RHListener Listener;

    [SerializeField]
    private GameObject ProgressObject;
    [SerializeField]
    private RectTransform VictoryMarker;
    [SerializeField]
    private Text FavorText;
    [SerializeField]
    private Text AuthorityText;
    [SerializeField]
    private Text EmotionText;
    [SerializeField]
    private Text TrustText;

    private int numTraits = 0;

    // Update is called once per frame
    void Update()
    {
        if (Listener == null)
            return;
        float v = MoniteringConversation.Listeners[Listener];
        ProgressObject.transform.Find("Slider").GetComponent<Slider>().value = v / MoniteringConversation.MaxValue;
        ProgressObject.transform.Find("Label").GetComponent<Text>().text = v + " / " + MoniteringConversation.Threashould;
        VictoryMarker.anchoredPosition = new Vector2((MoniteringConversation.Threashould / MoniteringConversation.MaxValue) * 495, -25);
        FavorText.text = "Favor: " + Listener.GetFavor(Speaker);
        AuthorityText.text = "Authority: " + Listener.GetAuthority(Speaker);
        TrustText.text = "Authority: " + Listener.GetTrust(Speaker);
        EmotionText.text = "Emotion: " + Listener.GetEmotionalIntensity();

        int n = Listener.Traits.Count;
        if (n != numTraits)
        {
            foreach (Transform t in transform.Find("Icons"))
                Destroy(t.gameObject);
            foreach(RHPersonalityTrait t in Listener.Traits)
            {
                GameObject g = Instantiate(TextboxManager.ImagePrefab, transform.Find("icons"));
                g.GetComponent<Image>().sprite = t.TraitIcon;
                UIHoverText uiht = g.AddComponent<UIHoverText>();
                uiht.SetText(t.GetHoverText());
            }
            
            numTraits = n;
        }
    }
}
