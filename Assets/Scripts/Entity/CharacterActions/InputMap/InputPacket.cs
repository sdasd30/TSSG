using UnityEngine;
using System.Collections.Generic;

public class InputPacket {
    public Vector3 movementInput = new Vector3();
    public Vector3 MousePointWorld = new Vector3();
    public bool jump = false;
    public bool jumpDown = false;
    public bool leftMouse = false;
    public bool leftMousePress = false;
    public bool rightMouse = false;
    public bool rightMousePress = false;

    public bool middleMouse = false;
    public bool middleMousePress = false;

    public SerializableDictionary<InputKey, bool> InputKeyPressed = new SerializableDictionary<InputKey, bool>();
    public SerializableDictionary<InputKey, bool> InputKeyDown = new SerializableDictionary<InputKey, bool>();
    public bool interact = false;
    public bool inventory = false;
    public List<string> itemSlotUse = new List<string>();
    public void Combine(InputPacket ip)
    {
        movementInput = movementInput + ip.movementInput;
        MousePointWorld = ip.MousePointWorld;
        jump = jump || ip.jump;
        leftMouse = leftMouse || ip.leftMouse;
        leftMousePress = leftMousePress || ip.leftMousePress;
    }

    public void MatchPlayerInput()
    {
        DefaultPlayerInput();
    }

    public void DefaultPlayerInput()
    {
        movementInput.x = 0f;
        if (Input.GetButton("Left"))
            movementInput.x -= 1f;
        if (Input.GetButton("Right"))
            movementInput.x += 1f;
        movementInput.z = 0f;
        if (Input.GetButton("Down"))
            movementInput.z -= 1f;
        if (Input.GetButton("Up"))
            movementInput.z += 1f;
        jump = Input.GetButtonDown("Jump");
        leftMouse = Input.GetButton("Fire1");
        leftMousePress = Input.GetButtonDown("Fire1");
        InputKeyDown[InputKey.Fire] = Input.GetButton("Fire1");
        InputKeyPressed[InputKey.Fire] = Input.GetButtonDown("Fire1");

        rightMouse = Input.GetButton("Fire2");
        rightMousePress = Input.GetButtonDown("Fire2");
        InputKeyDown[InputKey.Fire2] = Input.GetButton("Fire2");
        InputKeyPressed[InputKey.Fire2] = Input.GetButtonDown("Fire2");

        middleMouse = Input.GetButton("Fire3");
        middleMousePress = Input.GetButtonDown("Fire3");
        InputKeyDown[InputKey.MiddleMouse] = Input.GetButton("Fire3");
        InputKeyPressed[InputKey.MiddleMouse] = Input.GetButtonDown("Fire3");

        MousePointWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (GameObject.FindObjectOfType<InputManager>() != null)
        {
            SerializableDictionary<InputKey, List<string>> my_keys = GameObject.FindObjectOfType<InputManager>().KeyMaps;
            foreach (InputKey ik in my_keys.Keys)
            {
                foreach (string s in my_keys[ik])
                {
                    if (Input.GetKeyDown(s))
                        InputKeyPressed[ik] = true;
                    if (Input.GetKey(s))
                        InputKeyDown[ik] = true;
                }
            } 
        } else {
            InputKeyPressed[InputKey.Inventory] = Input.GetKeyDown("i");
            InputKeyDown[InputKey.Inventory] = Input.GetKey("i");
            InputKeyPressed[InputKey.Interact] = Input.GetKeyDown("e");
            InputKeyDown[InputKey.Interact] = Input.GetKey("e");
        }
    }
}