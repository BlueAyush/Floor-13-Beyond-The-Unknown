using System.Collections.Generic;
using UnityEngine;

public class DialogueLog : MonoBehaviour
{
    public static DialogueLog Instance;

    private List<string> dialogueHistory = new List<string>();

    void Awake()
    {
        Instance = this;
    }

    public void AddLine(string speaker, string text)
    {
        string formattedLine;

        if (string.IsNullOrEmpty(speaker))
        {
            formattedLine = text;
        }
        else
        {
            formattedLine = speaker + ": " + text;
        }

        dialogueHistory.Add(formattedLine);

        Debug.Log("Dialogue Log: " + formattedLine);
    }

    public List<string> GetHistory()
    {
        return dialogueHistory;
    }

    public void ClearHistory()
    {
        dialogueHistory.Clear();
    }
}