using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "Scriptable Objects/TextData")]
public class TextData : ScriptableObject
{
    public string[] textLines;  
    public string lineSpecificText;

    private int currentIndex;
    public string GetTextLine()
    {
        if (textLines.Length == 0)
            return "";
        Debug.Log("Índice actual: " + currentIndex);
        string line = textLines[currentIndex];

        currentIndex++;

        if (currentIndex >= textLines.Length)
            currentIndex = 0;

        return line;
    }

    public void ResetText()
    {
        currentIndex = 0;
    }
}
    /*public string GetTextLine(int index)
    {
        if (index >= 0 && index < textLines.Length)
        {
            return textLines[index];
        }
        else
        {
            Debug.LogWarning("Index out of range for textLines array.");
            return string.Empty;
        }
    }*/

