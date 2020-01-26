using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regenerate : MonoBehaviour
{
    // Start is called before the first frame update
    private Attackable myAttackable;
    //public float regenCooldown = .05f; //In seconds. Default is every 50 miliseconds
    public float regenRate; //Howmuch heals after one second
    private float defaultRate;
    public bool powered;
    public float powerUpCooling;

    //private float regenCooldownMax;
    void Start()
    {
        myAttackable = GetComponent<Attackable>();
        defaultRate = regenRate;
        //regenCooldownMax = regenCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (myAttackable != null) Regen();
        if (powered)
        {
            powerUpCooling -= Time.deltaTime;
            if (powerUpCooling <= 0)
            {
                powered = false;
                regenRate = defaultRate;
                //FindObjectOfType<PowerUpUI>().DestroyHealth();
            }
        }
    }


    private void Regen()
    {
        myAttackable.hp += regenRate * Time.deltaTime;
    }

    public float getRegen()
    {
        return regenRate;
    }

    public void TemporaryRegenChange(float time, float newRate)
    {
        regenRate = newRate;
        powerUpCooling = time;
        powered = true;
        //FindObjectOfType<PowerUpUI>().CreateHealth();
    }

    public float returnPowerUpCooling()
    {
        return powerUpCooling;
    }

}
