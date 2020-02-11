using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPText : MonoBehaviour
{
    public GameObject player;
    Text me;
    int maxHP;
    int currentHP;
    // Start is called before the first frame update
    public void init(GameObject x)
    {
        player = x;
        me = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) updateText();
    }

    private void updateText()
    {
        currentHP = (int)player.GetComponent<Attackable>().Health;
        maxHP = (int)player.GetComponent<Attackable>().MaxHealth;
        if (currentHP <= 0) currentHP = 0;
        me.text = "HP: " + currentHP + "/" + maxHP;
    }

}
