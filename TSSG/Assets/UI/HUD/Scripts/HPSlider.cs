using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPSlider : MonoBehaviour
{
    public GameObject player;
    private Attackable plr;
    private RectTransform fillArea;
    private Slider kk;
    int sliderMax;
    float sliderCurrent;
    // Start is called before the first frame update

    public void init(GameObject x)
    {
        player = x;
        plr = player.GetComponent<Attackable>();
    }

    void Start()
    {
        //player = FindObjectOfType<PlayerScore>().gameObject;
        fillArea = GetComponentInChildren<RectTransform>();
        kk = this.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) updateSlider();
    }

    private void updateSlider()
    {
        sliderMax = (int)plr.maxHP;
        sliderCurrent = plr.hp;
        kk.maxValue = sliderMax;
        kk.value = sliderCurrent;
        fillArea.offsetMax = new Vector2(((plr.maxHP * 64f)), fillArea.offsetMax.y);
    }
}
