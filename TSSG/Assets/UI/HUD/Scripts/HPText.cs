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
        currentHP = (int)player.GetComponent<Attackable>().hp;
        maxHP = (int)player.GetComponent<Attackable>().maxHP;
        if (currentHP <= 0) currentHP = 0;
        me.text = "HP: " + currentHP + "/" + maxHP;
    }

}
