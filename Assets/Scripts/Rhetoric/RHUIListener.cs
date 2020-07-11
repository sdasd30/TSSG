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
    public void InitializeUI(RHListener listener, RHConversation conv, RHSpeaker startingSpeaker, Color backgroundColor)
    {
        GetComponent<Image>().color = new Color(backgroundColor.r / 2f, backgroundColor.g / 2f, backgroundColor.r / 2f, 0.9f);
        MoniteringConversation = conv;
        Speaker = startingSpeaker;
        Listener = listener;
    }
    // Update is called once per frame
    void Update()
    {
        if (MoniteringConversation == null || MoniteringConversation.IsFinished)
            Destroy(gameObject);
        if (Listener == null)
            return;
        float v = MoniteringConversation.Listeners[Listener];
        ProgressObject.transform.Find("Slider").GetComponent<Slider>().value = v / MoniteringConversation.MaxValue;
        ProgressObject.transform.Find("Label").GetComponent<Text>().text = v.ToString("F1") + " / " + MoniteringConversation.Threashould.ToString("F1");
        VictoryMarker.anchoredPosition = new Vector2((MoniteringConversation.Threashould / MoniteringConversation.MaxValue) * 495, -25);
        FavorText.text =  Listener.IsStatHidden(RHStat.FAVOR) ? "??" : Listener.GetFavor(Speaker).ToString("F1");
        AuthorityText.text = Listener.IsStatHidden(RHStat.AUTHORITY) ? "??" : Listener.GetAuthority(Speaker).ToString("F1");
        TrustText.text = Listener.IsStatHidden(RHStat.TRUST) ? "??" : Listener.GetTrust(Speaker).ToString("F1");
        EmotionText.text = Listener.IsStatHidden(RHStat.EMOTIONS) ? "??" : Listener.GetEmotionalIntensity().ToString("F1");

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
