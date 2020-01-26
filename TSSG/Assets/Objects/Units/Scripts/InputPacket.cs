using UnityEngine;

public class InputPacket {
    public Vector2 movementInput = new Vector2();
    public Vector2 MousePointWorld = new Vector2();
    public bool jump = false;
    public bool fire1 = false;
    public bool fire1Press = false;


    public void Combine(InputPacket ip)
    {
        movementInput = movementInput + ip.movementInput;
        MousePointWorld = ip.MousePointWorld;
        jump = jump || ip.jump;
        fire1 = fire1 || ip.fire1;
        fire1Press = fire1Press || ip.fire1Press;
    }
}