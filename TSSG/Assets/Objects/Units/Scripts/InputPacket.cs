using UnityEngine;

public class InputPacket {
    public Vector3 movementInput = new Vector2();
    public Vector2 MousePointWorld = new Vector2();
    public bool jump = false;
    public bool jumpDown = false;
    public bool fire1 = false;
    public bool fire1Press = false;
    public bool interact = false;
    public bool inventory = false;

    public void Combine(InputPacket ip)
    {
        movementInput = movementInput + ip.movementInput;
        MousePointWorld = ip.MousePointWorld;
        jump = jump || ip.jump;
        fire1 = fire1 || ip.fire1;
        fire1Press = fire1Press || ip.fire1Press;
    }
}