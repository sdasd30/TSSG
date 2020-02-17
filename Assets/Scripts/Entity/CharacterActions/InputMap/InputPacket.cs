﻿using UnityEngine;


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

    public void Combine(InputPacket ip)
    {
        movementInput = movementInput + movementInput;
        MousePointWorld = MousePointWorld;
        jump = jump || jump;
        leftMouse = leftMouse || leftMouse;
        leftMousePress = leftMousePress || leftMousePress;
    }

    public void MatchPlayerInput()
    {
        DefaultPlayerInput();
    }

    public void DefaultPlayerInput()
    {
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.z = Input.GetAxis("Vertical");
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
    }
}