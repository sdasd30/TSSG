

[System.Serializable]
public class Resistence
{
    public ElementType Element;
    public float Duration = 0f;
    public float Percentage = 0f;
    public bool Timed;
    public float StunResist = 0f;
    public float KnockbackResist = 0f;
    public bool AvoidOverflow = true;
    public float OverflowAmount = 0f;
}
