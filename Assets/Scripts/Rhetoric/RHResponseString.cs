using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class RHResponseString
{
    public RHResponseString(string textValue)
    {
        this.textValue = textValue;
        this.fontSize = 12;
        this.fontColor = Color.white;
    }
    public RHResponseString(string textValue, int fontSize)
    {
        this.textValue = textValue;
        this.fontSize = fontSize;
        this.fontColor = Color.white;
    }
    public RHResponseString(string textValue, int fontSize, Color c)
    {
        this.textValue = textValue;
        this.fontSize = fontSize;
        this.fontColor = c;
    }
    public string textValue;
    public int fontSize;
    public Color fontColor;
}