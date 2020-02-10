using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBoxes : MonoBehaviour
{
    private GameObject player;
    public GameObject HPBox;
    public GameObject HPBoxLeft;
    public GameObject HPBoxRight;
    private Attackable playerAtackable;
    private int maxHP;
    // Start is called before the first frame update

    public void init(GameObject x)
    {
        player = x;
        playerAtackable = player.GetComponent<Attackable>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        maxHP = (int)playerAtackable.MaxHealth;
        if (player != null && transform.childCount != maxHP) switchBoard(maxHP);
    }

    private void switchBoard(int mhp)
    {
        GameObject box;
        if (transform.childCount == 0)
        {
            box = Instantiate(HPBoxLeft, new Vector3(), new Quaternion());
            box.transform.SetParent(this.transform);
        }

        if (transform.childCount < mhp)
        {
            while (transform.childCount < mhp - 1)
            {
                box = Instantiate(HPBox, new Vector3(), new Quaternion());
                box.transform.SetParent(this.transform);
            }
            box = Instantiate(HPBoxRight, new Vector3(), new Quaternion());
            box.transform.SetParent(this.transform);
        }

        if (transform.childCount > mhp)
        {
            Destroy(transform.GetChild(1).gameObject);
        }
    }
}
