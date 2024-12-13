using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(2, 10)]
    public string[] sentences;
}