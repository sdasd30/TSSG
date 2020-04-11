using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WUIHPSimple : WUIBase
{
    [SerializeField]
    private Text DispText;
    [SerializeField]
    private Slider HPSlider;
    
    private float lastHealth;


    private void Update()
    {
        float h = Target.GetComponent<Attackable>().Health;
        if (h != lastHealth)
        {
            lastHealth = h;
            DispText.text = "HP: " + lastHealth + " / " + Target.GetComponent<Attackable>().MaxHealth;
            if (HPSlider != null)
                HPSlider.value = (lastHealth / Target.GetComponent<Attackable>().MaxHealth);
        }
    }
}
