using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InputKey
{
    Fire,
    MiddleMouse,
    Fire2,
    Interact,
    Item1,
    Item2,
    Item3,
    Inventory,
    Pause
}

public enum InputAxis
{
    Horizontal,
    Vertical
}
public class InputManager : MonoBehaviour
{
    public SerializableDictionary<InputKey,List<string>> KeyMaps = new SerializableDictionary<InputKey,List<string>>();
    public SerializableDictionary<InputAxis, string> AxisMaps = new SerializableDictionary<InputAxis, string>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public bool GetKeyDown(InputKey k)
    {
        if (KeyMaps.ContainsKey(k))
        {
            foreach (string button in KeyMaps[k])
                if (Input.GetButtonDown(button))
                    return true;
        }
        return false;
    }
    public bool GetKey(InputKey k)
    {
        if (KeyMaps.ContainsKey(k))
        {
            foreach (string button in KeyMaps[k])
                if (Input.GetButton(button))
                    return true;
        }
        return false;
    }

    public float GetAxis(InputAxis k)
    {
        if (AxisMaps.ContainsKey(k))
        {
            return Input.GetAxis(AxisMaps[k]);
        }
        return 0f;
    }
}
